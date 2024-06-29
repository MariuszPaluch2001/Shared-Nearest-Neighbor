using Microsoft.Data.Analysis;
using static System.Convert;
using static System.Double;
using static System.Type;
namespace SNN.DistanceMetric
{
    public sealed class GowerDistance
    {
        private static double DistanceForNumerical(double value1, double value2, double range)
        {
            return range == 0.0 ? MinValue : Abs(value1 - value2) / range;
        }

        private static double DistanceForNominalAtributes(string value1, string value2)
        {
            return value1.Equals(value2) ? 0.0 : 1.0;
        }

        public static Dictionary<int, double> GetRanges(DataFrame df)
        {
            var ranges = new Dictionary<int, double>();
            foreach (var (col, col_idx) in df.Columns.Select((col, col_idx) => (col, col_idx)))
            {
                if (GetTypeCode(col.DataType) == TypeCode.Double || GetTypeCode(col.DataType) == TypeCode.Int32)
                    ranges.Add(col_idx, ToDouble(col.Abs().Max()));
            }

            return ranges;
        }

        public static double[] CalcDistancesForPoint(DataFrame df, object[] point, Dictionary<int, double> ranges)
        {
            var distanceRow = new double[df.Rows.Count];

            foreach (var (row, rowIdx) in df.Rows.Select((row, rowIdx) => (row, rowIdx)))
                distanceRow[rowIdx] = CalcDistanceBetweenTwoPoints(row.ToArray(), point, ranges);

            return distanceRow;
        }

        public static float CalcDistanceBetweenTwoPoints(object[] point1, object[] point2, Dictionary<int, double> ranges)
        {
            var distance = 0.0;

            foreach (var (point1_value, idx) in point1.Select((point1_value, idx) => (point1_value, idx)))
            {
                var point2_value = point2[idx];
                switch (GetTypeCode(point1_value.GetType()))
                {
                    case TypeCode.Double:
                    case TypeCode.Int32:
                        distance += DistanceForNumerical(ToDouble(point1_value), ToDouble(point2_value), ranges[idx]);
                        break;
                    case TypeCode.String:
                        distance += DistanceForNominalAtributes(Convert.ToString(point1_value)!, Convert.ToString(point2_value)!);
                        break;
                    default:
                        break;
                }
            }
            return (float)distance / point1.Length;
        }
    }
}
