namespace SNN.Common
{
    public class WriteToFile
    {
        public static void Write(string text, string type, string dataset, int dimension, int rowsCount, int k, int thr)
        {
            var fileName = $"{type}_SNN_{dataset}_D{dimension}_R{rowsCount}_k{k}_kt{thr}.txt";
            using var writetext = new StreamWriter(fileName, true);
            writetext.WriteLine(text);
        }
    }
}
