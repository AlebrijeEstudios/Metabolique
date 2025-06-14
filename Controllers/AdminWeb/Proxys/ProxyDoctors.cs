using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos;
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
    [Tags("Proxy - Doctors")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/doctors")]
    [RequestTimeout("CustomPolicy")]
    public class ProxyDoctors : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProxyDoctors(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyDoctorsAsync([FromQuery] DoctorFilterDto filter, [FromQuery] int page)
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

            if (!string.IsNullOrEmpty(filter.role))
                queryParams.Add($"role={filter.role}");

            var queryString = "";
            var response = new HttpResponseMessage();

            queryParams.Add($"page={page}");
            queryString = string.Join("&", queryParams);

            response = await client.GetAsync($"https://{api}/api/admin/doctors?{queryString}");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> ProxyCreateDoctorAsync([FromBody] AWDoctorDto values)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var response = await client.PostAsJsonAsync($"https://{api}/api/admin/doctors", values);

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditDoctorAsync([FromBody] AllDoctorsDto values)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var url = $"https://{api}/api/admin/doctors";

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
        public async Task<IActionResult> ProxyDeleteDoctorAsync([FromQuery] Guid doctorID)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var response = await client.DeleteAsync($"https://{api}/api/admin/doctors/{doctorID}");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
