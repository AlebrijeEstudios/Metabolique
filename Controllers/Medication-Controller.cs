using AppVidaSana.Api;
using AppVidaSana.Models.Dtos.Exercise_Dtos;
using AppVidaSana.ProducesResponseType.Exercise;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.ProducesResponseType.Medications;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Models.Medications;
using AppVidaSana.ProducesResponseType.Medications.SideEffects;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/medication")]
    [EnableRateLimiting("concurrency")]
    public class MedicationController : ControllerBase
    {
        private readonly IMedication _MedicationService;
        private readonly ISideEffects _SideEffectsService;

        public MedicationController(IMedication MedicationService, ISideEffects SideEffects)
        {
            _MedicationService = MedicationService;
            _SideEffectsService = SideEffects;
        }

        /// <summary>
        /// This controller returns information on the medications that the user should consume or has consumed in the last 7 days.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The dateMedication property must have the following structure:   
        ///     {
        ///        "dateMedication": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns an array of medications, where each one of them manages an array of schedules. These are for the last 7 days.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnMedications))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetMedications([FromQuery] Guid accountID, [FromQuery] DateOnly date)
        {
            MedicationsAndValuesGraphicDto infoMedications = _MedicationService.GetMedications(accountID, date);

            ReturnMedications response = new ReturnMedications
            {
                medications = infoMedications.medications,
                weeklyAttachments = infoMedications.weeklyAttachments,
                sideEffects = infoMedications.sideEffects
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message, 
                                                             medications = response.medications, 
                                                             weeklyAttachments = response.weeklyAttachments,
                                                             sideEffects = response.sideEffects});
        }

        /// <summary>
        /// This controller adds the medications to be consumed during certain days.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The dateMedication property must have the following structure:   
        ///     {
        ///        "dateMedication": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="201">Returns a message that the information has been successfully stored.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return an error message if the user is not found.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnAddUpdateMedication))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddMedications([FromBody] AddMedicationUseDto medication)
        {
            try
            {
                InfoMedicationDto med = _MedicationService.AddMedication(medication);

                ReturnAddUpdateMedication response = new ReturnAddUpdateMedication
                {
                    medication = med
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, medication = response.medication });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (NotRepeatPeriodException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (UserNotFoundException ex)
            {

                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });

            }
        }

        /// <summary>
        /// This controller updates the properties of a medication.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The dateMedication property must have the following structure:   
        ///     {
        ///        "dateMedication": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>    
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateMedication))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public IActionResult UpdateMedication([FromBody] UpdateMedicationUseDto medication)
        {
            try
            {
                InfoMedicationDto med = _MedicationService.UpdateMedication(medication);

                ReturnAddUpdateMedication response = new ReturnAddUpdateMedication
                {
                    medication = med
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, medication = response.medication });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ListTimesVoidException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (NotRepeatPeriodException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (NewInitialDateAfterFinalDateException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (NewFinalDateBeforeInitialDateException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This controller updates the consumption status of a medication.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>    
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPut("status")]
        [Produces("application/json")]
        public IActionResult UpdateMedicationStatus([FromBody] UpdateMedicationStatusDto value)
        {
            try
            {
                _MedicationService.UpdateStatusMedication(value);

                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    message = "Ok.",
                    status = "Se registro el seguimiento correctamente"
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }
 
        /// <summary>
        /// This controller deletes a medication on a given day.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnDeleteMedication))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete]
        [Produces("application/json")]
        public IActionResult DeleteAMedication([FromQuery] Guid periodID, [FromQuery] DateOnly date)
        {
            try
            {
                string res = _MedicationService.DeleteAMedication(periodID, date);

                ReturnDeleteMedication response = new ReturnDeleteMedication
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
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
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnSideEffect))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
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
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (RepeatRegistrationException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
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
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnSideEffect))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
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
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
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
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnDeleteSideEffect))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
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
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }
    }
}
