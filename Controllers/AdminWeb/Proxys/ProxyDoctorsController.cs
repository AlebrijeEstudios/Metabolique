using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

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

        public ProxyDoctorsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet("all")]
        public async Task<IActionResult> ProxyListDoctorsAsync([FromHeader(Name = headerToken)] string authorization)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var response = await client.GetAsync($"https://{api}/api/doctors");

            return await this.HandleRegularRequestAsync(response);
        }

        [HttpGet]
        public async Task<IActionResult> ProxyDoctorsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var queryParams = this.BuildQueryParameters(filter, null, page);
            var queryString = string.Join("&", queryParams);

            var url = $"https://{api}/api/admin/doctors";

            return await this.GetHandleRegularRequestAsync(client, url, queryString);
        }

        [HttpPost]
        public async Task<IActionResult> ProxyCreateDoctorAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] AWDoctorDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/admin/doctors";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "POST", url, values);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditDoctorAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] AllDoctorsDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/admin/doctors";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeleteDoctorAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid doctorID)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/admin/doctors/{doctorID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }
    }
}