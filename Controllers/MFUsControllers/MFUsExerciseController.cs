using AppVidaSana.Api;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.ProducesResponseType.Exercise.MFUsExercise;

namespace AppVidaSana.Controllers.Seg_Men_Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/monthly-exercise-monitoring")]

    public class MFUsExerciseController : ControllerBase
    {
        private readonly IMFUsExercise _MFUsExcerciseService;
        private string mensaje = "Hubo un error, intentelo de nuevo.";

        public MFUsExerciseController(IMFUsExercise MFUsExcerciseService)
        {
            _MFUsExcerciseService = MFUsExcerciseService;
        }

        /// <summary>
        /// This controller receives responses from the monthly monitoring account.
        /// </summary>
        /// <response code="201">Returns a message indicating that the answers were stored correctly.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnAddResponsesExercise))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddResponsesExercise([FromBody] SaveResponsesDto responses)
        {
            try
            {
                var res = _MFUsExcerciseService.SaveAnswers(responses);

                ReturnAddResponsesExercise response = new ReturnAddResponsesExercise
                {
                    response = res
                };

                return StatusCode(StatusCodes.Status201Created, new { response });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    response = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
        }

        /// <summary>
        /// This controller returns the answers from the monthly monitoring questionnaire.
        /// </summary>
        /// <response code="200">Return the answers of the questionnaire that was made in such month and such year.</response>
        /// <response code="404">Return an error message if the user is not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnRetrieveResponses))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult RetrieveResponses([FromQuery] Guid id, [FromQuery] string month, [FromQuery] int year)
        {
            try
            {
                RetrieveResponsesDto res = _MFUsExcerciseService.RetrieveAnswers(id, month, year);

                ReturnRetrieveResponses response = new ReturnRetrieveResponses
                {
                    response = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (UserNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    response = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
        }
    }
}
