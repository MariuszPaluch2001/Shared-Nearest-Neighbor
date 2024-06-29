using Microsoft.Data.Analysis;
using static SNN.Common.StatWriter;

namespace SNN.SNN
{
    public sealed class Snn
    {
        private int kNearest;
        private int similarityThreshold;
        private string dataset;

        public Snn(int kNearest, int similarityThreshold, string dataset = "")
        {
            this.kNearest = kNearest;
            this.similarityThreshold = similarityThreshold;
            this.dataset = dataset;
        }

        private int[] InitArray(int count) => Enumerable.Range(0, count).ToArray();

        private bool IsEachOtherNeighborhood(int[] arr1, int[] arr2) => arr1.Contains(arr2[0]) && arr2.Contains(arr1[0]);

        private bool IsThresholdPassed(int[] arr1, int[] arr2) => arr1.Intersect(arr2).Count() > similarityThreshold;

        private void UpdateLabelTable(int[] labelsList, int oldLabel, int newLabel)
        {
            foreach (var (value, idx) in labelsList.Select((value, idx) => (value, idx)))
            {
                if (value == oldLabel)
                    labelsList[idx] = newLabel;
            }
        }

        private int[][] ConvertKnnDictToNeighborhoodTable(Dictionary<int, List<int>> dict)
        {
            var result = new int[dict.Count][];

            foreach (var (entry, idx) in dict.Select((entry, idx) => (entry, idx)))
            {
                entry.Value.Insert(0, entry.Key);
                result[idx] = entry.Value.ToArray();
            }

            return result;
        }

        public int[] GetSNN(DataFrame data, bool isKNNNaive = true, bool isNumericMin = true, bool isNominalMostPopular = true)
        {
            var knn = new Knn(kNearest, dataset);

            var knnStart = DateTime.Now;
            var kNearestDict = isKNNNaive ? knn.FindNearestNeighborsNaive(data.Clone()) : knn.FindNearestNeighborsWithTE(data.Clone(), isNumericMin, isNominalMostPopular);
            kNearestDict = kNearestDict.OrderBy(x => x.Key).ToDictionary(t => t.Key, t => t.Value);
            var knnEnd = DateTime.Now;

            WriteStat($"KNN time: {(knnEnd - knnStart).TotalSeconds}", dataset, data.Columns.Count, (int)data.Rows.Count, kNearest, similarityThreshold);

            var snnGroupingStart = DateTime.Now;

            var neighborhoodTable = ConvertKnnDictToNeighborhoodTable(kNearestDict);

            var labelsList = InitArray((int)data.Rows.Count);

            foreach (var (current_label, current_idx) in labelsList.Select((current_label, current_idx) => (current_label, current_idx)))
            {
                var arr1 = neighborhoodTable[current_idx];
                foreach (var (tested_label, tested_idx) in labelsList.Select((v, i) => (v, i)).Where(x => x.v != current_label))
                {
                    var arr2 = neighborhoodTable[tested_idx];
                    if (IsEachOtherNeighborhood(arr1, arr2) && IsThresholdPassed(arr1, arr2))
                    {
                        var oldLabel = current_label > tested_label ? current_label : tested_label;
                        var newLabel = current_label < tested_label ? current_label : tested_label;

                        UpdateLabelTable(labelsList, oldLabel, newLabel);
                        break;
                    }
                }
            }
            var snnGroupingEnd = DateTime.Now;

            WriteStat($"SNN time: {(snnGroupingEnd - snnGroupingStart).TotalSeconds}", dataset, data.Columns.Count, (int)data.Rows.Count, kNearest, similarityThreshold);

            return labelsList;
        }
    }
}
