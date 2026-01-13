using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

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
        private readonly string api = Environment.GetEnvironmentVariable(apiUrl)!;

        public ProxyFeedingsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyFeedingsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/feedings/export-feedings";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/feedings";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateFeedingDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

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

            var url = $"https://{api}/api/feeding/{userFeedID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }

        [HttpGet("foods")]
        public async Task<IActionResult> ProxyFoodsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/feedings/export-foods";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/feedings/foods";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpGet("mfu-feeding")]
        public async Task<IActionResult> ProxyMFUsFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/feedings/export-mfu-feeding";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/feedings/mfu-feeding";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPut("mfu-feeding/edit")]
        public async Task<IActionResult> ProxyEditMFUsFeedingAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateAnswersMFUsFoodDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/monthly-food-monitoring";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }

        [HttpGet("calories-needed-per-user")]
        public async Task<IActionResult> ProxyUserCaloriesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/feedings/export-calories-needed-per-user";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/feedings/calories-needed-per-user";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpGet("calories-consumed-per-day")]
        public async Task<IActionResult> ProxyCaloriesConsumedPerUserAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/feedings/export-calories-consumed-per-day";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/feedings/calories-consumed-per-day";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpGet("calories-required-per-days")]
        public async Task<IActionResult> ProxyCaloriesRequiredPerDaysAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/feedings/export-calories-required-per-days";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/feedings/calories-required-per-days";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }
    }
}