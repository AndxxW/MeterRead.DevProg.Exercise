using EnSek.MeterRead.BL.Workers;
using EnSek.MeterRead.DAL.Interfaces;
using EnSek.MeterRead.DAL.Repositories;
using EnSek.MeterRead.Models.Classes;
using EnSek.MeterRead.Models.Dtos;
using EnSek.MeterRead.Models.Entities;
using EnSek.MeterRead.Utilities.Classes;
using Microsoft.AspNetCore.Mvc;
using EnSek.MeterRead.Utilities.Extensions;
using EnSek.MeterRead.Utilities.Helpers;

namespace Ensek.MeterRead.API.Controllers
{
    [ApiController]
    [Route("meter-reading-uploads")]
    public class MeterReadingUploadsController : ControllerBase
    {
        private readonly ILogger<MeterReadingUploadsController> _logger;

        private readonly IMeterRepo _meterRepo;

        public MeterReadingUploadsController(ILogger<MeterReadingUploadsController> logger, IMeterRepo meterRepo)
        {
            _logger = logger;
            _meterRepo = meterRepo;
        }

        [HttpGet]
        public void Get()
        {
        }

        /// <summary>
        /// Processes an uploaded file of meter readings
        /// </summary>
        /// <param name="file">A csv file containing the meter readings</param>
        /// <returns>A summary of the processing (success and failure counts etc)</returns>
        [HttpPost]
        public async Task<MeterReadingsUploadsSummaryDto> ProcessMeterReadings(IFormFile file)
        {
            try
            {
                return await MeterReadingsFileWorker.ProcessCsvFile(file, _meterRepo);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to process meter readings upload", ex);
                throw;
            }

        }
    }
}