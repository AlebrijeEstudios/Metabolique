using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using Microsoft.AspNetCore.RateLimiting;

namespace AppVidaSana.Controllers.AdminWeb.Export_Csv
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Export - Habits")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/habits")]
    [RequestTimeout("CustomPolicy")]
    public class ExportHabitController : ControllerBase
    {
        private readonly IExportToZip _ExportService;
        private const string exportFilter = "with_filter";
        private const string exportAll = "all";
        private const string formatDate = "yyyy-MM-dd";
        private const string typeArchive = "application/zip";

        public ExportHabitController(IExportToZip exportService)
        {
            _ExportService = exportService;
        }

        /// <summary>
        /// This driver exports the water drinking habits records per patient in a csv file.
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
        [HttpGet("export-habits-drink")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyHabitsDrinkToCsvAsync([FromQuery] string typeExport, [FromQuery] HabitFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"HabitsDrink_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrinkZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_HabitsDrink_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrinkZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports the drug habit records per patient in a csv file.
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
        [HttpGet("export-habits-drugs")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyHabitsDrugsToCsvAsync([FromQuery] string typeExport, [FromQuery] HabitFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"HabitsDrugs_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrugsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_HabitsDrugs_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrugsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeExport, fileName);
        }

        /// <summary>
        /// This driver exports the sleep habits records per patient in a csv file.
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
        [HttpGet("export-habits-sleep")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyHabitsSleepToCsvAsync([FromQuery] string typeExport, [FromQuery] HabitFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"HabitsSleep_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsSleepZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_HabitsSleep_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsSleepZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports the patients' monthly habit tracking records in a csv file.
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
        [HttpGet("export-mfu-habit")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyMFUsHabitsToCsvAsync([FromQuery] string typeExport, [FromQuery] PatientFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"MFUsHabits_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsHabitsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_MFUsHabits_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsHabitsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }
    }
}
