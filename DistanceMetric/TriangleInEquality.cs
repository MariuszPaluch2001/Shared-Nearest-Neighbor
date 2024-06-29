using Microsoft.Data.Analysis;
using static SNN.DistanceMetric.GowerDistance;
using static System.Type;
namespace SNN.DistanceMetric
{
    public sealed class TriangleInEquality
    {
        public static object[] ReferencePoint(DataFrame df, bool isNumericMin = true, bool isNominalMostPopular = true)
        {
            var referencePoint = new object[df.Columns.Count];
            foreach (var (col, i) in df.Columns.Select((value, i) => (value, i)))
            {
                switch (GetTypeCode(col.DataType))
                {
                    case TypeCode.Double:
                    case TypeCode.Int32:
                        referencePoint[i] = isNumericMin ? col.Min() : col.Max();
                        break;
                    case TypeCode.String:
                        referencePoint[i] = isNominalMostPopular ? col.ValueCounts()["Values"].Sort()[0] : col.ValueCounts()["Values"].Sort(ascending: false)[0];
                        break;
                    default:
                        break;
                }
            }
            return referencePoint;
        }

        public static double[] DistancesToReferencePoint(DataFrame df, object[] referencePoint, Dictionary<int, double> ranges)
        {
            return CalcDistancesForPoint(df, referencePoint, ranges);
        }

    }
}
