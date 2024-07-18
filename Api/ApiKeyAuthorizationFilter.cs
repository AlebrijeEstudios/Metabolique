using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Azure.Core;

namespace AppVidaSana.Api
{
    public class ApiKeyAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var apiKeyHeader = context.HttpContext.Request.Headers["Metabolique_API_KEY"].FirstOrDefault();
            var storedApiKey = Environment.GetEnvironmentVariable("API_KEY");
            var headerApiKey = context.HttpContext.Request.Headers["Metabolique_API_KEY"].ToString();

            if (string.IsNullOrEmpty(apiKeyHeader) || (storedApiKey != headerApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }

    public class ApiKeyAuthorizationFilterAttribute : TypeFilterAttribute
    {
        public ApiKeyAuthorizationFilterAttribute() : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
