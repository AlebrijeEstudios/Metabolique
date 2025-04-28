using AppVidaSana.Services.IServices.IAdminWeb;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;
using AppVidaSana.ProducesResponseType.AdminWeb.Medication;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/admin/medications")]
    [RequestTimeout("CustomPolicy")]
    public class AdminMedicationsController : ControllerBase
    {
        private readonly IAWMedication _MedicationService;
        private readonly IExportToZip _ExportService;

        public AdminMedicationsController(IAWMedication MedicationService, IExportToZip exportService)
        {
            _MedicationService = MedicationService;
            _ExportService = exportService;
        }

        /// <summary>
        /// PENDIENTE
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPeriodMedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("periods-medications")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPeriodMedicationsPerUserAsync([FromQuery] PeriodMedicationsFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var pMeds = await _MedicationService.GetAllPeriodMedicationsPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetPeriodMedResponse response = new GetPeriodMedResponse
                {
                    periodsMed = pMeds
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, periods = response.periodsMed });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// PENDIENTE
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSideEffectsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("side-effects")]
        [Produces("application/json")]
        public async Task<IActionResult> GetSideEffectsAAsync([FromQuery] SideEffectsFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var sEff = await _MedicationService.GetAllSideEffectsAsync(filter, page, HttpContext.RequestAborted);

                GetSideEffectsResponse response = new GetSideEffectsResponse
                {
                    sideEffects = sEff
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, sideEffects = response.sideEffects });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// PENDIENTE
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSideEffectsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("mfu-medication")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMFUsMedicationsAsync([FromQuery] MFUsMedicationFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var mfu = await _MedicationService.GetMFUsMedicationsAsync(filter, page, HttpContext.RequestAborted);

                GetMFUsMedicationResponse response = new GetMFUsMedicationResponse
                {
                    mfu = mfu
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, mfu = response.mfu });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
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
        [HttpGet("export-periods-medications")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyPeriodMedicationsToCsvAsync([FromQuery] string typeExport, [FromQuery] PeriodMedicationsFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"PeriodsMedications_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyPeriodMedicationsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_PeriodsMedications_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyPeriodMedicationsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
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
        [HttpGet("export-side-effects")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlySideEffectsToCsvAsync([FromQuery] string typeExport, [FromQuery] SideEffectsFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"SideEffects_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlySideEffectsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_SideEffects_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlySideEffectsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
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
        [HttpGet("export-mfu-medication")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyMFUsMedicationToCsvAsync([FromQuery] string typeExport, [FromQuery] MFUsMedicationFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"MFUsMedication_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsMedicationZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_MFUsMedication_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsMedicationZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }
    }
}
