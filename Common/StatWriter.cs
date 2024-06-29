using static SNN.Common.WriteToFile;

namespace SNN.Common
{
    public class StatWriter
    {
        public static void WriteStat(string text, string dataset, int dimension, int rowsCount, int k, int thr)
        {
            Write(text, "Stat", dataset, dimension, rowsCount, k, thr);
        }
    }
}
