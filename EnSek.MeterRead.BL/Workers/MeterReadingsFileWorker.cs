using EnSek.MeterRead.DAL.Interfaces;
using EnSek.MeterRead.Models.Dtos;
using EnSek.MeterRead.Models.Entities;
using EnSek.MeterRead.Utilities.Classes;
using EnSek.MeterRead.Utilities.Extensions;
using EnSek.MeterRead.Utilities.Helpers;
using Microsoft.AspNetCore.Http;

namespace EnSek.MeterRead.BL.Workers
{
    /// <summary>
    /// Worker class to allow processing of a csv file which has been uploaded to the server
    /// </summary>
    public static class MeterReadingsFileWorker
    {
        //TODO - This could be changed to a class instance with the IMeterRepo injected
        public static async Task<MeterReadingsUploadsSummaryDto> ProcessCsvFile(IFormFile csvFile, IMeterRepo meterRepo)
        {
            var uploadResult = new MeterReadingsUploadsSummaryDto();

            if (csvFile is { Length: > 0 })
            {
                var csvContent = csvFile.ConvertCsvFileToString();

                if (!string.IsNullOrWhiteSpace(csvContent))
                {
                    var settings = new CsvSettings()
                    {
                        //TODO: Avoid hard-coding the date format
                        DateFormat = "dd/MM/yyyy HH:mm",
                        //TODO: Cater for the NNNNN format fully, rather than just checking an integer range
                        ValidateItem = x => (x as MeterReading)?.MeterReadValue is >= 0 and <= 99999
                    };

                    var csvConvertResult = CsvHelper.ConvertCsvTo<MeterReading>(csvContent, settings);

                    var readings = csvConvertResult.Instances;

                    var commitResult = await meterRepo.CommitReadings(readings);

                    //Populate counts to  indicate the overall success of the upload
                    uploadResult.CommitSuccessCount = commitResult.SuccessCount;
                    uploadResult.CommitFailureCount = commitResult.FailCount;
                    uploadResult.InvalidCsvRowCount = csvConvertResult.FailCount;
                }

            }

            return uploadResult;
        }
    }
}
