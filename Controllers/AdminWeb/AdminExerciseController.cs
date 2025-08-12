using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.AdminWeb.Exercise;
using AppVidaSana.Exceptions;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Exercises")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/exercises")]
    [RequestTimeout("CustomPolicy")]
    public class AdminExerciseController : ControllerBase
    {
        private readonly IAWExercise _ExerciseService;

        public AdminExerciseController(IAWExercise ExerciseService)
        {
            _ExerciseService = ExerciseService;
        }

        /// <summary>
        /// Patients' physical activity records are obtained.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetExercisesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetExercisesPerUserAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var exs = await _ExerciseService.GetAllExercisesPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetExercisesResponse response = new GetExercisesResponse
                {
                    exercises = exs
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, exercises = response.exercises });
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
        /// Monthly follow-ups of patients' physical activity are obtained.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMFUsExerciseResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("mfu-exercise")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMFUsExerciseAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var mfu = await _ExerciseService.GetMFUsExerciseAsync(filter, page, HttpContext.RequestAborted);

                GetMFUsExerciseResponse response = new GetMFUsExerciseResponse
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
        /// Total active minutes per exercises realizaded per patient.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetActiveMinutesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("active-minutes")]
        [Produces("application/json")]
        public async Task<IActionResult> GetActiveMinutesPerExerciseAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var actM = await _ExerciseService.GetAllActiveMinutesPerExerciseAsync(filter, page, HttpContext.RequestAborted);

                GetActiveMinutesResponse response = new GetActiveMinutesResponse
                {
                    actMin = actM
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, actMin = response.actMin });
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
