using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.JsonPatch;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using System.Globalization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Habits")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/habits")]
    [RequestTimeout("CustomPolicy")]
    public class ProxyHabits : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProxyHabits(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("drink")]
        public async Task<IActionResult> ProxyHabitDrinkAsync([FromQuery] string? typeExport, [FromQuery] HabitDrinkFilterDto filter, [FromQuery] int page)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

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

                response = await client.GetAsync($"https://{api}/api/admin/habits/export-habits-drink?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/habits/drink?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpPatch("drink/edit")]
        public async Task<IActionResult> ProxyEditHabitDrinkAsync([FromQuery] Guid drinkHabitID, [FromBody] JsonPatchDocument values)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var url = $"https://{api}/api/habits-drink?drinkHabitID={drinkHabitID}";

            var json = JsonConvert.SerializeObject(values, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PatchAsync(url, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    error = "Error al llamar a la API remota",
                    status = response.StatusCode,
                    content = responseBody
                });
            }

            return Content(responseBody, "application/json");
        }

        [HttpGet("drugs")]
        public async Task<IActionResult> ProxyHabitDrugsAsync([FromQuery] string? typeExport, [FromQuery] HabitDrugFilterDto filter, [FromQuery] int page)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

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

            if (!string.IsNullOrEmpty(filter.predominatEmotionalState))
                queryParams.Add($"predominatEmotionalState={filter.predominatEmotionalState}");

            var queryString = "";
            var response = new HttpResponseMessage();

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/habits/export-habits-drugs?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/habits/drugs?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpPatch("drugs/edit")]
        public async Task<IActionResult> ProxyEditHabitDrugsAsync([FromQuery] Guid drugsHabitID, [FromBody] JsonPatchDocument values)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var url = $"https://{api}/api/habits-drugs?drugsHabitID={drugsHabitID}";

            var json = JsonConvert.SerializeObject(values, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PatchAsync(url, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    error = "Error al llamar a la API remota",
                    status = response.StatusCode,
                    content = responseBody
                });
            }

            return Content(responseBody, "application/json");
        }

        [HttpGet("sleep")]
        public async Task<IActionResult> ProxyHabitSleepAsync([FromQuery] string? typeExport, [FromQuery] HabitSleepFilterDto filter, [FromQuery] int page)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

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

            if (!string.IsNullOrEmpty(filter.perceptionRelax))
                queryParams.Add($"perceptionRelax={filter.perceptionRelax}");


            var queryString = "";
            var response = new HttpResponseMessage();

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/habits/export-habits-sleep?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/habits/sleep?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpPatch("sleep/edit")]
        public async Task<IActionResult> ProxyEditHabitSleepAsync([FromQuery] Guid sleepHabitID, [FromBody] JsonPatchDocument values)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var url = $"https://{api}/api/habits-sleep?sleepHabitID={sleepHabitID}";

            var json = JsonConvert.SerializeObject(values, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PatchAsync(url, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    error = "Error al llamar a la API remota",
                    status = response.StatusCode,
                    content = responseBody
                });
            }

            return Content(responseBody, "application/json");
        }

        [HttpGet("mfu-habit")]
        public async Task<IActionResult> ProxyMFUsHabitsAsync([FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

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


            var queryString = "";
            var response = new HttpResponseMessage();

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/habits/export-mfu-habit?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/habits/mfu-habit?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpPut("mfu-habit/edit")]
        public async Task<IActionResult> ProxyEditMFUsHabitsAsync([FromBody] UpdateResponsesHabitsDto values)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var url = $"https://{api}/api/monthly-habits-monitoring";
            Console.WriteLine($"URL: {url}");

            var response = await client.PutAsJsonAsync(url, values);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    error = "Error al llamar a la API remota",
                    status = response.StatusCode,
                    content = responseBody
                });
            }

            return Content(responseBody, "application/json");
        }
    }
}
