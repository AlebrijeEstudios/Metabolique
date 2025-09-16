using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppVidaSana.ProducesResponseType
{
    public class CommonResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasCommonResponses = context.MethodInfo
                .GetCustomAttributes(typeof(CommonApiResponsesAttribute), false)
                .Length;

            if (hasCommonResponses > 0)
            {
                if (!operation.Responses.ContainsKey("429"))
                {
                    operation.Responses.Add("429", new OpenApiResponse
                    {
                        Description = "Returns a series of messages indicating too many requests.",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = context.SchemaGenerator.GenerateSchema(typeof(RequestGeneralExceptionMessage), context.SchemaRepository)
                            }
                        }
                    });
                }

                if (!operation.Responses.ContainsKey("503"))
                {
                    operation.Responses.Add("503", new OpenApiResponse
                    {
                        Description = "Returns a message indicating that the response timeout has passed.",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = context.SchemaGenerator.GenerateSchema(typeof(RequestGeneralExceptionMessage), context.SchemaRepository)
                            }
                        }
                    });
                }
            }
        }
    }
}
