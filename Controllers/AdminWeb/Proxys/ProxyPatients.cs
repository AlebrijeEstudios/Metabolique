using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
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
    [Route("proxy/admin/patients")]
    [RequestTimeout("CustomPolicy")]
    public class ProxyPatients : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProxyPatients(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyPatientsAsync([FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
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

                response = await client.GetAsync($"https://{api}/api/admin/patients/export-patients?{queryString}");

                var content = await response.Content.ReadAsByteArrayAsync();
                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = contentDisposition?.FileName ?? "default.zip";

                return new FileContentResult(content, "application/zip")
                {
                    FileDownloadName = fileName
                };
            }
            else { 
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                response = await client.GetAsync($"https://{api}/api/admin/patients?{queryString}");

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");

            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditPatientAsync([FromBody] InfoAccountDto values)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var url = $"https://{api}/api/accounts";
            Console.WriteLine($"URL: {url}");

            var response = await client.PutAsJsonAsync(url, values);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    error = "Error al llamar a la API remota",
                    status = response.StatusCode,
                    content = responseBody
                });
            }

            return Content(responseBody, "application/json");


        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeletePatientAsync([FromQuery] Guid accountID)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var response = await client.DeleteAsync($"https://{api}/api/accounts/{accountID}");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
