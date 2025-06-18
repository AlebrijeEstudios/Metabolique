using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Patients")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/patients")]
    [RequestTimeout("CustomPolicy")]
    public class ProxyPatientsController : ControllerBase
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

        public ProxyPatientsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyPatientsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
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

                var response = await client.GetAsync($"https://{api}/api/admin/patients/export-patients?{queryString}");

                var content = await response.Content.ReadAsByteArrayAsync();
                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = contentDisposition?.FileName ?? defaultNameZip;

                return new FileContentResult(content, typeArchiveZip)
                {
                    FileDownloadName = fileName
                };
            }
            else { 
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var response = await client.GetAsync($"https://{api}/api/admin/patients?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, typeArchiveJson);

            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditPatientAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] InfoAccountDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(apiKeyHeaderName, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var url = $"https://{api}/api/accounts";

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
        public async Task<IActionResult> ProxyDeletePatientAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid accountID)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(apiKeyHeaderName, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var response = await client.DeleteAsync($"https://{api}/api/accounts/{accountID}");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, typeArchiveJson);
        }
    }
}
