namespace SNN.RAND
{
    public sealed class RAND
    {
        public static int Factorial(int n)
        {
            return n * (n - 1) / 2;
        }

        public static Tuple<int, int> CalcTPTN<T>(IList<T> trueLabels, IList<int> predictedLabels)
        {
            var sameElements = 0;
            var diffElements = 0;

            var n = trueLabels.Count;

            for (int i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    if (trueLabels[i].Equals(trueLabels[j]) && predictedLabels[i] == predictedLabels[j])
                        sameElements++;
                    else if (!trueLabels[i].Equals(trueLabels[j]) && predictedLabels[i] != predictedLabels[j])
                        diffElements++;
                }
            }

            return new Tuple<int, int>(sameElements, diffElements);
        }

        public static double Score<T>(IList<T> trueLabels, IList<int> predictedLabels)
        {
            var tuple = CalcTPTN(trueLabels, predictedLabels);
            return (tuple.Item1 + tuple.Item2) / (double)Factorial(trueLabels.Count);
        }
    }
}
