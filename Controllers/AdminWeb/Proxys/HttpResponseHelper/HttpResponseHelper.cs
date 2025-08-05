using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;

namespace AppVidaSana.Controllers.AdminWeb.Proxys.HttpResponseHelper
{
    public static class HttpResponseHelper
    {
        private const string formatDate = "yyyy-MM-dd";
        private const string apiKeyHeaderName = "ApiKeyHeaderName";
        private const string apiKey = "API_KEY";
        private const string bearerScheme = "Bearer";
        private const string typeArchiveJson = "application/json";
        private const string typeArchiveZip = "application/zip";
        private const string defaultNameZip = "default.zip";

        public static List<string> BuildQueryParameters(this ControllerBase controller, 
            object filter, string? typeExport, int page)
        {
            var queryParams = new List<string>();

            if (filter == null)
            {
                AddPaginationOrExport(queryParams, typeExport, page);
                return queryParams;
            }

            var properties = filter.GetType().GetProperties();

            var commonPropertyMappings = new Dictionary<string, string>
            {
                ["doctorID"] = "doctorID",
                ["accountID"] = "accountID",
                ["uiemID"] = "uiemID",
                ["username"] = "username",
                ["month"] = "month",
                ["year"] = "year",
                ["sex"] = "sex",
                ["protocolToFollow"] = "protocolToFollow",
                ["typeExercise"] = "typeExercise",
                ["intensityExercise"] = "intensityExercise",
                ["startDate"] = "startDate",
                ["endDate"] = "endDate"
            };

            foreach (var property in properties)
            {
                var value = property.GetValue(filter);
                if (value == null) continue;

                string parameterName = "";
                if (commonPropertyMappings.ContainsKey(property.Name))
                {
                    parameterName = commonPropertyMappings[property.Name];
                }

                string parameterValue = FormatValue(value, property.Name);

                if (!string.IsNullOrEmpty(parameterValue))
                {
                    queryParams.Add($"{parameterName}={parameterValue}");
                }
            }

            AddPaginationOrExport(queryParams, typeExport, page);
            return queryParams;
        }

        private static string FormatValue(object value, string propertyName)
        {
            if (value is DateTime dateValue)
            {
                return dateValue.ToString(formatDate, CultureInfo.InvariantCulture);
            }

            var stringValue = value.ToString();
            return string.IsNullOrEmpty(stringValue) ? string.Empty : stringValue;
        }

        private static void AddPaginationOrExport(List<string> queryParams, string? typeExport, int page)
        {
            if (!string.IsNullOrEmpty(typeExport))
            {
                queryParams.Add($"typeExport={typeExport}");
            }
            else
            {
                queryParams.Add($"page={page}");
            }
        }

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

        public static HttpClient ConfigureHttpClient(this ControllerBase controller, IHttpClientFactory clientFactory, string authorization)
        {
            var client = clientFactory.CreateClient();

            var apiKeyHeader = Environment.GetEnvironmentVariable(apiKeyHeaderName);
            var apiKeyValue = Environment.GetEnvironmentVariable(apiKey);
            if (!string.IsNullOrEmpty(apiKeyHeader) && !string.IsNullOrEmpty(apiKeyValue))
            {
                client.DefaultRequestHeaders.Add(apiKeyHeader, apiKeyValue);
            }

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var token = headerValue.Parameter;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(bearerScheme, token);
            }

            return client;
        }

        public static async Task<IActionResult> HandleExportRequestAsync(this ControllerBase controller, HttpClient client, string url, string queryString)
        {
            var response = await client.GetAsync($"{url}?{queryString}");
            var content = await response.Content.ReadAsByteArrayAsync();
            var contentDisposition = response.Content.Headers.ContentDisposition;
            var fileName = contentDisposition?.FileName ?? defaultNameZip;

            return new FileContentResult(content, typeArchiveZip)
            {
                FileDownloadName = fileName
            };
        }

        public static async Task<IActionResult> GetHandleRegularRequestAsync(this ControllerBase controller, 
            HttpClient client, string url, string queryString)
        {
            var response = await client.GetAsync($"{url}?{queryString}");

            return await controller.HandleRegularRequestAsync(response);
        }

        public static async Task<IActionResult> PostPutDeleteHandleRegularRequestAsync(this ControllerBase controller, 
            HttpClient client, string method, string url, object? values)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            switch (method) 
            {
                case "POST": 
                    response = await client.PostAsJsonAsync(url, values);
                    break;
                case "PUT":
                    response = await client.PutAsJsonAsync(url, values);
                    break;
                case "DELETE":
                    response = await client.DeleteAsync(url);
                    break;
            }

            return await controller.HandleRegularRequestAsync(response);
        }

        public static async Task<IActionResult> HandleRegularRequestAsync(this ControllerBase controller, HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return controller.HandleErrorResponse(response, responseBody);
            }

            return controller.Content(responseBody, typeArchiveJson);
        }
    }
}
