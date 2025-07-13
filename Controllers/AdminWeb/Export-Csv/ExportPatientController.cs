using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using Microsoft.AspNetCore.RateLimiting;

namespace AppVidaSana.Controllers.AdminWeb.Export_Csv
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Export - Patients")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/patients")]
    [RequestTimeout("CustomPolicy")]
    public class ExportPatientController : ControllerBase
    {
        private readonly IExportToZip _ExportService;
        private const string exportFilter = "with_filter";
        private const string exportAll = "all";
        private const string formatDate = "yyyy-MM-dd";
        private const string typeArchive = "application/zip";

        public ExportPatientController(IExportToZip exportService)
        {
            _ExportService = exportService;
        }

        /// <summary>
        /// This driver exports in csv records.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("export-patients")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyPatientsToCsvAsync([FromQuery] string typeExport, [FromQuery] PatientFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString(formatDate);
            byte[] zipBytes = [];

            if (typeExport == exportFilter)
            {
                fileName = $"Patients_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyPatientsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == exportAll)
            {
                fileName = $"All_Patients_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyPatientsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, typeArchive, fileName);
        }
    }
}
