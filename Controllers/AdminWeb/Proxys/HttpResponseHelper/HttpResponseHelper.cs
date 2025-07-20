using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper
{
    public static class HttpResponseHelper
    {
        public static IActionResult HandleErrorResponse(this ControllerBase controller,
        HttpResponseMessage response, string responseBody)
        {
            var contentObject = JsonConvert.DeserializeObject(responseBody);
            return controller.StatusCode((int)response.StatusCode, new
            {
                statusCode = response.StatusCode,
                content = contentObject
            });
        }
    }
}
