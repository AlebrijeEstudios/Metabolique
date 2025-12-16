using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.AdminWeb.Medication;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.ProducesResponseType.ResponseOperationsFilters.ApiResponsesAttribute;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Medications")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/medications")]
    [RequestTimeout("CustomPolicy")]
    public class AdminMedicationsController : ControllerBase
    {
        private readonly IAWMedication _MedicationService;

        public AdminMedicationsController(IAWMedication MedicationService)
        {
            _MedicationService = MedicationService;
        }

        /// <summary>
        /// This driver obtains the medication consumption records per patient.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        [CommonApiResponse]
        [BadRequestApiResponse]
        [UnauthorizedApiResponse]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetInfoMedResponse))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("info-medications")]
        [Produces("application/json")]
        public async Task<IActionResult> GetInfoMedicationsPerUserAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var meds = await _MedicationService.GetAllInfoMedicationsPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetInfoMedResponse response = new GetInfoMedResponse
                {
                    meds = meds
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, meds = response.meds});
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
        /// This driver obtains the side effect records per patient.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        [CommonApiResponse]
        [BadRequestApiResponse]
        [UnauthorizedApiResponse]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSideEffectsResponse))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("side-effects")]
        [Produces("application/json")]
        public async Task<IActionResult> GetSideEffectsAAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
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
        /// This driver obtains the monthly medication tracking records per patient.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        [CommonApiResponse]
        [BadRequestApiResponse]
        [UnauthorizedApiResponse]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMFUsMedicationResponse))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("mfu-medication")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMFUsMedicationsAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
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
    }
}
