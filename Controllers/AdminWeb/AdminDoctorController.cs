using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.ProducesResponseType.AdminWeb.Doctor;
using Microsoft.AspNetCore.RateLimiting;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize(Roles = "Admin")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Doctors")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/doctors")]
    [RequestTimeout("CustomPolicy")]
    public class AdminDoctorController : ControllerBase
    {
        private readonly IAWDoctors _DoctorService;

        public AdminDoctorController(IAWDoctors DoctorsService)
        {
            _DoctorService = DoctorsService;
        }

        /// <summary>
        /// This controller obtains all doctor accounts.
        /// </summary>
        /// <response code="200">Returns account information if found.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDoctorsResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetDoctorsAsync([FromQuery] DoctorFilterDto filter, [FromQuery] int page)
        {
            var doctors = await _DoctorService.GetDoctorsAsync(filter, page, HttpContext.RequestAborted);

            GetDoctorsResponse response = new GetDoctorsResponse
            {
                doctors = doctors
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message, doctors = response.doctors });
        }

        /// <summary>
        /// This controller creates the doctor's account.
        /// </summary>
        /// <response code="201">Returns a token to validate in the app.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message that you were unable to log in.</response>        
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddUpdateDoctorsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [AllowAnonymous]
        [EnableRateLimiting("write")]
        [HttpPost]
        [Produces("application/json")]
        [RequestTimeout("CustomPolicy")]
        public async Task<IActionResult> CreateDoctorAsync([FromBody] AWDoctorDto values)
        {
            try
            {
                var doctor = await _DoctorService.CreateDoctorAsync(values, HttpContext.RequestAborted);

                AddUpdateDoctorsResponse response = new AddUpdateDoctorsResponse
                {
                    doctor = doctor
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, doctor = response.doctor });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (NoRoleAssignmentException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ValuesInvalidException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This driver updates the doctor's account.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response>    
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddUpdateDoctorsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("write")]
        [HttpPut]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateDoctorAsync([FromBody] AllDoctorsDto values)
        {
            try
            {
                var doctor = await _DoctorService.UpdateDoctorAsync(values, HttpContext.RequestAborted);

                AddUpdateDoctorsResponse response = new AddUpdateDoctorsResponse
                {
                    doctor = doctor
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, doctor = response.doctor });

            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This driver deletes the doctor's account.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response>
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("write")]
        [HttpDelete("{doctorID:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteDoctorAsync(Guid doctorID)
        {
            try
            {
                var message = await _DoctorService.DeleteDoctorAsync(doctorID, HttpContext.RequestAborted);

                ResponseMessage response = new ResponseMessage
                {
                    status = message
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
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
