using Microsoft.Data.Analysis;
using System.Globalization;

namespace SNN.Common
{
    internal sealed class DataLoader
    {
        public static DataFrame LoadDataFromCSV(string filename,
                                                char sep = ',',
                                                bool includeHeader = true,
                                                Type[]? dataTypes = null,
                                                string[]? columnNames = null)
        {
            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = Path.Combine(sCurrentDirectory, @$"..\..\..\Data\{filename}");
            string sFilePath = Path.GetFullPath(sFile);

            return DataFrame.LoadCsv(sFilePath,
                                     columnNames: columnNames,
                                     cultureInfo: CultureInfo.InvariantCulture,
                                     separator: sep,
                                     header: includeHeader,
                                     dataTypes: dataTypes);
        }
    }
}
