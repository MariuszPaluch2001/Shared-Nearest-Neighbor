using SNN.SNN;
using static SNN.Common.DataLoader;
using static SNN.Common.OutWriter;
using static SNN.Common.StatWriter;
using static SNN.Common.UniqueGroups;
using static SNN.RAND.RAND;

namespace SNN
{
    public sealed class Program
    {

        static string[] knnType = new string[] { "naive", "tin" };
        static string[] numericRefPointTypes = new string[] { "min", "max" };
        static string[] nominalRefPointTypes = new string[] { "least", "most" };


        static void VoteCase()
        {
            Console.WriteLine("Vote Case");

            var dataLoadStart = DateTime.Now;
            var df = LoadDataFromCSV("vote.csv",
                includeHeader: true,
                dataTypes: Enumerable.Repeat(typeof(string), 17).ToArray());
            ;
            var dataLoadEnd = DateTime.Now;

            var labels = df["Label"].Cast<string>().ToList();
            df.Columns.Remove("Label");

            var rowsCount = (int)df.Rows.Count;
            var columnCount = df.Columns.Count;

            var snnParameters = new[] { (15, 10), (25, 15), (30, 20), (50, 25), (100, 50) };

            foreach (var param in snnParameters)
            {
                WriteStat("Vote", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                WriteStat($"Data load time: {(dataLoadEnd - dataLoadStart).TotalSeconds}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                var snn = new Snn(param.Item1, param.Item2, "Vote");
                foreach (var kType in knnType)
                {
                    if (kType == "tin")
                    {
                        foreach (var numericRefType in numericRefPointTypes)
                        {
                            WriteStat($"Type=TIN,numericRefType={numericRefType},k={param.Item1},kt={param.Item2}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);

                            var totalTimeStart = DateTime.Now;
                            var result = snn.GetSNN(df.Clone(), false, numericRefType == "min");
                            var randTimeStart = DateTime.Now;
                            var rand = Score(labels, result.ToList());
                            var randTimeEnd = DateTime.Now;
                            var totalTimeEnd = DateTime.Now;

                            var tptnTuple = CalcTPTN(labels, result);

                            WriteOut(df, labels, result, "Vote", columnCount, rowsCount, param.Item1, param.Item2, $"TIN/{numericRefType}");
                            WriteStat($"Rand time: {(randTimeEnd - randTimeStart).TotalSeconds}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"Total time: {(totalTimeEnd - totalTimeStart).TotalSeconds}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"|TP|={tptnTuple.Item1},|TN|={tptnTuple.Item2}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"Real groups count={UniqueGroupsCount(labels)}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"Predicted groups count={UniqueGroupsCount(result)}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"NPointPairs={Factorial(labels.Count)}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"Rand={rand}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                        }
                    }
                    else
                    {
                        WriteStat($"Type=NAIVE,k={param.Item1},kt={param.Item2}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);

                        var totalTimeStart = DateTime.Now;
                        var result = snn.GetSNN(df.Clone(), true);

                        var randTimeStart = DateTime.Now;
                        var rand = Score(labels, result.ToList());
                        var randTimeEnd = DateTime.Now;

                        var totalTimeEnd = DateTime.Now;

                        var tptnTuple = CalcTPTN(labels, result);

                        WriteOut(df, labels, result, "Vote", columnCount, rowsCount, param.Item1, param.Item2, "NAIVE");
                        WriteStat($"Rand time: {(randTimeEnd - randTimeStart).TotalSeconds}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Total time: {(totalTimeEnd - totalTimeStart).TotalSeconds}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"|TP|={tptnTuple.Item1},|TN|={tptnTuple.Item2}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Real groups count={UniqueGroupsCount(labels)}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Predicted groups count={UniqueGroupsCount(result)}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"NPointPairs={Factorial(labels.Count)}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"RAND={rand}", "Vote", columnCount, rowsCount, param.Item1, param.Item2);
                    }
                }
            }
        }

        static void ObesityCase()
        {
            Console.WriteLine("Obesity Case");

            var dataLoadStart = DateTime.Now;
            var df = LoadDataFromCSV("Obesity.csv",
                includeHeader: true,
                dataTypes: new Type[] {
                    typeof(string),
                                typeof(double),
                                typeof(double),
                                typeof(double),
                                typeof(string),
                                typeof(string),
                                typeof(double),
                                typeof(double),
                                typeof(string),
                                typeof(string),
                                typeof(double),
                                typeof(string),
                                typeof(double),
                                typeof(double),
                                typeof(string),
                                typeof(string),
                                typeof(string)
            });
            var dataLoadEnd = DateTime.Now;

            var labels = df["NObeyesdad"].Cast<string>().ToList();
            df.Columns.Remove("NObeyesdad");

            var rowsCount = (int)df.Rows.Count;
            var columnCount = df.Columns.Count;

            var snnParameters = new[] { (20, 10), (50, 25), (100, 50), (200, 100), (400, 200) };
            foreach (var param in snnParameters)
            {
                WriteStat("Obesity", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                WriteStat($"Data load time: {(dataLoadEnd - dataLoadStart).TotalSeconds}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);

                var snn = new Snn(param.Item1, param.Item2, "Obesity");
                foreach (var kType in knnType)
                {
                    if (kType == "tin")
                    {
                        foreach (var numericRefType in numericRefPointTypes)
                        {
                            foreach (var nominalRefType in nominalRefPointTypes)
                            {
                                WriteStat($"Type=TIN,numericRefType={numericRefType},nominalType={nominalRefType},k={param.Item1},kt={param.Item2}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);

                                var totalTimeStart = DateTime.Now;
                                var result = snn.GetSNN(df.Clone(), false, numericRefType == "min", nominalRefType == "most");

                                var randTimeStart = DateTime.Now;
                                var rand = Score(labels, result.ToList());
                                var randTimeEnd = DateTime.Now;

                                var totalTimeEnd = DateTime.Now;

                                var tptnTuple = CalcTPTN(labels, result);

                                WriteOut(df, labels, result, "Obesity", columnCount, rowsCount, param.Item1, param.Item2, $"TIN/{numericRefType}/{nominalRefType}");
                                WriteStat($"Rand time: {(randTimeEnd - randTimeStart).TotalSeconds}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                                WriteStat($"Total time: {(totalTimeEnd - totalTimeStart).TotalSeconds}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                                WriteStat($"|TP|={tptnTuple.Item1},|TN|={tptnTuple.Item2}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                                WriteStat($"Real groups count={UniqueGroupsCount(labels)}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                                WriteStat($"Predicted groups count={UniqueGroupsCount(result)}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                                WriteStat($"NPointPairs={Factorial(labels.Count)}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                                WriteStat($"Rand={rand}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                            }
                        }
                    }
                    else
                    {
                        WriteStat($"Type=NAIVE,k={param.Item1},kt={param.Item2}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);

                        var totalTimeStart = DateTime.Now;
                        var result = snn.GetSNN(df.Clone(), true);

                        var randTimeStart = DateTime.Now;
                        var rand = Score(labels, result.ToList());
                        var randTimeEnd = DateTime.Now;

                        var totalTimeEnd = DateTime.Now;


                        var tptnTuple = CalcTPTN(labels, result);

                        WriteOut(df, labels, result, "Obesity", columnCount, rowsCount, param.Item1, param.Item2, "NAIVE");
                        WriteStat($"Rand time: {(randTimeEnd - randTimeStart).TotalSeconds}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Total time: {(totalTimeEnd - totalTimeStart).TotalSeconds}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"|TP|={tptnTuple.Item1},|TN|={tptnTuple.Item2}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Real groups count={UniqueGroupsCount(labels)}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Predicted groups count={UniqueGroupsCount(result)}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"NPointPairs={Factorial(labels.Count)}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Rand={rand}", "Obesity", columnCount, rowsCount, param.Item1, param.Item2);
                    }
                }
            }
        }

        static void FrogsCase()
        {
            Console.WriteLine("Frogs Case");

            var dataLoadStart = DateTime.Now;
            var df = LoadDataFromCSV("Frogs_MFCCs.csv",
                includeHeader: true,
                dataTypes: Enumerable.Repeat(typeof(double), 22).Concat(Enumerable.Repeat(typeof(string), 4)).ToArray());
            var dataLoadEnd = DateTime.Now;

            var labels = df["Family"].Cast<string>().ToList();
            df.Columns.Remove("Family");
            df.Columns.Remove("Genus");
            df.Columns.Remove("Species");
            df.Columns.Remove("RecordID");

            var rowsCount = (int)df.Rows.Count;
            var columnCount = df.Columns.Count;

            var snnParameters = new[] { (25, 15), (100, 50), (200, 100) };
            foreach (var param in snnParameters)
            {
                WriteStat("Frogs", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                WriteStat($"Data load time: {(dataLoadEnd - dataLoadStart).TotalSeconds}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);

                var snn = new Snn(param.Item1, param.Item2, "Frogs");
                foreach (var kType in knnType)
                {
                    if (kType == "tin")
                    {
                        foreach (var numericRefType in numericRefPointTypes)
                        {
                            WriteStat($"Type=TIN,numericRefType={numericRefType},k={param.Item1},kt={param.Item2}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);

                            var totalTimeStart = DateTime.Now;
                            var result = snn.GetSNN(df.Clone(), false, numericRefType == "min");

                            var randTimeStart = DateTime.Now;
                            var rand = Score(labels, result.ToList());
                            var randTimeEnd = DateTime.Now;

                            var totalTimeEnd = DateTime.Now;

                            var tptnTuple = CalcTPTN(labels, result);

                            WriteOut(df, labels, result, "Frogs", columnCount, rowsCount, param.Item1, param.Item2, $"TIN/{numericRefType}");
                            WriteStat($"Rand time: {(randTimeEnd - randTimeStart).TotalSeconds}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"Total time: {(totalTimeEnd - totalTimeStart).TotalSeconds}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"|TP|={tptnTuple.Item1},|TN|={tptnTuple.Item2}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"Real groups count={UniqueGroupsCount(labels)}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"Predicted groups count={UniqueGroupsCount(result)}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"NPointPairs={Factorial(labels.Count)}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                            WriteStat($"Rand={rand}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                        }
                    }
                    else
                    {
                        WriteStat($"Type=NAIVE,k={param.Item1},kt={param.Item2}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);

                        var totalTimeStart = DateTime.Now;
                        var result = snn.GetSNN(df.Clone(), true);

                        var randTimeStart = DateTime.Now;
                        var rand = Score(labels, result.ToList());
                        var randTimeEnd = DateTime.Now;

                        var totalTimeEnd = DateTime.Now;

                        var tptnTuple = CalcTPTN(labels, result);

                        WriteOut(df, labels, result, "Frogs", columnCount, rowsCount, param.Item1, param.Item2, "NAIVE");
                        WriteStat($"Rand time: {(randTimeEnd - randTimeStart).TotalSeconds}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Total time: {(totalTimeEnd - totalTimeStart).TotalSeconds}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"|TP|={tptnTuple.Item1},|TN|={tptnTuple.Item2}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Real groups count={UniqueGroupsCount(labels)}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Predicted groups count={UniqueGroupsCount(result)}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"NPointPairs={Factorial(labels.Count)}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                        WriteStat($"Rand={rand}", "Frogs", columnCount, rowsCount, param.Item1, param.Item2);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            VoteCase();
            ObesityCase();
            FrogsCase();
        }
    }
}