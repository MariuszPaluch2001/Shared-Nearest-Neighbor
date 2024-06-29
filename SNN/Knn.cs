using Microsoft.Data.Analysis;
using System.Data;
using static SNN.Common.KnnWriter;
using static SNN.DistanceMetric.GowerDistance;
using static SNN.DistanceMetric.TriangleInEquality;
using static System.Double;

namespace SNN.SNN
{
    public sealed class Knn
    {

        private int k_nearest;
        private string dataset;
        public Knn(int k, string dataset = "")
        {
            k_nearest = k;
            this.dataset = dataset;
        }

        public Dictionary<int, List<int>> FindNearestNeighborsNaive(DataFrame df)
        {
            var ranges = GetRanges(df);
            var result = new Dictionary<int, List<int>>();

            foreach (var (row, idx) in df.Rows.Select((row, index) => (row, index)))
            {
                var distances = CalcDistancesForPoint(df, row.ToArray(), ranges);

                result[idx] = distances
                    .Select((value, i) => new KeyValuePair<int, double>(i, value))
                    .OrderBy(x => x.Value)
                    .ThenBy(x => x.Key)
                    .Take(k_nearest + 1)
                    .Select(x => x.Key)
                    .ToList();

                result[idx] = result[idx].Where(x => x != idx).ToList();
                WriteKNN(idx, distances[result[idx].Last()], distances.Max(), result[idx].ToArray(), dataset, df.Columns.Count, (int)df.Rows.Count, k_nearest, 0, "Naive");
            }
            return result;
        }

        public Dictionary<int, List<int>> FindNearestNeighborsWithTE(DataFrame df, bool isNumericMin = true, bool isNominalMostPopular = true)
        {
            var ranges = GetRanges(df);
            var referencePoint = ReferencePoint(df, isNumericMin, isNominalMostPopular);
            var refDistances = DistancesToReferencePoint(df, referencePoint, ranges);
            df.Columns.Add(new Int32DataFrameColumn("index", Enumerable.Range(0, (int)df.Rows.Count).ToArray()));
            df.Columns.Add(new DoubleDataFrameColumn("refDist", refDistances));

            df = df.OrderBy("refDist");
            var indexes = df.Rows.Select(x => (int)x["index"]).ToArray();
            df.Columns.Remove("refDist");
            df.Columns.Remove("index");

            var result = new Dictionary<int, List<int>>();
            foreach (var (row, idx) in df.Rows.Select((row, index) => (row, index)))
            {
                var realDistances = new Dictionary<int, double>();
                int offset = 1;
                var lowerBoundChecking = true;
                var upperBoundChecking = true;
                double eps = MaxValue;
                double maxEps = -1.0;
                while (lowerBoundChecking || upperBoundChecking)
                {
                    if (realDistances.Count < k_nearest)
                    {
                        if (offset <= idx)
                        {
                            var dist = CalcDistanceBetweenTwoPoints(row.ToArray(), df.Rows[idx - offset].ToArray(), ranges);
                            realDistances.Add(indexes[idx - offset], dist);
                        }
                        if (idx + offset < df.Rows.Count)
                        {
                            var dist = CalcDistanceBetweenTwoPoints(row.ToArray(), df.Rows[idx + offset].ToArray(), ranges);
                            realDistances.Add(indexes[idx + offset], dist);
                        }
                    }
                    else
                    {
                        eps = realDistances.OrderBy(x => x.Value).ElementAt(k_nearest - 1).Value;
                        maxEps = maxEps < 0 ? eps : maxEps;
                        if (lowerBoundChecking && offset <= idx && refDistances[idx] - refDistances[idx - offset] < eps)
                        {
                            var dist = CalcDistanceBetweenTwoPoints(row.ToArray(), df.Rows[idx - offset].ToArray(), ranges);
                            realDistances.Add(indexes[idx - offset], dist);
                        }
                        else
                            lowerBoundChecking = false;

                        eps = realDistances.OrderBy(x => x.Value).ElementAt(k_nearest - 1).Value;
                        if (upperBoundChecking && idx + offset < df.Rows.Count && refDistances[idx + offset] - refDistances[idx] < eps)
                        {
                            var dist = CalcDistanceBetweenTwoPoints(row.ToArray(), df.Rows[idx + offset].ToArray(), ranges);
                            realDistances.Add(indexes[idx + offset], dist);
                        }
                        else
                            upperBoundChecking = false;
                    }
                    offset++;
                }
                var kNearest = realDistances.OrderBy(x => x.Value).ThenBy(x => x.Key).Take(k_nearest).Select(x => x.Key).ToList();
                result.Add(indexes[idx], kNearest);

                WriteKNN(idx, realDistances[kNearest.Last()], maxEps, kNearest.ToArray(), dataset, df.Columns.Count, (int)df.Rows.Count, k_nearest, 0, "TE");
            }
            return result;
        }
    }
}
