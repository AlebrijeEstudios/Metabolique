using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Habits.MFUsHabits;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.MFUsControllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/monthly-habits-monitoring")]
    public class MFUsHabitsController : ControllerBase
    {
        private readonly IMFUsHabits _MFUsHabitsService;

        public MFUsHabitsController(IMFUsHabits MFUsHabitsService)
        {
            _MFUsHabitsService = MFUsHabitsService;
        }

        /// <summary>
        /// This controller stores the responses and results of the monthly Habits tracking survey.
        /// </summary>
        /// <response code="201">Returns a message indicating that the answers were stored correctly.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="404">Returns an error message if the user is not found or if a record does not exist in the resultsHabits table.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnResponsesAndResultsMFUsHabits))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddAnswersHabitsAsync([FromBody] SaveResponsesHabitsDto responses)
        {
            try
            {
                var results = await _MFUsHabitsService.SaveAnswersAsync(responses, HttpContext.RequestAborted);

                ReturnResponsesAndResultsMFUsHabits response = new ReturnResponsesAndResultsMFUsHabits
                {
                    mfus = results
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, mfus = response.mfus });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (RepeatRegistrationException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (UserNotFoundException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
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
        /// This controller returns responses from the monthly Habits tracking questionnaire.
        /// </summary>
        /// <response code="200">Return the answers of the questionnaire that was made in such month and such year.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnResponsesAndResultsMFUsHabits))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))] 
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> RetrieveAnswersHabits([FromQuery] Guid accountID, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var results = await _MFUsHabitsService.RetrieveAnswersAsync(accountID, month, year, HttpContext.RequestAborted);

                ReturnResponsesAndResultsMFUsHabits response = new ReturnResponsesAndResultsMFUsHabits
                {
                    mfus = results
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, mfus = response.mfus });

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
        /// This controller updates the responses and results of the monthly habit tracking.
        /// </summary>
        /// <response code="200">Returns monthly monitoring results and responses.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnResponsesAndResultsMFUsHabits))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateAnswersHabits([FromBody] UpdateResponsesHabitsDto responses)
        {
            try
            {
                var results = await _MFUsHabitsService.UpdateAnswersAsync(responses, HttpContext.RequestAborted);

                ReturnResponsesAndResultsMFUsHabits response = new ReturnResponsesAndResultsMFUsHabits
                {
                    mfus = results
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, mfus = response.mfus });
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
    }
}
