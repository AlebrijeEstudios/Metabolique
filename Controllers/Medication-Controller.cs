using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Medications;
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
    public class MedicationController : ControllerBase
    {
        private readonly IMedication _MedicationService;

        public MedicationController(IMedication MedicationService)
        {
            _MedicationService = MedicationService;
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
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnMedications))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetMedications([FromQuery] Guid accountID, [FromQuery] DateOnly date)
        {
            try
            {
                MedicationsAndValuesGraphicDto infoMedications = _MedicationService.GetMedications(accountID, date);

                ReturnMedications response = new ReturnMedications
                {
                    medications = infoMedications.medications,
                    weeklyAttachments = infoMedications.weeklyAttachments,
                    sideEffects = infoMedications.sideEffects,
                    mfuStatus = infoMedications.mfuStatus
                };

                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = response.message,
                    medications = response.medications,
                    weeklyAttachments = response.weeklyAttachments,
                    sideEffects = response.sideEffects,
                    mfuStatus = response.mfuStatus
                });
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnAddUpdateMedication))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddMedications([FromBody] AddMedicationUseDto medication)
        {
            try
            {
                InfoMedicationDto? med = _MedicationService.AddMedication(medication);

                ReturnAddUpdateMedication response = new ReturnAddUpdateMedication
                {
                    medication = med
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, medication = response.medication });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (NotRepeatPeriodException ex)
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateMedication))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public IActionResult UpdateMedication([FromBody] UpdateMedicationUseDto medication)
        {
            try
            {
                InfoMedicationDto? med = _MedicationService.UpdateMedication(medication);

                ReturnAddUpdateMedication response = new ReturnAddUpdateMedication
                {
                    medication = med
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, medication = response.medication });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ListTimesVoidException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (NotRepeatPeriodException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (NewInitialDateAfterFinalDateException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (NewFinalDateBeforeInitialDateException ex)
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
        /// This controller updates the consumption status of a medication.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>    
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPut("status")]
        [Produces("application/json")]
        public IActionResult UpdateMedicationStatus([FromBody] UpdateMedicationStatusDto value)
        {
            try
            {
                _MedicationService.UpdateStatusMedication(value);

                ExceptionMessage response = new ExceptionMessage
                {
                    message = "Ok.",
                    status = "Se registro el seguimiento correctamente"
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

        /// <summary>
        /// This controller deletes a medication on a given day.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnDeleteMedication))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
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
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }
    }
}
