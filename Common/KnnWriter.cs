using static SNN.Common.WriteToFile;

namespace SNN.Common
{
    public class KnnWriter
    {
        public static void WriteKNN(int id, double eps, double maxEps, int[] kNearest, string dataset, int dimension, int rowsCount, int k, int thr, string experimentType)
        {
            var strKNearest = "";
            foreach (var r in kNearest)
            {
                strKNearest += r.ToString() + ',';
            }
            Write($"{id},{eps},{maxEps},{strKNearest}", $"KNN_{experimentType}", dataset, dimension, rowsCount, k, thr);
        }
    }
}
