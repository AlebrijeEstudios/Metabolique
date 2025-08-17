using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
namespace AppVidaSana.Controllers.AdminWeb.Export_Csv
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Export - Feedings")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/feedings")]
    [RequestTimeout("CustomPolicy")]
    public class ExportFeedingController : ControllerBase
    {
        private readonly IExportToZip _ExportService;
        private const string exportFilter = "with_filter"; 
        private const string exportAll = "all";
        private const string formatDate = "yyyy-MM-dd";
        private const string typeArchive = "application/zip";

        public ExportFeedingController(IExportToZip exportService)
        {
            _ExportService = exportService;
        }

        /// <summary>
        /// This driver exports patient feeding records in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-feedings")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyFeedsOfAUserToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"InfoFeedings_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyFeedingsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_InfoFeedings_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyFeedingsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports the food consumed per feeding record in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-foods")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyFoodsConsumedPerFeedingToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"FoodsConsumedPerFeedingPerPatient_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyFoodsConsumedPerFeedingZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_FoodsConsumedPerFeedingPerPatient_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyFoodsConsumedPerFeedingZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports the calories needed per patient in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-calories-needed-per-user")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyUserCaloriesToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"CaloriesRequiredPerPatient_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyUserCaloriesZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_CaloriesRequiredPerPatient_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyUserCaloriesZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports the monthly patient feeding tracking records in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-mfu-feeding")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyMFUsFeedingToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"MFUsFeeding_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsFeedingZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_MFUsFeeding_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsFeedingZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }


        /// <summary>
        /// This driver exports the calories consumed per patient in csv a records.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-calories-consumed-per-day")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyCaloriesConsumedToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"TotalCaloriesConsumedPerPatientPerDay_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyCaloriesConsumedZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_TotalCaloriesConsumedPerPatientPerDay_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyCaloriesConsumedZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports the calories required per days per patient in csv a records.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-calories-required-per-days")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyCaloriesRequiredPerDaysToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"CaloriesRequiredPerDaysPerPatient_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyCaloriesRequiredPerDaysZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_CaloriesRequiredPerDaysPerPatient_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyCaloriesRequiredPerDaysZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }
    }
}
