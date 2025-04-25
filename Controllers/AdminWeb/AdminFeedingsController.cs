using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Services.IServices.IAdminWeb;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType.AdminWeb.Feeding;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Services.IServices;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/admin/feedings")]
    [RequestTimeout("CustomPolicy")]
    public class AdminFeedingsController : ControllerBase
    {
        private readonly IAWFeeding _FeedingService;
        private readonly IExportToZip _ExportService;

        public AdminFeedingsController(IAWFeeding FeedingService, IExportToZip exportService)
        {
            _FeedingService = FeedingService;
            _ExportService = exportService;
        }

        /// <summary>
        /// This controller returns all information about a user's power supply.
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
        /// <response code="200">Returns information from the user's feed.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFeedingsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetFeedsOfAUserAsync([FromQuery] UserFeedFilterDto filter, [FromQuery] int page)
        {
            try 
            {
                var feedings = await _FeedingService.GetAllFeedsOfAUserAsync(filter, page, HttpContext.RequestAborted);

                GetFeedingsResponse response = new GetFeedingsResponse
                {
                    feedings = feedings
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, feedings = response.feedings });
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
        /// This controller returns all food consumed by the user.
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
        /// <response code="200">Returns information from the food consumed by the user.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFoodsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("foods")]
        [Produces("application/json")]
        public async Task<IActionResult> GetFoodsConsumedPerUserFeedAsync([FromQuery] UserFeedFilterDto filter, [FromQuery] int page)
        {
            try 
            { 
                var foods = await _FeedingService.GetAllFoodsConsumedPerUserFeedAsync(filter, page, HttpContext.RequestAborted);

                GetFoodsResponse response = new GetFoodsResponse
                {
                    foods = foods
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, foods = response.foods });
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCalConsumedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("calories-consumed-per-day")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCaloriesConsumedPerUserAsync([FromQuery] CaloriesConsumedFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var calConsumed = await _FeedingService.GetAllCaloriesConsumedPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetCalConsumedResponse response = new GetCalConsumedResponse
                {
                    calConsumed = calConsumed
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, calConsumed = response.calConsumed });
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCalRequiredResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("calories-required-per-days")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCaloriesRequiredPerDaysAsync([FromQuery] CaloriesRequiredPerDaysFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var calRequired = await _FeedingService.GetAllCaloriesRequiredPerDaysAsync(filter, page, HttpContext.RequestAborted);

                GetCalRequiredResponse response = new GetCalRequiredResponse
                {
                    calRequired = calRequired
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, calRequired = response.calRequired });
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserCaloriesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("calories-needed-per-user")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserCaloriesAsync([FromQuery] PatientFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var userCal = await _FeedingService.GetAllUserCaloriesAsync(filter, page, HttpContext.RequestAborted);

                GetUserCaloriesResponse response = new GetUserCaloriesResponse
                {
                    userCal = userCal
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, calNeeded = response.userCal });
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMFUsFeedingResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("mfu-feeding")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMFUsFeedingAsync([FromQuery] PatientFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var mfu = await _FeedingService.GetMFUsFeedingAsync(filter, page, HttpContext.RequestAborted);

                GetMFUsFeedingResponse response = new GetMFUsFeedingResponse
                {
                    mfu = mfu
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, mfu = response.mfu});
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
        [HttpGet("export-feedings")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyFeedsOfAUserToCsvAsync([FromQuery] string typeExport, [FromQuery] UserFeedFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"InfoFeedings_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyFeedingsZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_InfoFeedings_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyFeedingsZipAsync(null, typeExport, HttpContext.RequestAborted);
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
        [HttpGet("export-foods")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyFoodsConsumedPerFeedingToCsvAsync([FromQuery] string typeExport, [FromQuery] UserFeedFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"FoodsConsumedPerFeedingPerPatient_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyFoodsConsumedPerFeedingZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_FoodsConsumedPerFeedingPerPatient_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyFoodsConsumedPerFeedingZipAsync(null, typeExport, HttpContext.RequestAborted);
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
        [HttpGet("export-calories-consumed-per-day")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyCaloriesConsumedToCsvAsync([FromQuery] string typeExport, [FromQuery] CaloriesConsumedFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"TotalCaloriesConsumedPerPatientPerDay_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyCaloriesConsumedZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_TotalCaloriesConsumedPerPatientPerDay_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyCaloriesConsumedZipAsync(null, typeExport, HttpContext.RequestAborted);
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
        [HttpGet("export-calories-required-per-days")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyCaloriesRequiredPerDaysToCsvAsync([FromQuery] string typeExport, [FromQuery] CaloriesRequiredPerDaysFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"CaloriesRequiredPerDaysPerPatient_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyCaloriesRequiredPerDaysZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_CaloriesRequiredPerDaysPerPatient_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyCaloriesRequiredPerDaysZipAsync(null, typeExport, HttpContext.RequestAborted);
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
        [HttpGet("export-calories-needed-per-user")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyUserCaloriesToCsvAsync([FromQuery] string typeExport, [FromQuery] PatientFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"CaloriesRequiredPerPatient_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyUserCaloriesZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_CaloriesRequiredPerPatient_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyUserCaloriesZipAsync(null, typeExport, HttpContext.RequestAborted);
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
        [HttpGet("export-mfu-feeding")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyMFUsFeedingToCsvAsync([FromQuery] string typeExport, [FromQuery] PatientFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"MFUsFeeding_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsFeedingZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_MFUsFeeding_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsFeedingZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }
    }
}
