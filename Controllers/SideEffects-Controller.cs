using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Medications.SideEffects;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/medication")]
    public class SideEffectsController : ControllerBase
    {
        private readonly ISideEffects _SideEffectsService;

        public SideEffectsController(ISideEffects SideEffects)
        {
            _SideEffectsService = SideEffects;
        }

        /// <summary>
        /// This controller adds the side effects on a given day.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The date property must have the following structure:   
        ///     {
        ///        "date": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="201">Returns a message that the information has been successfully stored.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnSideEffect))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExpiredTokenException))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPost("side-effects")]
        [Produces("application/json")]
        public IActionResult AddSideEffects([FromBody] AddSideEffectDto values)
        {
            try
            {
                SideEffectsListDto sideEffect = _SideEffectsService.AddSideEffect(values);

                ReturnSideEffect response = new ReturnSideEffect
                {
                    sideEffect = sideEffect
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, sideEffect = response.sideEffect });
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
        /// This controller updates side effect records on a given day.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnSideEffect))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExpiredTokenException))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPut("side-effects")]
        [Produces("application/json")]
        public IActionResult UpdateSideEffects([FromBody] SideEffectsListDto values)
        {
            try
            {
                SideEffectsListDto sideEffect = _SideEffectsService.UpdateSideEffect(values);

                ReturnSideEffect response = new ReturnSideEffect
                {
                    sideEffect = sideEffect
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, sideEffect = response.sideEffect });
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
        /// This controller deletes side effect records on a given day.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnDeleteSideEffect))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExpiredTokenException))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("side-effects")]
        [Produces("application/json")]
        public IActionResult DeleteASideEffects([FromQuery] Guid sideEffectID)
        {
            try
            {
                string res = _SideEffectsService.DeleteSideEffect(sideEffectID);

                ReturnDeleteSideEffect response = new ReturnDeleteSideEffect
                {
                    status = res
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
