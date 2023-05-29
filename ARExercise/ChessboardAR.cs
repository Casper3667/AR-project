using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace ARExercise
{
    public class ChessboardAR : FrameLoop
    {
        private readonly VideoCapture vidCap = new(1);
        private readonly Mat frame = new();
        private Matrix<float>? intrinsics;
        private Matrix<float>? distcoeffs;
        private readonly VectorOfPointF corners = new();
        private bool work = false;
        private readonly Matrix<float> rotations = new(3, 1);
        private readonly Matrix<float> translations = new(3, 1);
        private readonly Matrix<float> rtMatrix = new(new float[]
        {
        });

        public override void OnFrame()
        {
            vidCap.Read(frame);


            UtilityAR.ReadIntrinsicsFromFile(out intrinsics, out distcoeffs);

            work = CvInvoke.FindChessboardCorners(frame, new Size(7, 4), corners);
            var projection = CornerFind(frame, work);

            if (projection != null)
            {
                UtilityAR.DrawCube(frame, projection); ;
            }

            CvInvoke.Imshow("test", frame);

        }

        private Matrix<float>? CornerFind(Mat frame, bool work)
        {
            MCvPoint3D32f[] d32f = UtilityAR.GenerateObjectPointsForChessboard(new Size(7, 4));

            if (work)
            {
                CvInvoke.SolvePnP(d32f, corners.ToArray(), intrinsics, distcoeffs, rotations, translations);
                Matrix<float> RotMatrix = new(3, 3);
                CvInvoke.Rodrigues(rotations, RotMatrix);

                var RtMat = GetRtMatrix(RotMatrix, translations);
                return intrinsics * RtMat;
            }
            else
                return null;

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

    }
}
