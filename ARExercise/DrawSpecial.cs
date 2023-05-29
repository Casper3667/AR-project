using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace ARExercise
{
    internal class DrawSpecial
    {
        // Stolen from UtilityAR for reference purpose and to keep it self contained. Not modified.
        /// <summary>
        /// Draws a cube at the origin (0,0) in the world-coordinate. 
        /// </summary>
        /// <param name="img">The image to draw the cube onto</param>
        /// <param name="scale">The size of the cube</param>
        /// <param name="projection">the projection-matrix to use for converting world coordinates to screen coordinates</param>
        public static void DrawCube(IInputOutputArray img, Matrix<float> projection, float scale = 1)
        {
            Matrix<float>[] worldPoints = new[]
            {
                new Matrix<float>(new float[] { 0, 0, 0, 1 }), new Matrix<float>(new float[] { scale, 0, 0, 1 }),
                new Matrix<float>(new float[] { scale, scale, 0, 1 }), new Matrix<float>(new float[] { 0, scale, 0, 1 }),
                new Matrix<float>(new float[] { 0, 0, -scale, 1 }), new Matrix<float>(new float[] { scale, 0, -scale, 1 }),
                new Matrix<float>(new float[] { scale, scale, -scale, 1 }), new Matrix<float>(new float[] { 0, scale, -scale, 1 })
            };

            Point[] screenPoints = worldPoints
                .Select(x => UtilityAR.WorldToScreen(x, projection)).ToArray();

            Tuple<int, int>[] lineIndexes = new[] {
                Tuple.Create(0, 1), Tuple.Create(1, 2), // Floor
                Tuple.Create(2, 3), Tuple.Create(3, 0),
                Tuple.Create(4, 5), Tuple.Create(5, 6), // Top
                Tuple.Create(6, 7), Tuple.Create(7, 4),
                Tuple.Create(0, 4), Tuple.Create(1, 5), // Pillars
                Tuple.Create(2, 6), Tuple.Create(3, 7)
            };

            // Draw filled floor
            VectorOfVectorOfPoint floorContour = new(new VectorOfPoint(screenPoints.Take(4).ToArray()));
            CvInvoke.DrawContours(img, floorContour, -1, new MCvScalar(0, 255, 0), -3);

            // Draw top
            foreach (Tuple<int, int> li in lineIndexes.Skip(4).Take(4))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, new MCvScalar(255, 0, 0), 3);
            }

            // Draw pillars
            foreach (Tuple<int, int> li in lineIndexes.Skip(8).Take(4))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, new MCvScalar(0, 0, 255), 3);
            }
        }

        // draws a box with two sides forming a triangle invwards while the part in the air remains a square
        public static void DrawBoxInvert(IInputOutputArray img, Matrix<float> projection, float scale = 1, bool TwoFound = false)
        {
            float scale_half = scale / 2;
            float scale_partial = (scale_half / 2) + 0.1f;
            float scale_more = scale * 1.25f;

            MCvScalar color1 = new(255, 0, 0);
            MCvScalar color2 = new(200, 200, 0);
            MCvScalar color3 = new(150, 200, 200);

            if (TwoFound)
            {
                color1 = new MCvScalar(0, 255, 0);
                color2 = new MCvScalar(0, 255, 0);
                color3 = new MCvScalar(0, 255, 0);
            }

            Matrix<float>[] worldPoints = new[]
            {
                new Matrix<float>(new float[] { 0, 0, 0, 1 }), // bottom left - floor - 0
                new Matrix<float>(new float[] { scale, 0, 0, 1 }), // top left - floor - 1
                new Matrix<float>(new float[] { scale, scale, 0, 1 }), // top right - floor - 2
                new Matrix<float>(new float[] { 0, scale, 0, 1 }), // bottom right - floor - 3

                new Matrix<float>(new float[] { scale - scale_partial, scale_half, 0, 1 }), // top middle - 4
                new Matrix<float>(new float[] { scale_partial, scale_half, 0, 1 }), // bottom middle - 5

                new Matrix<float>(new float[] { 0, 0, -scale, 1 }), // bottom left - air - 6
                new Matrix<float>(new float[] { scale, 0, -scale, 1 }),// top left - air - 7
                new Matrix<float>(new float[] { scale, scale, -scale, 1 }), // top right - air - 8
                new Matrix<float>(new float[] { 0, scale, -scale, 1 }),// bottom right - air - 9
            };

            Point[] screenPoints = worldPoints
                .Select(x => UtilityAR.WorldToScreen(x, projection)).ToArray();

            Tuple<int, int>[] lineIndexes = new[] {
                Tuple.Create(0, 1), // Floor
                Tuple.Create(2, 3),
                Tuple.Create(0, 5), Tuple.Create(3, 5),
                Tuple.Create(2, 4), Tuple.Create(1, 4),

                Tuple.Create(0, 6), Tuple.Create(1, 7),
                Tuple.Create(2, 8), Tuple.Create(3, 9),

                Tuple.Create(6, 7), Tuple.Create(7, 8),
                Tuple.Create(8, 9), Tuple.Create(9, 6),
            };

            // Draw top
            foreach (Tuple<int, int> li in lineIndexes.Take(6))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color1, 3);
            }
            foreach (Tuple<int, int> li in lineIndexes.Skip(6).Take(4))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color2, 3);
            }
            foreach (Tuple<int, int> li in lineIndexes.Skip(10).Take(4))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color3, 3);
            }

        }

        // draws a circle-ish shape with 8 points
        public static void DrawCircle(IInputOutputArray img, Matrix<float> projection, float scale = 1, bool TwoFound = false)
        {
            float scale_half = scale / 2;
            float scale_partial = scale_half / 2;
            float scale_more = scale * 1.25f;

            MCvScalar color1 = new(158, 32, 240);
            MCvScalar color2 = new(258, 132, 140);
            MCvScalar color3 = new(158, 232, 170);

            if (TwoFound)
            {
                color1 = new MCvScalar(0, 255, 0);
                color2 = new MCvScalar(0, 255, 0);
                color3 = new MCvScalar(0, 255, 0);
            }

            Matrix<float>[] worldPoints = new[]
            {
                new Matrix<float>(new float[] { 0, 0, 0, 1 }), // bottom left - floor - 0
                new Matrix<float>(new float[] { scale, 0, 0, 1 }), // top left - floor - 1
                new Matrix<float>(new float[] { scale, scale, 0, 1 }), // top right - floor - 2
                new Matrix<float>(new float[] { 0, scale, 0, 1 }), // bottom right - floor - 3

                new Matrix<float>(new float[] { 0, 0, -scale, 1 }), // bottom left - air - 4
                new Matrix<float>(new float[] { scale, 0, -scale, 1 }),// top left - air - 5
                new Matrix<float>(new float[] { scale, scale, -scale, 1 }), // top right - air - 6
                new Matrix<float>(new float[] { 0, scale, -scale, 1 }),// bottom right - air - 7

                new Matrix<float>(new float[] { scale_half, -scale_partial, 0, 1 }), // left middle - 8
                new Matrix<float>(new float[] { scale_more, scale_half, 0, 1 }), // top middle - 9
                new Matrix<float>(new float[] { scale_half, scale_more, 0, 1 }), // right middle - 10
                new Matrix<float>(new float[] { -scale_partial, scale_half, 0, 1 }), // bottom middle - 11

                new Matrix<float>(new float[] { scale_half, -scale_partial, -scale, 1 }), // left middle - air - 12
                new Matrix<float>(new float[] { scale_more, scale_half, -scale, 1 }), // top middle - air - 13
                new Matrix<float>(new float[] { scale_half, scale_more, -scale, 1 }), // right middle - air - 14
                new Matrix<float>(new float[] { -scale_partial, scale_half, -scale, 1 }), // bottom middle - air - 15
            };

            Point[] screenPoints = worldPoints
                .Select(x => UtilityAR.WorldToScreen(x, projection)).ToArray();

            Tuple<int, int>[] lineIndexes = new[] {
                Tuple.Create(0, 8), Tuple.Create(1, 8), // ground
                Tuple.Create(1, 9), Tuple.Create(2, 9),
                Tuple.Create(2, 10), Tuple.Create(3, 10),
                Tuple.Create(3, 11), Tuple.Create(0, 11),

                Tuple.Create(4, 12), Tuple.Create(5, 12), // air
                Tuple.Create(5, 13), Tuple.Create(6, 13),
                Tuple.Create(6, 14), Tuple.Create(7, 14),
                Tuple.Create(7, 15), Tuple.Create(4, 15),

                Tuple.Create(0, 4), Tuple.Create(1, 5), // pillar
                Tuple.Create(2, 6), Tuple.Create(3, 7),
                Tuple.Create(8, 12), Tuple.Create(9, 13),
                Tuple.Create(10, 14), Tuple.Create(11, 15)
            };

            foreach (Tuple<int, int> li in lineIndexes.Take(8))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color1, 3);
            }
            foreach (Tuple<int, int> li in lineIndexes.Skip(8).Take(8))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color2, 3);
            }
            foreach (Tuple<int, int> li in lineIndexes.Skip(16).Take(8))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color3, 3);
            }
        }

        // draws a cross on the ground & the air
        public static void DrawCross(IInputOutputArray img, Matrix<float> projection, float scale = 1, bool TwoFound = false)
        {
            float scale_half = scale / 2;
            float scale_partial = (scale_half / 2) + 0.1f;
            float scale_more = scale * 1.25f;

            MCvScalar color1 = new(0, 0, 255);
            MCvScalar color2 = new(0, 255, 0);

            if (TwoFound)
            {
                color1 = new MCvScalar(0, 255, 0);
                color2 = new MCvScalar(0, 255, 0);
            }

            Matrix<float>[] worldPoints = new[]
            {
                new Matrix<float>(new float[] { 0, 0, 0, 1 }), // bottom left - floor - 0
                new Matrix<float>(new float[] { scale, 0, 0, 1 }), // top left - floor - 1
                new Matrix<float>(new float[] { scale, scale, 0, 1 }), // top right - floor - 2
                new Matrix<float>(new float[] { 0, scale, 0, 1 }), // bottom right - floor - 3

                new Matrix<float>(new float[] { 0, 0, -scale, 1 }), // bottom left - air - 4
                new Matrix<float>(new float[] { scale, 0, -scale, 1 }),// top left - air - 5
                new Matrix<float>(new float[] { scale, scale, -scale, 1 }), // top right - air - 6
                new Matrix<float>(new float[] { 0, scale, -scale, 1 }),// bottom right - air - 7
            };

            Point[] screenPoints = worldPoints
                .Select(x => UtilityAR.WorldToScreen(x, projection)).ToArray();

            Tuple<int, int>[] lineIndexes = new[] {
                Tuple.Create(0, 2),
                Tuple.Create(1, 3),
                Tuple.Create(0, 6), // air
                Tuple.Create(1, 7),
                Tuple.Create(2, 4),
                Tuple.Create(3, 5),
            };

            // Draw top
            foreach (Tuple<int, int> li in lineIndexes.Take(2))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color1, 3);
            }

            foreach (Tuple<int, int> li in lineIndexes.Skip(2).Take(4))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color2, 3);
            }

        }

        // Hold camera directly above the symbol or find both symbols to match it and see the true shape on the ground. Meme knowledge required to understand.
        public static void DrawUnique(IInputOutputArray img, Matrix<float> projection, float scale = 1, bool TwoFound = false)
        {
            float scale_half = scale / 2;
            float scale_quarter = scale_half / 2;
            float scale_barely = scale_quarter / 2;
            float scale_even_less = scale_quarter / 4;
            float pilllarOne = scale_quarter - scale_even_less;
            float pillarTwo = scale_quarter + scale_even_less;
            float pillarThree = scale_half + scale_quarter - scale_even_less;
            float pillarFour = scale_quarter + scale_even_less + scale_half;

            MCvScalar color1 = new(0, 0, 255);
            MCvScalar color2 = new(250, 0, 25);
            MCvScalar color3 = new(150, 0, 105);
            MCvScalar color4 = new(200, 100, 130);
            MCvScalar color5 = new(170, 130, 190);

            if (TwoFound)
            {
                color1 = new MCvScalar(0, 255, 0);
                color2 = new MCvScalar(0, 255, 0);
                color3 = new MCvScalar(0, 255, 0);
                color4 = new MCvScalar(0, 255, 0);
                color5 = new MCvScalar(0, 255, 0);
            }

            Matrix<float>[] worldPoints = new[]
            {
                new Matrix<float>(new float[] { scale_half, 0, 0, 1 }), // middle left - floor - 0
                new Matrix<float>(new float[] { scale_half, scale, 0, 1 }), // middle right - floor - 1

                new Matrix<float>(new float[] { 0, scale_half, 0, 1 }), // middle bottom - floor - 2
                new Matrix<float>(new float[] { scale, scale_half, 0, 1 }), // middle top - floor - 3

                new Matrix<float>(new float[] { scale_half, pilllarOne, ColorMatch(TwoFound, scale_even_less), 1 }), // left middle pillar 1 - floor - 4
                new Matrix<float>(new float[] { scale_quarter, pilllarOne, ColorMatch(TwoFound, scale_even_less), 1 }), // left lower pillar 1 - floor - 5
                new Matrix<float>(new float[] { scale_half + scale_quarter, pilllarOne, ColorMatch(TwoFound, scale_even_less), 1 }), // left upper pillar 1 - floor - 6

                new Matrix<float>(new float[] { scale_half, pillarTwo, ColorMatch(TwoFound, scale_barely), 1 }), // left middle pillar 2 - floor - 7
                new Matrix<float>(new float[] { scale_quarter, pillarTwo, ColorMatch(TwoFound, scale_barely), 1 }), // left lower pillar 2 - floor - 8

                new Matrix<float>(new float[] { scale_half, pillarThree, ColorMatch(TwoFound, scale_quarter), 1 }), // right middle pillar 1 - floor - 9
                new Matrix<float>(new float[] { scale_quarter, pillarThree, ColorMatch(TwoFound, scale_quarter), 1 }), // right lower pillar 1 - floor - 10
                new Matrix<float>(new float[] { scale_half + scale_quarter, pillarThree, ColorMatch(TwoFound, scale_quarter), 1 }), // right upper pillar 1 - floor - 11

                new Matrix<float>(new float[] { scale_quarter, (scale_barely / 2) + scale_half, ColorMatch(TwoFound, scale_half), 1 }), // right lower bar 2 - left right - floor - 12
                new Matrix<float>(new float[] { scale_quarter, scale, ColorMatch(TwoFound, scale_half), 1 }), // right lower bar - far right 2 - floor - 13

                new Matrix<float>(new float[] { scale_half, pillarFour, ColorMatch(TwoFound, scale), 1 }), // right middle pillar 2 - floor - 14
                new Matrix<float>(new float[] { scale_half + scale_barely, pillarFour, ColorMatch(TwoFound, scale), 1 }), // right upper pillar 2 - floor - 15
            };

            Point[] screenPoints = worldPoints
                .Select(x => UtilityAR.WorldToScreen(x, projection)).ToArray();

            Tuple<int, int>[] lineIndexes = new[] {
                Tuple.Create(0, 1),
                Tuple.Create(2, 3),

                Tuple.Create(5, 6),
                Tuple.Create(7, 8),

                Tuple.Create(10, 11),
                Tuple.Create(12, 13),

                Tuple.Create(14, 15),
            };

            // Draw it
            foreach (Tuple<int, int> li in lineIndexes.Take(2))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color1, 3);
            }
            foreach (Tuple<int, int> li in lineIndexes.Skip(2).Take(1))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color2, 3);
            }
            foreach (Tuple<int, int> li in lineIndexes.Skip(3).Take(1))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color3, 3);
            }
            foreach (Tuple<int, int> li in lineIndexes.Skip(4).Take(2))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color4, 3);
            }
            foreach (Tuple<int, int> li in lineIndexes.Skip(6).Take(1))
            {
                Point p1 = screenPoints[li.Item1];
                Point p2 = screenPoints[li.Item2];

                CvInvoke.Line(img, p1, p2, color5, 3);
            }
        }

        private static float ColorMatch(bool TwoFound, float BaseValue)
        {
            float result = -BaseValue;
            if (TwoFound)
            {
                result = 0;
            }
            return result;
        }
    }
}
