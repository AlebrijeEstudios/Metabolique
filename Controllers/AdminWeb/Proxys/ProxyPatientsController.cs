using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Patients")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/patients")]
    public class ProxyPatientsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string headerToken = "Authorization";
        private const string apiUrl = "SERVER";
        private string api = Environment.GetEnvironmentVariable(apiUrl)!;

        public ProxyPatientsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyPatientsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/patients/export-patients";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else 
            {
                var url = $"https://{api}/api/admin/patients";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditPatientAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] InfoAccountDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/accounts";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeletePatientAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid accountID)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/accounts/{accountID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }
    }
}