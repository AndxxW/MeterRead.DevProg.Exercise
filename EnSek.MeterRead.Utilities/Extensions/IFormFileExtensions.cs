using Microsoft.AspNetCore.Http;
using System.Text;

namespace EnSek.MeterRead.Utilities.Extensions
{
    /// <summary>
    /// IFormFile extension methods
    /// </summary>
    public static class IFormFileExtensions
    {
        /// <summary>
        /// Converts an uploaded csv File to a string
        /// </summary>
        /// <param name="source">The source file</param>
        /// <returns>String containing its contents</returns>
        public static string? ConvertCsvFileToString(this IFormFile source)
        {
            if (source?.ContentType == "text/csv")
            {
                using (var stream = new MemoryStream())
                {
                    source.CopyTo(stream);

                    //Run it through a second memory steam to avoid issues with BOM chars 
                    using (var memoryStream = new MemoryStream(stream.ToArray()))
                    {
                        return new StreamReader(memoryStream).ReadToEnd();
                    }
                }
            }

            return null;
        }
    }
}
