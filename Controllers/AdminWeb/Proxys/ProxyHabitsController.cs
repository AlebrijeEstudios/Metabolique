using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
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
    [Tags("Proxy - Habits")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/habits")]
    public class ProxyHabitsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string headerToken = "Authorization";
        private const string apiUrl = "SERVER";
        private const string typeArchiveJson = "application/json";
        private readonly string api = Environment.GetEnvironmentVariable(apiUrl)!;

        public ProxyHabitsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet("drink")]
        public async Task<IActionResult> ProxyHabitDrinkAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/habits/export-habits-drink";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/habits/drink";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPatch("drink/edit")]
        public async Task<IActionResult> ProxyEditHabitDrinkAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid drinkHabitID, [FromBody] JsonPatchDocument values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/habits-drink?drinkHabitID={drinkHabitID}";

            var json = JsonConvert.SerializeObject(values, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });

            var content = new StringContent(json, Encoding.UTF8, typeArchiveJson);

            var response = await client.PatchAsync(url, content);

            return await this.HandleRegularRequestAsync(response);
        }

        [HttpGet("drugs")]
        public async Task<IActionResult> ProxyHabitDrugsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/habits/export-habits-drugs";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/habits/drugs";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPatch("drugs/edit")]
        public async Task<IActionResult> ProxyEditHabitDrugsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid drugsHabitID, [FromBody] JsonPatchDocument values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/habits-drugs?drugsHabitID={drugsHabitID}";

            var json = JsonConvert.SerializeObject(values, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });

            var content = new StringContent(json, Encoding.UTF8, typeArchiveJson);

            var response = await client.PatchAsync(url, content);

            return await this.HandleRegularRequestAsync(response);
        }

        [HttpGet("sleep")]
        public async Task<IActionResult> ProxyHabitSleepAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/habits/export-habits-sleep";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/habits/sleep";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPatch("sleep/edit")]
        public async Task<IActionResult> ProxyEditHabitSleepAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid sleepHabitID, [FromBody] JsonPatchDocument values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/habits-sleep?sleepHabitID={sleepHabitID}";

            var json = JsonConvert.SerializeObject(values, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });

            var content = new StringContent(json, Encoding.UTF8, typeArchiveJson);

            var response = await client.PatchAsync(url, content);

            return await this.HandleRegularRequestAsync(response);
        }

        [HttpGet("mfu-habit")]
        public async Task<IActionResult> ProxyMFUsHabitsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/habits/export-mfu-habit";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/habits/mfu-habit";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPut("mfu-habit/edit")]
        public async Task<IActionResult> ProxyEditMFUsHabitsAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateResponsesHabitsDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/monthly-habits-monitoring";
            var dtoToSend = new
            {
                monthlyFollowUpID = values.monthlyFollowUpID,
                month = values.month,
                year = values.year,
                answerQuestion1 = values.answerQuestion1.ToString("HH:mm"),
                answerQuestion2 = values.answerQuestion2,
                answerQuestion3 = values.answerQuestion3.ToString("HH:mm"),
                answerQuestion4 = values.answerQuestion4,
                answerQuestion5a = values.answerQuestion5a,
                answerQuestion5b = values.answerQuestion5b,
                answerQuestion5c = values.answerQuestion5c,
                answerQuestion5d = values.answerQuestion5d,
                answerQuestion5e = values.answerQuestion5e,
                answerQuestion5f = values.answerQuestion5f,
                answerQuestion5g = values.answerQuestion5g,
                answerQuestion5h = values.answerQuestion5h,
                answerQuestion5i = values.answerQuestion5i,
                answerQuestion5j = values.answerQuestion5j,
                answerQuestion6 = values.answerQuestion6,
                answerQuestion7 = values.answerQuestion7,
                answerQuestion8 = values.answerQuestion8,
                answerQuestion9 = values.answerQuestion9,

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
    }
}