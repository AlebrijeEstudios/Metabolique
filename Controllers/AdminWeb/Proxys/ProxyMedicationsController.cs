using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;
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

        public ProxyMedicationsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyInfoMedicationsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PeriodMedicationsFilterDto filter, [FromQuery] int page)
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

            if (!string.IsNullOrEmpty(filter.nameMedication))
                queryParams.Add($"medication={filter.nameMedication}");

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            if (filter.status != null)
                queryParams.Add($"status={filter.status}");

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/medications/export-periods-medications";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/medications/info-medications";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateMedicationUseDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

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
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/medication?periodID={periodID}&date={date.ToString(formatDate, CultureInfo.InvariantCulture)}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }

        [HttpGet("side-effects")]
        public async Task<IActionResult> ProxySideEffectsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] SideEffectsFilterDto filter, [FromQuery] int page)
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

                var url = $"https://{api}/api/admin/medications/export-side-effects";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/medications/side-effects";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpPut("side-effects/edit")]
        public async Task<IActionResult> ProxyEditSideEffectsAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] SideEffectsListDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

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
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/medication/side-effects?sideEffectID={sideEffectID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }

        [HttpGet("mfu-medication")]
        public async Task<IActionResult> ProxyMFUsMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] MFUsMedicationFilterDto filter, [FromQuery] int page)
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

            if (!string.IsNullOrEmpty(filter.statusAdherence))
                queryParams.Add($"statusAdherence={filter.statusAdherence}");

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/medications/export-mfu-medication";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else
            {
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/medications/mfu-medication";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpPut("mfu-medication/edit")]
        public async Task<IActionResult> ProxyEditMFUsMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateResponsesMedicationsDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/monthly-medications-monitoring";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }
    }
}
