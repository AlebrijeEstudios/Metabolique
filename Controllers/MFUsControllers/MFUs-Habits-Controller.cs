using AppVidaSana.Api;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Exercise.MFUsExercise;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
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
        /// <response code="201">Returns a message indicating that the answers were stored correctly. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns an error message if the user is not found or if a record does not exist in the resultsHabits table. The information is stored in the attribute called 'response'.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid. The information is stored in the attribute called 'response'.</response>
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

                return StatusCode(StatusCodes.Status201Created, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (RepeatRegistrationException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (UserNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (HabitNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
        }

        /// <summary>
        /// This controller returns responses from the monthly Habits tracking questionnaire..
        /// </summary>
        /// <response code="200">Return the answers of the questionnaire that was made in such month and such year. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns an error message if the user is not found or if a record with the survey results is not found. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnRetrieveResponsesHabits))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult RetrieveResponsesHabits([FromQuery] Guid id, [FromQuery] string month, [FromQuery] int year)
        {

            RetrieveResponsesHabitsDto res = _MFUsHabitsService.RetrieveAnswers(id, month, year);

            ReturnRetrieveResponsesHabits response = new ReturnRetrieveResponsesHabits
            {
                responsesAnswers = res
            };

            return StatusCode(StatusCodes.Status200OK, new { response });
        }
    }
}
