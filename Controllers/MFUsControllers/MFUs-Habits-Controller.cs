using AppVidaSana.Api;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.ProducesResponseType.Habits.MFUsHabits;
using AppVidaSana.Exceptions.Habits;

namespace AppVidaSana.Controllers.MFUsControllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/monthly-habits-monitoring")]
    [EnableRateLimiting("sliding")]
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
        /// <response code="404">Returns an error message if the user is not found or if a record does not exist in the resultsHabits table.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnAddResponsesHabits))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddResponsesHabits([FromBody] SaveResponsesHabitsDto responses)
        {
            try
            {
                var results = _MFUsHabitsService.SaveAnswers(responses);
                var res = _MFUsHabitsService.SaveResults(results);

                ReturnAddResponsesHabits response = new ReturnAddResponsesHabits
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, actionStatus = response.actionStatus, status = response.status });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, actionStatus = response.actionStatus, status = response.status });
            }
            catch (RepeatRegistrationException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, actionStatus = response.actionStatus, status = response.status });
            }
            catch (UserNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, actionStatus = response.actionStatus, status = response.status });
            }
            catch (HabitNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, actionStatus = response.actionStatus, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, actionStatus = response.actionStatus, status = response.status });
            }
        }

        /// <summary>
        /// This controller returns responses from the monthly Habits tracking questionnaire.
        /// </summary>
        /// <response code="200">Return the answers of the questionnaire that was made in such month and such year.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnRetrieveResponsesHabits))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult RetrieveResponsesHabits([FromQuery] Guid accountID, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                RetrieveResponsesHabitsDto res = _MFUsHabitsService.RetrieveAnswers(accountID, month, year);

                ReturnRetrieveResponsesHabits response = new ReturnRetrieveResponsesHabits
                {
                    responsesAnswers = res
                };

                if (res.month == null)
                {
                    return StatusCode(StatusCodes.Status200OK, new { message = response.message, actionStatus = false });
                }

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, actionStatus = response.actionStatus, responsesAnswers = response.responsesAnswers });

            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, actionStatus = response.actionStatus, status = response.status });
            }

        }
    }
}
