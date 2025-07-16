using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AppVidaSana.Controllers
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("App - Doctors")]
    [ApiExplorerSettings(GroupName = "app")]
    [Route("api/doctors")]
    [RequestTimeout("CustomPolicy")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctor _DoctorService;

        public DoctorController(IDoctor DoctorService)
        {
            _DoctorService = DoctorService;
        }

        /// <summary>
        /// This controller obtains the list doctor´s.
        /// </summary>
        /// <response code="200">Returns account information if found.</response>
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetDoctorsAsync()
        {

            var doctors = await _DoctorService.GetDoctorsAsync(HttpContext.RequestAborted);

            DoctorResponse response = new DoctorResponse
            {
                doctors = doctors
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message, doctors = response.doctors });
        }
    }
}
