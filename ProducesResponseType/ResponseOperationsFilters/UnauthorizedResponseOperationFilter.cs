using AppVidaSana.ProducesResponseType.ResponseOperationsFilters.ApiResponsesAttribute;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppVidaSana.ProducesResponseType.ResponseOperationsFilters
{
    public class UnauthorizedResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasCommonResponses = context.MethodInfo
                .GetCustomAttributes(typeof(UnauthorizedApiResponseAttribute), false)
                .Length;

            if (hasCommonResponses > 0)
            {
                if (!operation.Responses.ContainsKey("401"))
                {
                    operation.Responses.Add("401", new OpenApiResponse
                    {
                        Description = "Returns a message indicating that the token has expired.",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = context.SchemaGenerator.GenerateSchema(typeof(ExceptionExpiredTokenMessage), context.SchemaRepository)
                            }
                        }
                    });
                }
            }
        }
    }
}
