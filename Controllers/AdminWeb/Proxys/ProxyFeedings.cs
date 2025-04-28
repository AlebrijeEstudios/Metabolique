using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("proxy/admin/feedings")]
    [RequestTimeout("CustomPolicy")]
    public class ProxyFeedings : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProxyFeedings(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyFeedingsAsync([FromQuery] string? typeExport, [FromQuery] UserFeedFilterDto filter, [FromQuery] int page)
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

            if (!string.IsNullOrEmpty(filter.dailyMeal))
                queryParams.Add($"dailyMeal={filter.dailyMeal}");

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate}");

            var queryString = "";
            var response = new HttpResponseMessage();

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/feedings/export-feedings?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/feedings?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpGet("foods")]
        public async Task<IActionResult> ProxyFoodsAsync([FromQuery] string? typeExport, [FromQuery] UserFeedFilterDto filter, [FromQuery] int page)
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

            if (!string.IsNullOrEmpty(filter.dailyMeal))
                queryParams.Add($"dailyMeal={filter.dailyMeal}");

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate}");

            var queryString = "";
            var response = new HttpResponseMessage();

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/feedings/export-foods?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/feedings/foods?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpGet("calories-consumed-per-day")]
        public async Task<IActionResult> ProxyCaloriesConsumedPerUserAsync([FromQuery] string? typeExport, [FromQuery] CaloriesConsumedFilterDto filter, [FromQuery] int page)
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

            if (!string.IsNullOrEmpty(filter.sex))
                queryParams.Add($"sex={filter.sex}");

            if (!string.IsNullOrEmpty(filter.protocolToFollow))
                queryParams.Add($"protocolToFollow={filter.protocolToFollow}");

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate}");

            var queryString = "";
            var response = new HttpResponseMessage();

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/feedings/export-calories-consumed-per-day?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/feedings/calories-consumed-per-day?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpGet("calories-required-per-days")]
        public async Task<IActionResult> ProxyCaloriesRequiredPerDaysAsync([FromQuery] string? typeExport, [FromQuery] CaloriesRequiredPerDaysFilterDto filter, [FromQuery] int page)
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

            if (!string.IsNullOrEmpty(filter.sex))
                queryParams.Add($"sex={filter.sex}");

            if (!string.IsNullOrEmpty(filter.protocolToFollow))
                queryParams.Add($"protocolToFollow={filter.protocolToFollow}");

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate}");

            var queryString = "";
            var response = new HttpResponseMessage();

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/feedings/export-calories-required-per-days?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/feedings/calories-required-per-days?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpGet("calories-needed-per-user")]
        public async Task<IActionResult> ProxyUserCaloriesAsync([FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
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

                response = await client.GetAsync($"https://{api}/api/admin/feedings/export-calories-needed-per-user?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/feedings/calories-needed-per-user?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpGet("mfu-feeding")]
        public async Task<IActionResult> ProxyMFUsFeedingAsync([FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
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

                response = await client.GetAsync($"https://{api}/api/admin/feedings/export-mfu-feeding?{queryString}");

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

                response = await client.GetAsync($"https://{api}/api/admin/feedings/mfu-feeding?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }
    }
}
