using AppVidaSana.ProducesResponseType.ResponseOperationsFilters.ApiResponsesAttribute;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppVidaSana.ProducesResponseType.ResponseOperationsFilters
{
    public class InternalServerErrorResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasCommonResponses = context.MethodInfo
                .GetCustomAttributes(typeof(InternalServerErrorApiResponseAttribute), false)
                .Length;

            if (hasCommonResponses > 0)
            {
                if (!operation.Responses.ContainsKey("500"))
                {
                    operation.Responses.Add("500", new OpenApiResponse
                    {
                        Description = "Returns a message indicating internal server errors.",
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
}
