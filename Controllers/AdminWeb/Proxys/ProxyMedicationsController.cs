using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
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
        private const string apiKeyHeaderName = "ApiKeyHeaderName";
        private const string apiKey = "API_KEY";
        private const string bearerScheme = "Bearer";
        private const string typeArchiveJson = "application/json";
        private const string typeArchiveZip = "application/zip";
        private const string defaultNameZip = "default.zip";

        public ProxyMedicationsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyInfoMedicationsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PeriodMedicationsFilterDto filter, [FromQuery] int page)
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

                var response = await client.GetAsync($"https://{api}/api/admin/medications/export-periods-medications?{queryString}");

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

                var response = await client.GetAsync($"https://{api}/api/admin/medications/info-medications?{queryString}");

                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return this.HandleErrorResponse(response, responseBody);
                }

                return Content(responseBody, typeArchiveJson);
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateMedicationUseDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

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

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchiveJson);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeleteMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid periodID, [FromQuery] DateOnly date)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var response = await client.DeleteAsync($"https://{api}/api/medication?periodID={periodID}&date={date.ToString(formatDate, CultureInfo.InvariantCulture)}");

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchiveJson);
        }

        [HttpGet("side-effects")]
        public async Task<IActionResult> ProxySideEffectsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] SideEffectsFilterDto filter, [FromQuery] int page)
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

            if (filter.startDate != null)
                queryParams.Add($"startDate={filter.startDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            if (filter.endDate != null)
                queryParams.Add($"endDate={filter.endDate?.ToString(formatDate, CultureInfo.InvariantCulture)}");

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var response = await client.GetAsync($"https://{api}/api/admin/medications/export-side-effects?{queryString}");

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

                var response = await client.GetAsync($"https://{api}/api/admin/medications/side-effects?{queryString}");

                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return this.HandleErrorResponse(response, responseBody);
                }

                return Content(responseBody, typeArchiveJson);
            }
        }

        [HttpPut("side-effects/edit")]
        public async Task<IActionResult> ProxyEditSideEffectsAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] SideEffectsListDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

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

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchiveJson);
        }

        [HttpDelete("side-effects/delete")]
        public async Task<IActionResult> ProxyDeleteSideEffectsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid sideEffectID)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var response = await client.DeleteAsync($"https://{api}/api/medication/side-effects?sideEffectID={sideEffectID}");

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchiveJson);
        }

        [HttpGet("mfu-medication")]
        public async Task<IActionResult> ProxyMFUsMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] MFUsMedicationFilterDto filter, [FromQuery] int page)
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

            if (!string.IsNullOrEmpty(filter.statusAdherence))
                queryParams.Add($"statusAdherence={filter.statusAdherence}");


            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var response = await client.GetAsync($"https://{api}/api/admin/medications/export-mfu-medication?{queryString}");

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

                var response = await client.GetAsync($"https://{api}/api/admin/medications/mfu-medication?{queryString}");

                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return this.HandleErrorResponse(response, responseBody);
                }

                return Content(responseBody, typeArchiveJson);
            }
        }

        [HttpPut("mfu-medication/edit")]
        public async Task<IActionResult> ProxyEditMFUsMedicationAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateResponsesMedicationsDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var url = $"https://{api}/api/monthly-medications-monitoring";

            var response = await client.PutAsJsonAsync(url, values);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchiveJson);
        }
    }
}
