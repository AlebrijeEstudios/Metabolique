using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Exercise.MFUsExercise;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.Seg_Men_Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/monthly-exercise-monitoring")]
    public class MFUsExerciseController : ControllerBase
    {
        private readonly IMFUsExercise _MFUsExerciseService;

        public MFUsExerciseController(IMFUsExercise MFUsExerciseService)
        {
            _MFUsExerciseService = MFUsExerciseService;
        }

        /// <summary>
        /// This controller stores the responses and results of the monthly Exercise tracking survey.
        /// </summary>
        /// <response code="201">Returns monthly monitoring results and responses.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return an error message if the user is not found.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnResponsesAndResultsMFUsExercise))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddAnswersExerciseAsync([FromBody] SaveResponsesExerciseDto responses)
        {
            try
            {
                var results = await _MFUsExerciseService.SaveAnswersAsync(responses, HttpContext.RequestAborted);

                ReturnResponsesAndResultsMFUsExercise response = new ReturnResponsesAndResultsMFUsExercise
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
        /// This controller returns responses from the monthly Exercise tracking questionnaire.
        /// </summary>
        /// <response code="200">It returns the answers of the questionnaire that was made in such month and such year, otherwise it returns empty results.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnResponsesAndResultsMFUsExercise))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> RetrieveAnswersAsync([FromQuery] Guid accountID, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var results = await _MFUsExerciseService.RetrieveAnswersAsync(accountID, month, year, HttpContext.RequestAborted);

                ReturnResponsesAndResultsMFUsExercise response = new ReturnResponsesAndResultsMFUsExercise
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
        /// This controller updates monthly tracking responses and results.
        /// </summary>
        /// <response code="200">Returns monthly monitoring results and responses.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnResponsesAndResultsMFUsExercise))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateAnswersExerciseAsync([FromBody] UpdateResponsesExerciseDto responses)
        {
            try
            {
                var results = await _MFUsExerciseService.UpdateAnswersAsync(responses, HttpContext.RequestAborted);

                ReturnResponsesAndResultsMFUsExercise response = new ReturnResponsesAndResultsMFUsExercise
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
