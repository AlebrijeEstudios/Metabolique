using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

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

        public ExportHabitController(IExportToZip exportService)
        {
            _ExportService = exportService;
        }

        /// <summary>
        /// This driver exports the water drinking habits records per patient in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("export-habits-drink")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyHabitsDrinkToCsvAsync([FromQuery] string typeExport, [FromQuery] HabitDrinkFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"HabitsDrink_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrinkZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_HabitsDrink_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrinkZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }

        /// <summary>
        /// This driver exports the drug habit records per patient in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("export-habits-drugs")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyHabitsDrugsToCsvAsync([FromQuery] string typeExport, [FromQuery] HabitDrugFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"HabitsDrugs_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrugsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_HabitsDrugs_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrugsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }

        /// <summary>
        /// This driver exports the sleep habits records per patient in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("export-habits-sleep")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyHabitsSleepToCsvAsync([FromQuery] string typeExport, [FromQuery] HabitSleepFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"HabitsSleep_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsSleepZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_HabitsSleep_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsSleepZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }

        /// <summary>
        /// This driver exports the patients' monthly habit tracking records in a csv file.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("export-mfu-habit")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyMFUsHabitsToCsvAsync([FromQuery] string typeExport, [FromQuery] PatientFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"MFUsHabits_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsHabitsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_MFUsHabits_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsHabitsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }
    }
}
