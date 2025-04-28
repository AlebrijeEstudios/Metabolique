using AppVidaSana.Services.IServices.IAdminWeb;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.AdminWeb.Exercise;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/admin/exercises")]
    [RequestTimeout("CustomPolicy")]
    public class AdminExerciseController : ControllerBase
    {
        private readonly IAWExercise _ExerciseService;
        private readonly IExportToZip _ExportService;

        public AdminExerciseController(IAWExercise ExerciseService, IExportToZip exportService)
        {
            _ExerciseService = ExerciseService;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetExercisesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetExercisesPerUserAsync([FromQuery] ExerciseFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var exs = await _ExerciseService.GetAllExercisesPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetExercisesResponse response = new GetExercisesResponse
                {
                    exercises = exs
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, exercises = response.exercises });
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetActiveMinutesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("active-minutes")]
        [Produces("application/json")]
        public async Task<IActionResult> GetActiveMinutesPerExerciseAsync([FromQuery] ActiveMinutesFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var actM = await _ExerciseService.GetAllActiveMinutesPerExerciseAsync(filter, page, HttpContext.RequestAborted);

                GetActiveMinutesResponse response = new GetActiveMinutesResponse
                {
                    actMin = actM
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, actMin = response.actMin });
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMFUsExerciseResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("mfu-exercise")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMFUsExerciseAsync([FromQuery] PatientFilterDto filter, [FromQuery] int page)
        {
            try
            {
                var mfu = await _ExerciseService.GetMFUsExerciseAsync(filter, page, HttpContext.RequestAborted);

                GetMFUsExerciseResponse response = new GetMFUsExerciseResponse
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
        [HttpGet("export-exercises")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyExercisesToCsvAsync([FromQuery] string typeExport, [FromQuery] ExerciseFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"Exercises_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyExercisesZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_Exercises_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyExercisesZipAsync(null, typeExport, HttpContext.RequestAborted);
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
        [HttpGet("export-active-minutes")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyActivesMinutesToCsvAsync([FromQuery] string typeExport, [FromQuery] ActiveMinutesFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"ActivesMinutes_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyActivesMinutesZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_ActivesMinutes_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyActivesMinutesZipAsync(null, typeExport, HttpContext.RequestAborted);
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
        [HttpGet("export-mfu-exercise")]
        [Produces("application/zip")]
        public async Task<IActionResult> ExportOnlyMFUsExerciseToCsvAsync([FromQuery] string typeExport, [FromQuery] PatientFilterDto filter)
        {
            string fileName = "";
            string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
            byte[] zipBytes = [];

            if (typeExport == "with_filter")
            {
                fileName = $"MFUsExercise_With_Filters_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsExerciseZipAsync(filter, typeExport, HttpContext.RequestAborted);
            }

            if (typeExport == "all")
            {
                fileName = $"All_MFUsExercise_{dateSuffix}.zip";
                zipBytes = await _ExportService.GenerateOnlyMFUsExerciseZipAsync(null, typeExport, HttpContext.RequestAborted);
            }

            return File(zipBytes, "application/zip", fileName);
        }
    }
}
