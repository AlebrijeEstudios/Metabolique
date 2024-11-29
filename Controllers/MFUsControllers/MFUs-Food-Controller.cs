using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Food.MFUsFood;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.MFUsControllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/monthly-food-monitoring")]
    [RequestTimeout("CustomPolicy")]
    public class MFUsFoodController : ControllerBase
    {
        private readonly IMFUsFood _MFUsFoodService;

        public MFUsFoodController(IMFUsFood MFUsFoodService)
        {
            _MFUsFoodService = MFUsFoodService;
        }

        /// <summary>
        /// This controller stores the responses and results of the monthly food tracking survey.
        /// </summary>
        /// <response code="201">Returns a message indicating that the answers were stored correctly.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MFUsFoodResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddAnswersFoodAsync([FromBody] MFUsFoodDto values)
        {
            try
            {
                var results = await _MFUsFoodService.SaveAnswersAsync(values, HttpContext.RequestAborted);

                MFUsFoodResponse response = new MFUsFoodResponse
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
        /// This controller returns the answers from the monthly food monitoring questionnaire.
        /// </summary>
        /// <response code="200">Return the answers of the questionnaire that was made in such month and such year.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MFUsFoodResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> RetrieveAnswersFoodAsync([FromQuery] Guid accountID, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var results = await _MFUsFoodService.RetrieveAnswersAsync(accountID, month, year, HttpContext.RequestAborted);

                MFUsFoodResponse response = new MFUsFoodResponse
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
        /// This controller updates the responses and results of the monthly feed tracking. 
        /// </summary>
        /// <response code="200">Returns monthly monitoring results and responses.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MFUsFoodResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateAnswersFoodAsync([FromBody] UpdateAnswersMFUsFoodDto values)
        {
            try
            {
                var results = await _MFUsFoodService.UpdateAnswersAsync(values, HttpContext.RequestAborted);

                MFUsFoodResponse response = new MFUsFoodResponse
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
