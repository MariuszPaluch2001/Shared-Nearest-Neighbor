namespace SNN
{
    internal class DistanceMatrix
    {
        private int m, n;


        private int[,] array;
        public DistanceMatrix(int m, int n)
        {
            //assign the array inside the constructor
            array = new int[m, n];
        }

        public int GetValue(int m, int n) => array[m, n];

        public void SetValue(int m, int n) => array[m, n] = value;
    }
}
