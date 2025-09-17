using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.ProducesResponseType.ResponseOperationsFilters.ApiResponsesAttribute;

namespace AppVidaSana.Controllers.AdminWeb.Export_Csv
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Export - Exercises")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/exercises")]
    [RequestTimeout("CustomPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class ExportExerciseController : ControllerBase
    { 
        private readonly IExportToZip _ExportService;
        private const string exportFilter = "with_filter";
        private const string exportAll = "all";
        private const string formatDate = "yyyy-MM-dd";
        private const string typeArchive = "application/zip";

        public ExportExerciseController(IExportToZip exportService)
        {
            _ExportService = exportService;
        }

        /// <summary>
        /// This controller exports patients' physical activity records in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        [CommonApiResponse]
        [UnauthorizedApiResponse]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-exercises")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyExercisesToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"Exercises_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyExercisesZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_Exercises_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyExercisesZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports patients' monthly physical activity tracking records in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        [CommonApiResponse]
        [UnauthorizedApiResponse]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-mfu-exercise")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyMFUsExerciseToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"MFUsExercise_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsExerciseZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_MFUsExercise_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsExerciseZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports active minutes in csv records.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        [CommonApiResponse]
        [UnauthorizedApiResponse]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-active-minutes")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyActivesMinutesToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"ActivesMinutes_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyActivesMinutesZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_ActivesMinutes_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyActivesMinutesZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }
    }
}
