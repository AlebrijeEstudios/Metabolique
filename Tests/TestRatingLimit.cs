using AppVidaSana.Models.Alimentación.Alimentos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace AppVidaSana.Tests
{
    public class TestRatingLimit
    {
        [Theory]
        [InlineData("044cb372-7ca5-43b5-af14-021762723427")]
        public async Task Test(Guid id)
        {
            var client = new HttpClient();
            var requestUri = "https://localhost:7098/api/accounts/" + id;

            for (int i = 0; i < 50; i++)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkphaGlyIE5pY29sw6FzIiwiZW1haWwiOiJqYWhpcm5pY29sYXM4QGdtYWlsLmNvbSIsInJvbGUiOiJVc2VyIiwibmJmIjoxNzIxMzgwMjkwLCJleHAiOjE3MjE5ODUwOTAsImlhdCI6MTcyMTM4MDI5MCwiaXNzIjoidmlkYXNhbmFhcGkiLCJhdWQiOiJ2aWRhc2FuYS5jb20ifQ.MyCdi78HErCoVbCgMEbdOxQ-pjkOOcVLJhsMnp3hpkE");
                client.DefaultRequestHeaders.Add("Metabolique_API_KEY", Environment.GetEnvironmentVariable("API_KEY"));

                var response = await client.GetAsync(requestUri);

                if (i < 50)
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                else
                {
                    Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(30));

            for (int i = 0; i < 50; i++)
            {
                var response = await client.GetAsync(requestUri);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
