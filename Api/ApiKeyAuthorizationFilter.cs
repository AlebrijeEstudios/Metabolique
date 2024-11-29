using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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

    [AttributeUsage(AttributeTargets.Method)]
    public class ApiKeyAuthorizationFilterAttribute : TypeFilterAttribute
    {
        public ApiKeyAuthorizationFilterAttribute() : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
