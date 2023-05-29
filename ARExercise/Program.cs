namespace ARExercise
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //UtilityAR.CaptureLoop(new Size(7, 4), 1);
            //UtilityAR.CalibrateCamera(new Size(7, 4), false);

            //ChessboardAR board = new ChessboardAR();

            MarkerAR board = new();

            board.Run();
        }
    }
}