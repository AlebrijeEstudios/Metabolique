using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Doctors")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/doctors")]
    public class ProxyDoctorsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string headerToken = "Authorization";
        private const string apiUrl = "SERVER";
        private const string apiKeyHeaderName = "ApiKeyHeaderName";
        private const string apiKey = "API_KEY";
        private const string bearerScheme = "Bearer";
        private const string typeArchive = "application/json";

        public ProxyDoctorsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet("all")]
        public async Task<IActionResult> ProxyListDoctorsAsync([FromHeader(Name = headerToken)] string authorization)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var response = await client.GetAsync($"https://{api}/api/doctors");

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchive);
        }

        [HttpGet]
        public async Task<IActionResult> ProxyDoctorsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] DoctorFilterDto filter, [FromQuery] int page)
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

            if (!string.IsNullOrEmpty(filter.role))
                queryParams.Add($"role={filter.role}");

            var queryString = "";

            queryParams.Add($"page={page}");
            queryString = string.Join("&", queryParams);

            var response = await client.GetAsync($"https://{api}/api/admin/doctors?{queryString}");

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchive);
        }

        [HttpPost]
        public async Task<IActionResult> ProxyCreateDoctorAsync([FromBody] AWDoctorDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            var response = await client.PostAsJsonAsync($"https://{api}/api/admin/doctors", values);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchive);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditDoctorAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] AllDoctorsDto values)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var url = $"https://{api}/api/admin/doctors";

            var response = await client.PutAsJsonAsync(url, values);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchive);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeleteDoctorAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid doctorID)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            var response = await client.DeleteAsync($"https://{api}/api/admin/doctors/{doctorID}");

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            return Content(responseBody, typeArchive);
        }
    }
}
