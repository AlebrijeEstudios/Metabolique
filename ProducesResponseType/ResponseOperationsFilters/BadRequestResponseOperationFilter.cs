using AppVidaSana.ProducesResponseType.ResponseOperationsFilters.ApiResponsesAttribute;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppVidaSana.ProducesResponseType.ResponseOperationsFilters
{
    public class BadRequestResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasCommonResponses = context.MethodInfo
                .GetCustomAttributes(typeof(BadRequestApiResponseAttribute), false)
                .Length;

            if (hasCommonResponses > 0 && !operation.Responses.ContainsKey("400"))
            {
                operation.Responses.Add("400", new OpenApiResponse
                {
                    Description = "Returns a message that the requested action could not be performed.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(ExceptionMessage), context.SchemaRepository)
                        }
                    }
                });
            }
        }
    }
}
