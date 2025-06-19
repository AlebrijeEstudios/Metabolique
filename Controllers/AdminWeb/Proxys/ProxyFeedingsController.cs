using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Feedings")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/feedings")]
    [RequestTimeout("CustomPolicy")]
    public class ProxyFeedingsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string formatDate = "yyyy-MM-dd";
        private const string headerToken = "Authorization";
        private const string apiUrl = "SERVER";
        private const string apiKeyHeaderName = "ApiKeyHeaderName";
        private const string apiKey = "API_KEY";
        private const string bearerScheme = "Bearer";
        private const string typeArchiveJson = "application/json";
        private const string typeArchiveZip = "application/zip";
        private const string defaultNameZip = "default.zip";
        private const string messageError = "Error al llamar a la API remota.";

        public ProxyFeedingsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyFeedingsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] UserFeedFilterDto filter, [FromQuery] int page)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

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

            if (!string.IsNullOrEmpty(filter.dailyMeal))
                queryParams.Add($"dailyMeal={filter.dailyMeal}");

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var response = await client.GetAsync($"https://{api}/api/admin/feedings/export-feedings?{queryString}");

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

                var response = await client.GetAsync($"https://{api}/api/admin/feedings?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, typeArchiveJson);

            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateFeedingDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var url = $"https://{api}/api/feeding";
            var dtoToSend = new
            {
                userFeedID = values.userFeedID,
                userFeedDate = values.userFeedDate.ToString(formatDate),
                userFeedTime = values.userFeedTime.ToString("HH:mm"),
                dailyMeal = values.dailyMeal,
                foodsConsumed = values.foodsConsumed,
                satietyLevel = values.satietyLevel,
                emotionsLinked = values.emotionsLinked,
                saucerPicture = values.saucerPicture
            };

            var json = JsonConvert.SerializeObject(dtoToSend, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });

            var content = new StringContent(json, Encoding.UTF8, typeArchiveJson);

            var response = await client.PutAsync(url, content);

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
        public async Task<IActionResult> ProxyDeleteFeediingAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid userFeedID)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var response = await client.DeleteAsync($"https://{api}/api/feeding/{userFeedID}");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, typeArchiveJson);
        }

        [HttpGet("foods")]
        public async Task<IActionResult> ProxyFoodsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] UserFeedFilterDto filter, [FromQuery] int page)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

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

            if (!string.IsNullOrEmpty(filter.dailyMeal))
                queryParams.Add($"dailyMeal={filter.dailyMeal}");

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var response = await client.GetAsync($"https://{api}/api/admin/feedings/export-foods?{queryString}");

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

                var response = await client.GetAsync($"https://{api}/api/admin/feedings/foods?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, typeArchiveJson);

            }
        }

        [HttpGet("mfu-feeding")]
        public async Task<IActionResult> ProxyMFUsFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

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

                var response = await client.GetAsync($"https://{api}/api/admin/feedings/export-mfu-feeding?{queryString}");

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

                var response = await client.GetAsync($"https://{api}/api/admin/feedings/mfu-feeding?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, typeArchiveJson);

            }
        }

        [HttpPut("mfu-feeding/edit")]
        public async Task<IActionResult> ProxyEditMFUsFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateAnswersMFUsFoodDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var url = $"https://{api}/api/monthly-food-monitoring";

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

        [HttpGet("calories-needed-per-user")]
        public async Task<IActionResult> ProxyUserCaloriesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

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

                var response = await client.GetAsync($"https://{api}/api/admin/feedings/export-calories-needed-per-user?{queryString}");

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

                var response = await client.GetAsync($"https://{api}/api/admin/feedings/calories-needed-per-user?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, typeArchiveJson);

            }
        }

        /*[HttpGet("calories-consumed-per-day")]
       public async Task<IActionResult> ProxyCaloriesConsumedPerUserAsync([FromQuery] string? typeExport, [FromQuery] CaloriesConsumedFilterDto filter, [FromQuery] int page)
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
       }*/
    }
}
