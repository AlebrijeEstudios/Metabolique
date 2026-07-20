using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.ProducesResponseType.AdminWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

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

            var response = await client.PostAsJsonAsync(url, login);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return this.HandleErrorResponse(response, responseBody);
            }

            var parsed = JsonSerializer.Deserialize<GetAuthResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (parsed?.auth == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Respuesta inválida del servidor" });
            }

            Response.Cookies.Append("accessToken", parsed.auth.accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(2) // igualado a la duración real del JWT
            });

            return Ok(new { message = parsed.message, role = parsed.auth.role });
        }


        [AllowAnonymous]
        [HttpPost("logout")]
        public IActionResult ProxyLogout()
        {
            var token = Request.Cookies["accessToken"];

            Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Sesión cerrada" });
        }

        [AllowAnonymous]
        [HttpGet("me")]
        public IActionResult ProxyMeAsync()
        {
            var token = Request.Cookies["accessToken"]; // esto es tu isset($_COOKIE['accessToken'])
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }
            return Ok();
        }
    }
}
