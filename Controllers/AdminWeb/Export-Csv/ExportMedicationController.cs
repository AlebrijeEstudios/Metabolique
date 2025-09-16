using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Controllers.AdminWeb.Export_Csv
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Export - Medications")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/medications")]
    [RequestTimeout("CustomPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
    public class ExportMedicationController : ControllerBase
    {
        private readonly IExportToZip _ExportService;
        private const string exportFilter = "with_filter";
        private const string exportAll = "all";
        private const string formatDate = "yyyy-MM-dd";
        private const string typeArchive = "application/zip";

        public ExportMedicationController(IExportToZip exportService)
        {
            _ExportService = exportService;
        }

        /// <summary>
        /// This driver exports patient medication period records in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        [CommonApiResponses]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-periods-medications")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyPeriodMedicationsToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"PeriodsMedications_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyPeriodMedicationsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_PeriodsMedications_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyPeriodMedicationsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports patient side effect records in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        [CommonApiResponses]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-side-effects")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlySideEffectsToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"SideEffects_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlySideEffectsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_SideEffects_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlySideEffectsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }

        /// <summary>
        /// This driver exports the patients' monthly medication tracking records in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        [CommonApiResponses]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-mfu-medication")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyMFUsMedicationToCsvAsync([FromQuery] string typeExport, [FromQuery] FilterAdminDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"MFUsMedication_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsMedicationZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_MFUsMedication_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsMedicationZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }
    }
}