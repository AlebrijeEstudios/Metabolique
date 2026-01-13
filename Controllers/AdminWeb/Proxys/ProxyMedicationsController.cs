using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Medications")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/medications")]
    public class ProxyMedicationsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string formatDate = "yyyy-MM-dd";
        private const string headerToken = "Authorization";
        private const string apiUrl = "SERVER";
        private const string typeArchiveJson = "application/json";
        private readonly string api = Environment.GetEnvironmentVariable(apiUrl)!;

        public ProxyMedicationsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyInfoMedicationsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/medications/export-periods-medications";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/medications/info-medications";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateMedicationUseDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/medication";
            var dtoToSend = new
            {
                periodID = values.periodID,
                updateDate = values.updateDate.ToString(formatDate),
                nameMedication = values.nameMedication,
                dose = values.dose,
                initialFrec = values.initialFrec.ToString(formatDate),
                finalFrec = values.finalFrec.ToString(formatDate),
                newTimes = values.newTimes,
                times = values.times.Select(t => new
                {
                    timeID = t.timeID,
                    periodID = t.periodID,
                    dateMedication = t.dateMedication.ToString(formatDate),
                    time = t.time.ToString("HH:mm"),
                    medicationStatus = t.medicationStatus
                }).ToList()
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
        public async Task<IActionResult> ProxyDeleteMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid periodID, [FromQuery] DateOnly date)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/medication?periodID={periodID}&date={date.ToString(formatDate, CultureInfo.InvariantCulture)}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }

        [HttpGet("side-effects")]
        public async Task<IActionResult> ProxySideEffectsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/medications/export-side-effects";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/medications/side-effects";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPut("side-effects/edit")]
        public async Task<IActionResult> ProxyEditSideEffectsAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] SideEffectsListDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/medication/side-effects";

            var dtoToSend = new
            {
                sideEffectID = values.sideEffectID,
                initialTime = values.initialTime.ToString("HH:mm"),
                finalTime = values.finalTime.ToString("HH:mm"),
                description = values.description
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

        [HttpDelete("side-effects/delete")]
        public async Task<IActionResult> ProxyDeleteSideEffectsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid sideEffectID)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/medication/side-effects?sideEffectID={sideEffectID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }

        [HttpGet("mfu-medication")]
        public async Task<IActionResult> ProxyMFUsMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/medications/export-mfu-medication";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/medications/mfu-medication";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPut("mfu-medication/edit")]
        public async Task<IActionResult> ProxyEditMFUsMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateResponsesMedicationsDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/monthly-medications-monitoring";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }
    }
}