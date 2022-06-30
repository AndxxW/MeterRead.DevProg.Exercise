using Newtonsoft.Json;

namespace EnSek.MeterRead.Utilities.Helpers
{
    /// <summary>
    /// Helper for processing Json data.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Converts from a Json string to an instance of T
        /// </summary>
        /// <typeparam name="T">Class type</typeparam>
        /// <param name="json">Json string</param>
        /// <param name="settings">Json settings</param>
        /// <returns>Instance of T</returns>
        public static T FromJson<T>(string json, JsonSerializerSettings? settings = null)
            where T : class
        {
            var isSuccess = true;

            if (settings == null)
            {
                settings = new JsonSerializerSettings();
            }

            var errorHandler = settings.Error;

            settings.Error = (sender, args) =>
            {
                //Failed deserialisations will be returned as nulls
                isSuccess = false;
                args.ErrorContext.Handled = true;

                errorHandler?.Invoke(sender, args);
            };

            var instance = JsonConvert.DeserializeObject<T>(json, settings);

            return isSuccess ? instance : default;
        }

        /// <summary>
        /// Coverts an enumerable of Json strings to an instance of T.
        /// </summary>
        /// <typeparam name="T">Class type</typeparam>
        /// <param name="jsonStrings">Enumeration of Json strings</param>
        /// <param name="settings">Json serializer settings</param>
        /// <returns>Enumeration of T instances</returns>
        public static IEnumerable<T> ConvertJsonStringsTo<T>(IEnumerable<string> jsonStrings, JsonSerializerSettings? settings = null)
            where T : class
        {
            var instances = new List<T>();

            if (jsonStrings == null)
            {
                return instances;
            }

            foreach (var json in jsonStrings)
            {
                if (FromJson<T>(json, settings) is { } instance)
                {
                    instances.Add(instance);
                }
            }

            return instances;
        }

    }
}
