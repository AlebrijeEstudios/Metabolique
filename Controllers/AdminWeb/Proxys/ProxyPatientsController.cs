using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        private readonly string api = Environment.GetEnvironmentVariable(apiUrl)!;

        public ProxyPatientsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyPatientsAsync([FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentDoctorID = User.FindFirst("doctorID")?.Value;

            if (currentRole != "Admin")
            {
                filter.doctorID = Guid.Parse(currentDoctorID!);
            }

            var client = this.ConfigureHttpClient(_clientFactory);

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
        public async Task<IActionResult> ProxyEditPatientAsync([FromBody] InfoAccountDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory);

            var url = $"https://{api}/api/accounts";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeletePatientAsync([FromQuery] Guid accountID)
        {
            var client = this.ConfigureHttpClient(_clientFactory);

            var url = $"https://{api}/api/accounts/{accountID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }
    }
}