using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("proxy/admin")]
    [RequestTimeout("CustomPolicy")]
    public class ProxyAuth : ControllerBase
    {

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> ProxyLoginAsync([FromBody] LoginAdminDto login)
        {
            var client = new HttpClient();
            var api = Environment.GetEnvironmentVariable("API_AZURE");
            client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

            var response = await client.PostAsJsonAsync($"https://{api}/api/admin/auth", login);

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

    }
}
