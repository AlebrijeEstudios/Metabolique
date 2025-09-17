using AppVidaSana.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using AppVidaSana.ProducesResponseType.AdminWeb;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.ProducesResponseType.ResponseOperationsFilters.ApiResponsesAttribute;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Patients")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/patients")]
    [RequestTimeout("CustomPolicy")]
    public class AdminPatientsController : ControllerBase
    {
        private readonly IAWPatients _PatientsService;

        public AdminPatientsController(IAWPatients PatientsService)
        {
            _PatientsService = PatientsService;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPatientsResponse))]
        [CommonApiResponse]
        [UnauthorizedApiResponse]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetPatientsAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var patients = await _PatientsService.GetPatientsAsync(filter, page, HttpContext.RequestAborted);

            GetPatientsResponse response = new GetPatientsResponse
            {
                patients = patients
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message, patients = response.patients });
        }
    }
}
