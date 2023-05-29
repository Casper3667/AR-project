using Emgu.CV;

namespace ARExercise
{
    public abstract class FrameLoop
    {
        public void Run()
        {
            while (true)
            {
                OnFrame();
                CvInvoke.WaitKey(1);
            }
        }

        public abstract void OnFrame();
    }
}
