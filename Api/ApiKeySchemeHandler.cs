using AppVidaSana.Api.Key;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AppVidaSana.Api
{
    public class ApiKeySchemeHandler : AuthenticationHandler<ApiKeySchemeOptions>
    {
        private readonly ApiDbContext _context;

        public ApiKeySchemeHandler(ApiDbContext context, IOptionsMonitor<ApiKeySchemeOptions> options,

            ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(Options.HeaderName, out var headerValue))
            {
                return AuthenticateResult.Fail("Header Not Found.");
            }

            var apiKey = await _context.ApiKeys
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Key.ToString() == headerValue);

            if (apiKey is null)
            {
                return AuthenticateResult.Fail("Wrong Api Key.");
            }

            var claims = new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, $"{apiKey.ApiKeyId}"),
            new Claim(ClaimTypes.Name, apiKey.Name)
            };

            var identiy = new ClaimsIdentity(claims, nameof(ApiKeySchemeHandler));
            var principal = new ClaimsPrincipal(identiy);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
