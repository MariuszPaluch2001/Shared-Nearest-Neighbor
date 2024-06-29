using Microsoft.Data.Analysis;
using static SNN.Common.WriteToFile;

namespace SNN.Common
{
    public class OutWriter
    {
        public static void WriteOut<T>(DataFrame origin, IList<T> labels, int[] result, string dataset, int dimension, int rowsCount, int k, int thr, string experimentType)
        {
            Write(experimentType, "Out", dataset, dimension, rowsCount, k, thr);
            for (int i = 0; i < result.Length; i++)
            {
                var strPoint = "";
                foreach (var r in origin.Rows[i])
                {
                    strPoint += r.ToString() + ',';
                }
                Write($"{i},{strPoint}{labels[i]},{result[i]}", "Out", dataset, dimension, rowsCount, k, thr);
            }
        }
    }
}
