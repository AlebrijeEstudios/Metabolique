using AppVidaSana.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppVidaSana.Api
{
    public class ApiKeyAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var apiKeyHeader = context.HttpContext.Request.Headers["Metabolique_API_KEY"].ToString();;
            var storedApiKey = Environment.GetEnvironmentVariable("API_KEY");
            var storedIosApiKey = Environment.GetEnvironmentVariable("IOS_API_KEY");
            var storedAndroidApiKey = Environment.GetEnvironmentVariable("ANDROID_API_KEY");
            var storedAdminWebApiKey = Environment.GetEnvironmentVariable("ADMIN_WEB_API_KEY");

            var validKeys = new[]
            {
                storedApiKey,
                storedIosApiKey,
                storedAndroidApiKey,
                storedAdminWebApiKey
            };

            if (!validKeys.Contains(apiKeyHeader))
            {
                throw new ApiKeyException();
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
