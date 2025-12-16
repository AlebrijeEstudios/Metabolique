using AppVidaSana.ProducesResponseType.ResponseOperationsFilters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace AppVidaSana.Api
{
    public static class SwaggerConfiguration
    {
        public static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("app", new OpenApiInfo
                {
                    Title = "Metabolique API",
                    Version = "v1",
                    Description = "An ASP.NET Core web API to manage medical tracking elements of a user's medical record."
                });

                c.SwaggerDoc("admin", new OpenApiInfo
                {
                    Title = "Metabolique Admin Web APIs",
                    Version = "v1",
                    Description = "An ASP.NET Core web API for Metabolique web administrator."
                });

                c.SwaggerDoc("proxy", new OpenApiInfo
                {
                    Title = "Metabolique Proxys Admin Web APIs",
                    Version = "v1",
                    Description = "Proxies for Metabolique web administrator."
                });

                c.SwaggerDoc("app_test", new OpenApiInfo
                {
                    Title = "App TESTS",
                    Version = "v1",
                    Description = "TESTS Endpoints."
                });

                c.DocInclusionPredicate(DocInclusionByGroup);

                c.TagActionsBy(GetSwaggerTags);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                c.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date",
                    Example = new OpenApiString(DateTime.Today.ToString("yyyy-MM-dd"))
                });

                c.MapType<TimeOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "time",
                    Example = new OpenApiString(DateTime.Today.ToString("HH:mm"))
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                c.OperationFilter<CommonResponsesOperationFilter>();
                c.OperationFilter<BadRequestResponseOperationFilter>();
                c.OperationFilter<UnauthorizedResponseOperationFilter>();
                c.OperationFilter<ConflictResponseOperationFilter>();
                c.OperationFilter<InternalServerErrorResponseOperationFilter>();
            });
        }

        private static bool DocInclusionByGroup(string docName, ApiDescription apiDesc)
        {
            if (string.IsNullOrEmpty(apiDesc.GroupName))
                return false;

            return string.Equals(
                apiDesc.GroupName,
                docName,
                StringComparison.OrdinalIgnoreCase);
        }

        private static IList<string> GetSwaggerTags(ApiDescription api)
        {
            try
            {
                var tags = api.ActionDescriptor.EndpointMetadata
                    .OfType<TagsAttribute>()
                    .SelectMany(t => t.Tags)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList();

                if (tags.Any())
                    return tags;

                return new List<string>
                {
                api.GroupName ??
                api.ActionDescriptor.RouteValues["controller"]?.ToString()
                ?? "Default"
            };
            }
            catch
            {
                return new List<string>
                {
                api.ActionDescriptor.RouteValues["controller"]?.ToString()
                ?? "Default"
            };
            }
        }
    }
}
