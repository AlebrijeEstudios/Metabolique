using AppVidaSana.ProducesResponseType.ResponseOperationsFilters.ApiResponsesAttribute;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppVidaSana.ProducesResponseType.ResponseOperationsFilters
{
    public class ConflictResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasCommonResponses = context.MethodInfo
                .GetCustomAttributes(typeof(ConflictApiResponseAttribute), false)
                .Length;

            if (hasCommonResponses > 0)
            {
                if (!operation.Responses.ContainsKey("409"))
                {
                    operation.Responses.Add("409", new OpenApiResponse
                    {
                        Description = "Returns a series of messages indicating that some values are invalid.",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = context.SchemaGenerator.GenerateSchema(typeof(ExceptionListMessages), context.SchemaRepository)
                            }
                        }
                    });
                }
            }
        }
    }
}
