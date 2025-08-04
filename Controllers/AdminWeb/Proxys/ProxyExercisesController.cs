using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Models.Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Exercises")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/exercises")]
    public class ProxyExercisesController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string formatDate = "yyyy-MM-dd";
        private const string headerToken = "Authorization";
        private const string apiUrl = "SERVER";

        public ProxyExercisesController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyExercisesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] ExerciseFilterDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(filter.doctorID.ToString()))
                queryParams.Add($"doctorID={filter.doctorID}");

            if (!string.IsNullOrEmpty(filter.accountID.ToString()))
                queryParams.Add($"accountID={filter.accountID}");

            if (!string.IsNullOrEmpty(filter.uiemID))
                queryParams.Add($"uiemID={filter.uiemID}");

            if (!string.IsNullOrEmpty(filter.username))
                queryParams.Add($"username={filter.username}");

            if (!string.IsNullOrEmpty(filter.month.ToString()))
                queryParams.Add($"month={filter.month}");

            if (!string.IsNullOrEmpty(filter.year.ToString()))
                queryParams.Add($"year={filter.year}");

            if (!string.IsNullOrEmpty(filter.sex))
                queryParams.Add($"sex={filter.sex}");

            if (!string.IsNullOrEmpty(filter.protocolToFollow))
                queryParams.Add($"protocolToFollow={filter.protocolToFollow}");

            if (!string.IsNullOrEmpty(filter.typeExercise))
                queryParams.Add($"typeExercise={filter.typeExercise}");

            if (!string.IsNullOrEmpty(filter.intensityExercise))
                queryParams.Add($"intensityExercise={filter.intensityExercise}");

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/exercises/export-exercises";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/exercises";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditExerciseAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] ExerciseDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/exercises";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeleteExerciseAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid exerciseID)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/exercises/{exerciseID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }

        [HttpGet("mfu-exercise")]
        public async Task<IActionResult> ProxyMFUsExercisesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(filter.doctorID.ToString()))
                queryParams.Add($"doctorID={filter.doctorID}");

            if (!string.IsNullOrEmpty(filter.accountID.ToString()))
                queryParams.Add($"accountID={filter.accountID}");

            if (!string.IsNullOrEmpty(filter.uiemID))
                queryParams.Add($"uiemID={filter.uiemID}");

            if (!string.IsNullOrEmpty(filter.username))
                queryParams.Add($"username={filter.username}");

            if (!string.IsNullOrEmpty(filter.month.ToString()))
                queryParams.Add($"month={filter.month}");

            if (!string.IsNullOrEmpty(filter.year.ToString()))
                queryParams.Add($"year={filter.year}");

            if (!string.IsNullOrEmpty(filter.sex))
                queryParams.Add($"sex={filter.sex}");

            if (!string.IsNullOrEmpty(filter.protocolToFollow))
                queryParams.Add($"protocolToFollow={filter.protocolToFollow}");

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/exercises/export-mfu-exercise";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/exercises/mfu-exercise";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpPut("mfu-exercise/edit")]
        public async Task<IActionResult> ProxyEditMFUsExercisesAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateResponsesExerciseDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/monthly-exercise-monitoring";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }
    
        [HttpGet("active-minutes")]
        public async Task<IActionResult> ProxyActiveMinutesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] ActiveMinutesFilterDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(filter.doctorID.ToString()))
                queryParams.Add($"doctorID={filter.doctorID}");

            if (!string.IsNullOrEmpty(filter.accountID.ToString()))
                queryParams.Add($"accountID={filter.accountID}");

            if (!string.IsNullOrEmpty(filter.uiemID))
                queryParams.Add($"uiemID={filter.uiemID}");

            if (!string.IsNullOrEmpty(filter.username))
                queryParams.Add($"username={filter.username}");

            if (!string.IsNullOrEmpty(filter.month.ToString()))
                queryParams.Add($"month={filter.month}");

            if (!string.IsNullOrEmpty(filter.year.ToString()))
                queryParams.Add($"year={filter.year}");

            if (!string.IsNullOrEmpty(filter.sex))
                queryParams.Add($"sex={filter.sex}");

            if (!string.IsNullOrEmpty(filter.protocolToFollow))
                queryParams.Add($"protocolToFollow={filter.protocolToFollow}");

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/exercises/export-active-minutes";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/exercises/active-minutes";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }
    }
}