using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
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

        public ProxyPatientsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyPatientsAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] PatientFilterDto filter, [FromQuery] int page)
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

            var queryString = "";

            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/patients/export-patients";

                return await this.HandleExportRequestAsync(client, url, queryString);
            }
            else { 
                queryParams.Add($"page={page}");
                queryString = string.Join("&", queryParams);

                var url = $"https://{api}/api/admin/patients";

                return await this.GetHandleRegularRequestAsync(client, url, queryString);
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditPatientAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] InfoAccountDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/accounts";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeletePatientAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid accountID)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);
            var api = Environment.GetEnvironmentVariable(apiUrl);

            var url = $"https://{api}/api/accounts/{accountID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }
    }
}