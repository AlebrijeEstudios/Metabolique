﻿using AppVidaSana.Api;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Habits.DrugsHabit;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services.IServices.IHabits.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.ProducesResponseType.Exercise;
using AppVidaSana.ProducesResponseType.Habits.SleepHabit_DrugsHabit;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits-sleep-drugs")]
    [EnableRateLimiting("sliding")]
    public class HabitSleepDrugsController : ControllerBase
    {
        private readonly ISleepDrugsHabit _SleepDrugsHabitService;

        public HabitSleepDrugsController(ISleepDrugsHabit SleepDrugsHabitService)
        {
            _SleepDrugsHabitService = SleepDrugsHabitService;
        }

        /// <summary>
        /// This controller adds the cigars consumed by the user and the user's sleep hours.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The dateRegister property must have the following structure:   
        ///     {
        ///        "dateRegister": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="201">Returns a message that the information has been successfully stored.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return an error message if the user is not found.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnAddUpdateSleepHoursAndDrugsConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddSleepHoursAndDrugsConsumed([FromBody] SleepingHoursAndDrugsConsumedDto values)
        {
            try
            {
                var habits = _SleepDrugsHabitService.AddSleepHoursAndDrugsConsumed(values);

                ReturnAddUpdateSleepHoursAndDrugsConsumed response = new ReturnAddUpdateSleepHoursAndDrugsConsumed
                {
                    habitSleepDrugs = habits
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, habitsSleepDrugs = response.habitSleepDrugs });
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
        /// This controller updates the cigars consumed by the user and the user's sleep hours.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The dateRegister property must have the following structure:   
        ///     {
        ///        "dateRegister": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Returns a message indicating that no records have been found for a certain number of cigarettes consumed.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateSleepHoursAndDrugsConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public IActionResult UpdateSleepHoursAndDrugsConsumed([FromBody] ReturnSleepHoursAndDrugsConsumedDto values)
        {
            try
            {
                var habit = _SleepDrugsHabitService.UpdateSleepHoursAndDrugsConsumed(values);

                ReturnAddUpdateSleepHoursAndDrugsConsumed response = new ReturnAddUpdateSleepHoursAndDrugsConsumed
                {
                    habitSleepDrugs = habit
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, habitsSleepDrugs = response.habitSleepDrugs });
            }

            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (HabitNotFoundException ex)
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
        /// This controller deletes a record containing certain cigars consumed.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Returns a message indicating that the record with the cigarettes consumed does not exist in the DrugsHabit table.</response>     
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnDeleteSleepHoursAndDrugsConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{drugsHabitID:guid}")]
        [Produces("application/json")]
        public IActionResult DeleteDrugsConsumed(Guid drugsHabitID)
        {
            try
            {
                var res = _SleepDrugsHabitService.DeleteDrugsConsumed(drugsHabitID);

                ReturnDeleteSleepHoursAndDrugsConsumed response = new ReturnDeleteSleepHoursAndDrugsConsumed
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
            catch (HabitNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This controller deletes a log containing certain hours of sleep.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Returns a message indicating that the record with the specified sleep hours does not exist in the SleepHabit table.</response>     
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnDeleteSleepHoursAndDrugsConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{habitSleepID:guid}")]
        [Produces("application/json")]
        public IActionResult DeleteSleepHours(Guid habitSleepID)
        {
            try
            {
                var res = _SleepDrugsHabitService.DeleteSleepHours(habitSleepID);

                ReturnDeleteSleepHoursAndDrugsConsumed response = new ReturnDeleteSleepHoursAndDrugsConsumed
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
            catch (HabitNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
        }
    }
}