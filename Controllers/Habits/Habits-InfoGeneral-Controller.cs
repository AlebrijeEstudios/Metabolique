﻿using AppVidaSana.Api;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.ProducesResponseType.Habits;
using AppVidaSana.Services.IServices.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits")]
    public class HabitsInfoGeneralController : ControllerBase
    {
        private readonly IHabitsGeneral _HabitsInfoService;

        public HabitsInfoGeneralController(IHabitsGeneral HabitsInfoService)
        {
            _HabitsInfoService = HabitsInfoService;
        }

        /// <summary>
        /// This controller contains all the information in the habits section and returns the hours of sleep in the last 7 days..
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The sleepDateHabit,drugDateHabit and drinkDateHabit property must have the following structure:   
        ///     {
        ///        "drinkDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY),
        ///        "sleepDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY),
        ///        "drugDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns all the information from the Habits section for a given day and the hours of sleep for the last 7 days.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnHabitsInfo))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetHabitsInfoGeneral([FromQuery] Guid accountID, [FromQuery] DateOnly date)
        {

            ReturnInfoHabitsDto info = _HabitsInfoService.GetInfoGeneralHabits(accountID, date);

            ReturnHabitsInfo response = new ReturnHabitsInfo
            {
                drinkConsumed = info.drinkConsumed,
                hoursSleepConsumed = info.hoursSleepConsumed,
                drugsConsumed = info.drugsConsumed,
                hoursSleep = info.hoursSleep,
                mfuStatus = info.mfuStatus
            };

            return StatusCode(StatusCodes.Status200OK, new
            {
                message = response.message,
                drinkConsumed = response.drinkConsumed,
                hoursSleepConsumed = response.hoursSleepConsumed,
                drugsConsumed = response.drugsConsumed,
                hoursSleep = response.hoursSleep,
                mfuStatus = response.mfuStatus
            });
        }
    }
}
