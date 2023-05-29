using Emgu.CV;
using Emgu.CV.Util;

namespace ARExercise
{
    public static class Helpers
    {
        public static bool CompareStringList(string keyword, List<string> list, bool caseSensitive = false)
        {
            bool value = false;
            foreach (string usedword in list)
            {
                if ((!caseSensitive && keyword.ToLower() == usedword.ToLower()) || (caseSensitive && keyword == usedword))
                {
                    value = true;
                    break;
                }
            }
            return value;
        }

        public static bool CompareTwoStringList(List<string> keywordList, List<string> list, bool caseSensitive = false)
        {
            bool value = false;
            foreach (string keyword in keywordList)
            {
                foreach (string usedword in list)
                {
                    if ((!caseSensitive && keyword.ToLower() == usedword.ToLower()) || (caseSensitive && keyword == usedword))
                    {
                        value = true;
                        break;
                    }
                }
                if (value == true)
                    break;
            }
            return value;
        }

        public static bool ListContentFound(List<string> keywordList, List<Tuple<Mat, VectorOfPoint, Tuple<string, int>>> ValidMarkers)
        {
            int countGoal = keywordList.Count();
            int countCurrent = 0;
            foreach (var word in keywordList)
            {
                foreach (var ValidMarker in ValidMarkers)
                {
                    string markertype = ValidMarker.Item3.Item1;
                    if (word.ToLower() == markertype.ToLower())
                    {
                        countCurrent++;
                        break;
                    }
                }
            }
            if (countCurrent >= countGoal)
                return true;
            else
                return false;
        }
    }
}
