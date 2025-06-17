using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Auth")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin")]
    [RequestTimeout("CustomPolicy")]
    public class ProxyAuthController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public ProxyAuthController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> ProxyLoginAsync([FromBody] LoginAdminDto login)
        {
            var client = _clientFactory.CreateClient();
            var api = Environment.GetEnvironmentVariable("SERVER");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var response = await client.PostAsJsonAsync($"https://{api}/api/admin/auth", login);

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

    }
}
