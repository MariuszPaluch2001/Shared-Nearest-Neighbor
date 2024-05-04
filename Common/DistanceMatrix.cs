namespace SNN.Common
{
    public class DistanceMatrix
    {
        private long shape;

        public long Shape
        {
            get { return shape; }
            private set { shape = value; }
        }

        private readonly double[,] _array;

        public DistanceMatrix(long shape)
        {
            this.shape = shape;
            _array = new double[this.shape, this.shape];
        }

        public DistanceMatrix(double[,] array)
        {
            this.shape = array.Length;
            _array = array;
        }

        public double this[int i, int j]
        {
            get
            {
                return _array[i, j];
            }

            set
            {
                _array[i, j] = value;
            }
        }
    }
}
