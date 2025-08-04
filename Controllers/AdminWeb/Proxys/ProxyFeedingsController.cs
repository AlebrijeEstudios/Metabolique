using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Feedings")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/feedings")]
    public class ProxyFeedingsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string formatDate = "yyyy-MM-dd";
        private const string headerToken = "Authorization";
        private const string apiUrl = "SERVER";
        private const string typeArchiveJson = "application/json";

        public ProxyFeedingsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyFeedingsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] UserFeedFilterDto filter, [FromQuery] int page)
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

                var url = $"https://{api}/api/admin/feedings/export-feedings";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/feedings";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateFeedingDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

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

            return await this.HandleRegularRequestAsync(response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeleteFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid userFeedID)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/feeding/{userFeedID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }

        [HttpGet("foods")]
        public async Task<IActionResult> ProxyFoodsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] UserFeedFilterDto filter, [FromQuery] int page)
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

                var url = $"https://{api}/api/admin/feedings/export-foods";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/feedings/foods";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpGet("mfu-feeding")]
        public async Task<IActionResult> ProxyMFUsFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
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

                var url = $"https://{api}/api/admin/feedings/export-mfu-feeding";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/feedings/mfu-feeding";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpPut("mfu-feeding/edit")]
        public async Task<IActionResult> ProxyEditMFUsFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateAnswersMFUsFoodDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/monthly-food-monitoring";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }

        [HttpGet("calories-needed-per-user")]
        public async Task<IActionResult> ProxyUserCaloriesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
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

                var url = $"https://{api}/api/admin/feedings/export-calories-needed-per-user";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/feedings/calories-needed-per-user";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpGet("calories-consumed-per-day")]
        public async Task<IActionResult> ProxyCaloriesConsumedPerUserAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] CaloriesConsumedFilterDto filter, [FromQuery] int page)
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

                var url = $"https://{api}/api/admin/feedings/export-calories-consumed-per-day";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/feedings/calories-consumed-per-day";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpGet("calories-required-per-days")]
        public async Task<IActionResult> ProxyCaloriesRequiredPerDaysAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] CaloriesRequiredPerDaysFilterDto filter, [FromQuery] int page)
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

                var url = $"https://{api}/api/admin/feedings/export-calories-required-per-days";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/feedings/calories-required-per-days";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }
    }
}