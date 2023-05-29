using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace ARExercise
{
    public class MarkerAR : FrameLoop
    {
        private Mat frame = new();
        private readonly Mat changedImage = new();
        private readonly VectorOfVectorOfPoint contours = new();
        private readonly List<Tuple<string, List<Tuple<int, Matrix<Byte>>>>> KnownMarkers;
        private readonly Matrix<float>? intrinsics;
        private readonly Matrix<float>? distcoeffs;
        private VectorOfPoint apCurve = new();
        private readonly List<VectorOfPoint> contourSave = new();
        private readonly VectorOfPoint dstpoints = new(test);
        private static readonly Point[] test = new Point[4] { new Point(0, 0), new Point(300, 0), new Point(300, 300), new Point(0, 300) };
        private Mat homography = new();

        public MarkerAR()
        {
            KnownMarkers = Markers.AllMarkers;
            UtilityAR.ReadIntrinsicsFromFile(out intrinsics, out distcoeffs);
        }

        //VideoCapture vidCap = new VideoCapture(1);
        public override void OnFrame()
        {
            //UtilityAR.CaptureLoop();
            frame = CvInvoke.Imread("capture_1.jpg");
            //vidCap.Read(frame);

            CvInvoke.CvtColor(frame, changedImage, ColorConversion.Bgr2Gray);
            CvInvoke.Threshold(changedImage, changedImage, 0, 255, ThresholdType.Otsu);
            CvInvoke.FindContours(changedImage, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

            List<Tuple<Mat, VectorOfPoint, Matrix<Byte>>> DataFromContours = new();

            MCvScalar scalar = new(255, 0, 0);
            CvInvoke.DrawContours(frame, contours, -1, scalar);
            contourSave.Clear();
            int contourNumber = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                apCurve = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(contours[i], apCurve, 4, true);
                if (apCurve.Size == 4)
                {
                    contourSave.Add(apCurve);
                    contourNumber++;
                }
            }

            List<Tuple<Mat, VectorOfPoint, Tuple<string, int>>> ValidMarkers = new();

            for (int i = 0; i < contourSave.Count; i++)
            {
                homography = CvInvoke.FindHomography(contourSave[i], dstpoints, RobustEstimationAlgorithm.Ransac);
                CvInvoke.WarpPerspective(frame, changedImage, homography, new Size(300, 300));

                #region task 5
                Mat tempFirstImage = new();
                CvInvoke.CvtColor(changedImage, tempFirstImage, ColorConversion.Bgr2Gray);
                CvInvoke.Threshold(tempFirstImage, tempFirstImage, 0, 255, ThresholdType.Otsu);

                float length = tempFirstImage.Size.Width / 6;
                Matrix<byte> array = new(4, 4);

                for (int y = 1; y < 5; y++)
                {
                    for (int x = 1; x < 5; x++)
                    {
                        float X = (length * x) + (length / 2f);
                        float Y = (length * y) + (length / 2f);

                        var data = tempFirstImage.GetRawData(new[] { (int)X, (int)Y });

                        array[x - 1, y - 1] = data[0];
                    }
                }

                #endregion
                DataFromContours.Add(new Tuple<Mat, VectorOfPoint, Matrix<Byte>>(changedImage, contourSave[i], array));


                bool valid = false;

                foreach (var MarkerGroup in KnownMarkers)
                {
                    if (valid)
                        break;
                    foreach (var Marker in MarkerGroup.Item2)
                    {
                        if (Markers.CompareMarkers(Marker.Item2, array))
                        {
                            ValidMarkers.Add(new Tuple<Mat, VectorOfPoint, Tuple<string, int>>(changedImage, contourSave[i], new Tuple<string, int>(MarkerGroup.Item1, Marker.Item1)));
                            valid = true;
                            break;
                        }
                    }
                }
            }

            var ValidContours = new VectorOfVectorOfPoint();
            foreach (var ValidMarker in ValidMarkers)
            {
                VectorOfPoint contour = ValidMarker.Item2;
                float rotation = ValidMarker.Item3.Item2;

                var projection = CalulateProjection(frame, contour, rotation, 2); // last number = size of coordinate of marker (affects cube size in drawcube)
                string markertype = ValidMarker.Item3.Item1;
                List<string> SetOne = new() { "marker1", "marker2" };
                List<string> SetTwo = new() { "marker3", "marker4" };
                List<string> SetThree = new() { "marker5", "marker6" };
                List<string> SetFour = new() { "marker7", "marker8" };

                if (Helpers.CompareStringList(markertype, SetOne))
                {
                    bool found = Helpers.ListContentFound(SetOne, ValidMarkers);
                    DrawSpecial.DrawCircle(frame, projection, TwoFound: found);
                }
                else if (Helpers.CompareStringList(markertype, SetTwo))
                {
                    bool found = Helpers.ListContentFound(SetTwo, ValidMarkers);
                    DrawSpecial.DrawBoxInvert(frame, projection, TwoFound: found);
                }
                else if (Helpers.CompareStringList(markertype, SetThree))
                {
                    bool found = Helpers.ListContentFound(SetThree, ValidMarkers);
                    DrawSpecial.DrawCross(frame, projection, TwoFound: found);
                }
                else if (Helpers.CompareStringList(markertype, SetFour))
                {
                    bool found = Helpers.ListContentFound(SetFour, ValidMarkers);
                    DrawSpecial.DrawUnique(frame, projection, TwoFound: found);
                }
                else
                {
                    DrawSpecial.DrawCube(frame, projection);
                }
                ValidContours.Push(contour);
            }

            CvInvoke.Imshow($"image", frame);
        }

        private Matrix<float> GetRtMatrix(Matrix<float> RotMatrix, Matrix<float> TranMatrix)
        {
            float[,] RotArray = RotMatrix.Data;
            float[,] transarray = TranMatrix.Data;
            var RtMat = new Matrix<float>(3, 4);

            for (int i = 0; i < 3; i++)
            {
                RtMat[i, 0] = RotArray[i, 0];
                RtMat[i, 1] = RotArray[i, 1];
                RtMat[i, 2] = RotArray[i, 2];
                RtMat[i, 3] = transarray[i, 0];
            }
            return RtMat;
        }

        private Matrix<float> CalulateProjection(Mat frame, VectorOfPoint corners, float rotation, int size)
        {
            Matrix<float> rotationVector = new(3, 1);
            Matrix<float> TranslationVector = new(3, 1);

            MCvPoint3D32f[]? objectpoints = null;

            if (rotation == 0)
            {
                objectpoints = new MCvPoint3D32f[]
                {

                    new MCvPoint3D32f(size,0,0),
                    new MCvPoint3D32f(size,size,0),
                    new MCvPoint3D32f(0,size,0),
                     new MCvPoint3D32f(0,0,0),

                };
            }
            else if (rotation == 90)
            {
                objectpoints = new MCvPoint3D32f[]
                {


                    new MCvPoint3D32f(size,size,0),
                    new MCvPoint3D32f(0,size,0),
                     new MCvPoint3D32f(0,0,0),
                     new MCvPoint3D32f(size,0,0),


                };
            }
            else if (rotation == 180)
            {
                objectpoints = new MCvPoint3D32f[]
                {



                    new MCvPoint3D32f(0,size,0),
                     new MCvPoint3D32f(0,0,0),
                     new MCvPoint3D32f(size,0,0),
                     new MCvPoint3D32f(size,size,0),

                };
            }
            else if (rotation == 270)
            {
                objectpoints = new MCvPoint3D32f[]
                {
                    new MCvPoint3D32f(0,0,0),
                    new MCvPoint3D32f(size,0,0),
                    new MCvPoint3D32f(size,size,0),
                    new MCvPoint3D32f(0,size,0)

                };
            }

            var con = corners.ToArray();

            PointF[] conf = new PointF[4];
            int i = 0;
            foreach (Point p in con)
            {

                conf[i] = new PointF(p.X, p.Y);

                i++;
            }

            if (con.Length > 0)
            {
                CvInvoke.SolvePnP(objectpoints, conf, intrinsics, distcoeffs, rotationVector, TranslationVector);
                Matrix<float> RotMatrix = new(3, 3);
                CvInvoke.Rodrigues(rotationVector, RotMatrix);


                var RtMat = GetRtMatrix(RotMatrix, TranslationVector);


                return intrinsics * RtMat;

            }
            else
            {
                // Because the null warning in the editor is not majorly important in this case.
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }

        }

    }
}
