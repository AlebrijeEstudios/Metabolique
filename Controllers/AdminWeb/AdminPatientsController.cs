using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using AppVidaSana.ProducesResponseType.AdminWeb;
using AppVidaSana.Exceptions;
using AppVidaSana.Services.IServices.IAdminWeb;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/admin/patients")]
    [RequestTimeout("CustomPolicy")]
    public class AdminPatientsController : ControllerBase
    {
        private readonly IAWPatients _PatientsService;
        private readonly IExportToZip _ExportService;

        public AdminPatientsController(IAWPatients PatientsService, IExportToZip exportService)
        {
            _PatientsService = PatientsService;
            _ExportService = exportService;
        }

        /// <summary>
        /// This controller obtains all patient accounts.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The birthDate property must have the following structure:   
        ///     {
        ///        "birthDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns account information if found.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetPatientsAsync([FromQuery] Guid doctorID, [FromQuery] int page)
        {
            try
            {
                var patients = await _PatientsService.GetPatientsAsync(doctorID, page, HttpContext.RequestAborted);

                PatientsResponse response = new PatientsResponse
                {
                    patients = patients
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, account = response.patients });
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
        /// This driver exports in csv all records.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("export-all")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportAllToCsvAsync()
        {
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            string fileName = $"All_Sections_{dateSuffix}.zip";

            var zipBytes = await _ExportService.GenerateAllSectionsZipAsync(HttpContext.RequestAborted);
            return File(zipBytes, "application/zip", fileName);
        }
    }
}
