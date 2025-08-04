using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Auth")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin")]
    public class ProxyAuthController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string apiUrl = "SERVER";
        private const string apiKeyHeaderName = "ApiKeyHeaderName";
        private const string apiKey = "API_KEY";

        public ProxyAuthController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> ProxyLoginAsync([FromBody] LoginAdminDto login)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable(apiUrl);
            client.DefaultRequestHeaders.Add(Environment.GetEnvironmentVariable(apiKeyHeaderName)!, Environment.GetEnvironmentVariable(apiKey));

            var url = $"https://{api}/api/admin/auth";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "POST", url, login);
        }
    }
}
