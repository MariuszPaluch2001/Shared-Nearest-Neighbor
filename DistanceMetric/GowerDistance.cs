using Microsoft.Data.Analysis;
using SNN.Common;

namespace SNN.DistanceMetric
{
    public sealed class GowerDistance
    {
        private static double ManhatanDistance(double value1, double value2)
        {
            return 1.0 - Math.Abs(value1 - value2) / Math.Max(value1, value2);
        }

        private static double DistanceForNominalAtributes(string value1, string value2)
        {
            return value1.Equals(value2) ? 1.0 : 0.0;
        }

        private static double DistanceForBinaryAtributes(bool value1, bool value2)
        {
            return value1 && value2 ? 1.0 : 0.0;
        }

        public static DistanceMatrix CalcDistance(DataFrame df)
        {
            var matrix = new DistanceMatrix(df.Rows.Count);
            foreach (var indiv1 in df.Rows.Select((row, index) => (row, index)))
            {
                foreach (var indiv2 in df.Rows.Select((row, index) => (row, index)))
                {
                    var dist = 0.0;
                    var sum_weights = 0;
                    for (int c = 0; c < df.Columns.Count; c++)
                    {
                        var val1 = indiv1.row[c];
                        var val2 = indiv2.row[c];
                        if (val1 != null && val2 != null)
                        {
                            switch (Type.GetTypeCode(df.Columns[c].DataType))
                            {
                                case TypeCode.Double:
                                case TypeCode.Int32:
                                    dist += ManhatanDistance(Convert.ToDouble(val1), Convert.ToDouble(val2));
                                    sum_weights++;
                                    break;
                                case TypeCode.String:
                                    dist += DistanceForNominalAtributes(Convert.ToString(val1)!, Convert.ToString(val2)!);
                                    sum_weights++;
                                    break;
                                case TypeCode.Boolean:
                                    if ((bool)val1 || (bool)val2)
                                    {
                                        dist += DistanceForBinaryAtributes(Convert.ToBoolean(val1), Convert.ToBoolean(val2));
                                        sum_weights++;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    matrix[indiv1.index, indiv2.index] = 1.0 - dist / sum_weights;
                }
            }
            return matrix;
        }
    }
}
