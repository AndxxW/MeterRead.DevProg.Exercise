using System.Text;
using EnSek.MeterRead.Utilities.Classes;
using Newtonsoft.Json;

namespace EnSek.MeterRead.Utilities.Helpers
{
    /// <summary>
    /// Helper for processing csv data
    /// </summary>
    public static class CsvHelper
    {
        /// <summary>
        /// Converts csv data to an enumeration of Json strings
        /// </summary>
        /// <param name="csvData">The csv data text</param>
        /// <param name="settings">Settings for the conversion</param>
        /// <returns>Enumeration of json strings</returns>
        public static IEnumerable<string> ConvertCsvToJsonStrings(string csvData, CsvSettings? settings = null)
        {
            var jsonList = new List<string>();

            if (string.IsNullOrWhiteSpace(csvData))
            {
                return jsonList;
            }

            if (settings == null)
            {
                settings = new CsvSettings();
            }

            var rows = csvData.Split(settings.NewLineDelimiter).ToList();

            var headerNames = rows.First()
                .Split(settings.ColumnDelimiter)
                .Select(x => x.Trim())
                .Where(y => !string.IsNullOrWhiteSpace(y)).ToArray();

            rows.RemoveAt(0);

            var rowValues = 
                rows.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(y => y.Split(settings.ColumnDelimiter));

            foreach (var rowValue in rowValues)
            {
                var jsonBuilder = new StringBuilder("{");

                for (int i = 0; i < headerNames.Length; i++)
                {
                    var field = headerNames[i];

                    var value = rowValue.Length > i ? rowValue[i].Trim() : string.Empty;

                    jsonBuilder.Append($"\"{field}\":\"{value}\",");
                }

                jsonBuilder.Append('}');

                jsonList.Add(jsonBuilder.ToString());
            }

            return jsonList;
        }

        /// <summary>
        /// Converts csv data to an enumeration of class instances
        /// </summary>
        /// <typeparam name="T">The class type of the resultant items</typeparam>
        /// <param name="csvData">The csv data</param>
        /// <param name="settings">Settings for the conversion</param>
        /// <returns>A summary of the conversion process, indicating success and failure counts as well as the enumeration of data instances</returns>
        public static CsvConversionResult<T> ConvertCsvTo<T>(string csvData, CsvSettings? settings = null)
            where T : class
        {
            var jsonStrings = ConvertCsvToJsonStrings(csvData);

            JsonSerializerSettings? jsonSettings = null;

            if (!string.IsNullOrWhiteSpace(settings?.DateFormat))
            {
                jsonSettings = new JsonSerializerSettings
                {
                    //Allow a non-standard date format to be specified
                    DateFormatString = settings.DateFormat
                };
            }

            var instances = JsonHelper.ConvertJsonStringsTo<T>(jsonStrings, jsonSettings);

            //Allow additional validation to applied during the conversion
            if (settings?.ValidateItem != null)
            {
                instances = instances.Where(x => settings.ValidateItem(x));
            }

            var result = new CsvConversionResult<T>
            {
                CsvRowCount = jsonStrings.Count(),
                SuccessCount = instances.Count(),
                FailCount = jsonStrings.Count() - instances.Count(),
                Instances = instances
            };

            return result;
        }
    }
}
