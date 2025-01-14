using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Medications.MFUsMedications;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.MFUsControllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/monthly-medications-monitoring")]
    public class MFUsMedicationsController : ControllerBase
    {
        private readonly IMFUsMedications _MFUsMedicationsService;

        public MFUsMedicationsController(IMFUsMedications MFUsMedicationsService)
        {
            _MFUsMedicationsService = MFUsMedicationsService;
        }

        /// <summary>
        /// This controller stores the responses and results of the monthly medication tracking survey.
        /// </summary>
        /// <response code="201">Returns a message indicating that the answers were stored correctly.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="404">Returns an error message if the user is not found or if there is no record in the database.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnResponsesAndResultsMFUsMedications))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddAnswersMedications([FromBody] SaveResponsesMedicationsDto responses)
        {
            try
            {
                var results = await _MFUsMedicationsService.SaveAnswersAsync(responses, HttpContext.RequestAborted);

                ReturnResponsesAndResultsMFUsMedications response = new ReturnResponsesAndResultsMFUsMedications
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
        /// This controller returns responses from the monthly medication tracking questionnaire.
        /// </summary>
        /// <response code="200">Return the answers of the questionnaire that was made in such month and such year.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnResponsesAndResultsMFUsMedications))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))] 
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> RetrieveAnswersMedications([FromQuery] Guid accountID, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var results = await _MFUsMedicationsService.RetrieveAnswersAsync(accountID, month, year, HttpContext.RequestAborted);

                ReturnResponsesAndResultsMFUsMedications response = new ReturnResponsesAndResultsMFUsMedications
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
        /// This controller updates the responses and results of the monthly medication tracking.
        /// </summary>
        /// <response code="200">Returns monthly monitoring results and responses.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnResponsesAndResultsMFUsMedications))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))] 
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateAnswersMedications([FromBody] UpdateResponsesMedicationsDto responses)
        {
            try
            {
                var results = await _MFUsMedicationsService.UpdateAnswersAsync(responses, HttpContext.RequestAborted);

                ReturnResponsesAndResultsMFUsMedications response = new ReturnResponsesAndResultsMFUsMedications
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
