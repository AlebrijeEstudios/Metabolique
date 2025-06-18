using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Models.Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http.Headers;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Exercises")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/exercises")]
    [RequestTimeout("CustomPolicy")]
    public class ProxyExercisesController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string headerToken = "Authorization";
        private const string apiUrl = "SERVER";
        private const string apiKeyHeaderName = "Metabolique_API_KEY";
        private const string apiKey = "API_KEY";
        private const string bearerScheme = "Bearer";
        private const string typeArchiveJson = "application/json";
        private const string typeArchiveZip = "application/zip";
        private const string defaultNameZip = "default.zip";
        private const string messageError = "Error al llamar a la API remota.";

        public ProxyExercisesController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyExercisesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] ExerciseFilterDto filter, [FromQuery] int page)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(apiKeyHeaderName, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

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
                queryParams.Add($"startDate={filter.startDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var response = await client.GetAsync($"https://{api}/api/admin/exercises/export-exercises?{queryString}");

                var content = await response.Content.ReadAsByteArrayAsync();
                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = contentDisposition?.FileName ?? defaultNameZip;

                return new FileContentResult(content, typeArchiveZip)
                {
                    FileDownloadName = fileName
                };
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var response = await client.GetAsync($"https://{api}/api/admin/exercises?{queryString}");

                Console.WriteLine($"URL: {$"https://{api}/api/admin/exercises?{queryString}"}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, typeArchiveJson);

            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditExerciseAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] ExerciseDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(apiKeyHeaderName, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var url = $"https://{api}/api/exercises";

            var response = await client.PutAsJsonAsync(url, values);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    error = messageError,
                    status = response.StatusCode,
                    content = responseBody
                });
            }

            return Content(responseBody, typeArchiveJson);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeleteExerciseAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid exerciseID)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(apiKeyHeaderName, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var response = await client.DeleteAsync($"https://{api}/api/exercises/{exerciseID}");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, typeArchiveJson);
        }

        [HttpGet("mfu-exercise")]
        public async Task<IActionResult> ProxyMFUsExercisesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(apiKeyHeaderName, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

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

                var response = await client.GetAsync($"https://{api}/api/admin/exercises/export-mfu-exercise?{queryString}");

                var content = await response.Content.ReadAsByteArrayAsync();
                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = contentDisposition?.FileName ?? defaultNameZip;

                return new FileContentResult(content, typeArchiveZip)
                {
                    FileDownloadName = fileName
                };
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var response = await client.GetAsync($"https://{api}/api/admin/exercises/mfu-exercise?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, typeArchiveJson);

            }
        }

        [HttpPut("mfu-exercise/edit")]
        public async Task<IActionResult> ProxyEditMFUsExercisesAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateResponsesExerciseDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(apiKeyHeaderName, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var url = $"https://{api}/api/monthly-exercise-monitoring";

            var response = await client.PutAsJsonAsync(url, values);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    error = messageError,
                    status = response.StatusCode,
                    content = responseBody
                });
            }

            return Content(responseBody, typeArchiveJson);
        }
    
        /*[HttpGet("active-minutes")]
        public async Task<IActionResult> ProxyActiveMinutesAsync([FromQuery] string? typeExport, [FromQuery] ActiveMinutesFilterDto filter, [FromQuery] int page)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

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
                queryParams.Add($"startDate={filter.startDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");

            var queryString = "";
            var response = new HttpResponseMessage();

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/exercises/export-active-minutes?{queryString}");

                var content = await response.Content.ReadAsByteArrayAsync();
                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = contentDisposition?.FileName ?? "default.zip";

                return new FileContentResult(content, "application/zip")
                {
                    FileDownloadName = fileName
                };
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/exercises/active-minutes?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }*/
    }
}
