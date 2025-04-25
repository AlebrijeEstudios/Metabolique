using AppVidaSana.Services.IServices.IAdminWeb;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.ProducesResponseType.AdminWeb.Habit;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/admin/habits")]
    [RequestTimeout("CustomPolicy")]
    public class AdminHabitsController : ControllerBase
    {
        private readonly IAWHabits _HabitService;
        private readonly IExportToZip _ExportService;

        public AdminHabitsController(IAWHabits HabitService, IExportToZip exportService)
        {
            _HabitService = HabitService;
            _ExportService = exportService;
        }

        /// <summary>
        /// PENDIENTE
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHabitDrinkResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("drink")]
        [Produces("application/json")]
        public async Task<IActionResult> GetHabitsDrinkPerUserAsync([FromQuery] HabitDrinkFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var hDrink = await _HabitService.GetAllHabitsDrinkPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetHabitDrinkResponse response = new GetHabitDrinkResponse
                {
                    hDrink = hDrink
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, hDrink = response.hDrink });
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
        /// PENDIENTE
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHabitDrugsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("drugs")]
        [Produces("application/json")]
        public async Task<IActionResult> GetHabitsDrugsPerUserAsync([FromQuery] HabitDrugFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var hDrugs = await _HabitService.GetAllHabitsDrugsPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetHabitDrugsResponse response = new GetHabitDrugsResponse
                {
                    hDrugs = hDrugs
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, hDrugs = response.hDrugs });
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
        /// PENDIENTE
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHabitSleepResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("sleep")]
        [Produces("application/json")]
        public async Task<IActionResult> GetHabitsSleepPerUserAsync([FromQuery] HabitSleepFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var hSleep = await _HabitService.GetAllHabitsSleepPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetHabitSleepResponse response = new GetHabitSleepResponse
                {
                    hSleep = hSleep
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, hSleep = response.hSleep });
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
        /// PENDIENTE
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMFUsHabitsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("mfu-habit")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMFUsHabitsAsync([FromQuery] PatientFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var mfu = await _HabitService.GetMFUsHabitsAsync(filter, page, HttpContext.RequestAborted);

                GetMFUsHabitsResponse response = new GetMFUsHabitsResponse
                {
                    mfu = mfu
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, mfu = response.mfu });
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
        /// This driver exports in csv records.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("export-habits-drink")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyHabitsDrinkToCsvAsync([FromQuery] string typeExport, [FromQuery] HabitDrinkFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"HabitsDrink_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrinkZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_HabitsDrink_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrinkZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }

        /// <summary>
        /// This driver exports in csv records.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("export-habits-drugs")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyHabitsDrugsToCsvAsync([FromQuery] string typeExport, [FromQuery] HabitDrugFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"HabitsDrugs_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrugsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_HabitsDrugs_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsDrugsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }

        /// <summary>
        /// This driver exports in csv records.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("export-habits-sleep")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyHabitsSleepToCsvAsync([FromQuery] string typeExport, [FromQuery] HabitSleepFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"HabitsSleep_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsSleepZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_HabitsSleep_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyHabitsSleepZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }

        /// <summary>
        /// This driver exports in csv records.
        /// </summary>
        /// <response code="200">Returns information succesfully.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("export-mfu-habit")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyMFUsHabitsToCsvAsync([FromQuery] string typeExport, [FromQuery] PatientFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"MFUsHabits_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsHabitsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_MFUsHabits_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsHabitsZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }

    }
}
