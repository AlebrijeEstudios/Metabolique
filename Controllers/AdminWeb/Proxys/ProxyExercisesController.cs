using AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.AdminWeb.Proxys
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Proxy - Exercises")]
    [ApiExplorerSettings(GroupName = "proxy")]
    [Route("proxy/admin/exercises")]
    public class ProxyExercisesController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string headerToken = "Authorization";
        private const string apiUrl = "SERVER";
        private string api = Environment.GetEnvironmentVariable(apiUrl)!;

        public ProxyExercisesController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProxyExercisesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/exercises/export-exercises";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/exercises";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> ProxyEditExerciseAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] ExerciseDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/exercises";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> ProxyDeleteExerciseAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] Guid exerciseID)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/exercises/{exerciseID}";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "DELETE", url, null);
        }

        [HttpGet("mfu-exercise")]
        public async Task<IActionResult> ProxyMFUsExercisesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/exercises/export-mfu-exercise";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/exercises/mfu-exercise";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }

        [HttpPut("mfu-exercise/edit")]
        public async Task<IActionResult> ProxyEditMFUsExercisesAsync([FromHeader(Name = headerToken)] string authorization, [FromBody] UpdateResponsesExerciseDto values)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            var url = $"https://{api}/api/monthly-exercise-monitoring";

            return await this.PostPutDeleteHandleRegularRequestAsync(client, "PUT", url, values);
        }
    
        [HttpGet("active-minutes")]
        public async Task<IActionResult> ProxyActiveMinutesAsync([FromHeader(Name = headerToken)] string authorization, [FromQuery] string? typeExport, [FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            var client = this.ConfigureHttpClient(_clientFactory, authorization);

            if (!string.IsNullOrEmpty(typeExport))
            {
                var url = $"https://{api}/api/admin/exercises/export-active-minutes";

                return await this.HandleRequestAsync(client, filter, url, typeExport, 0);
            }
            else
            {
                var url = $"https://{api}/api/admin/exercises/active-minutes";

                return await this.HandleRequestAsync(client, filter, url, null, page);
            }
        }
    }
}