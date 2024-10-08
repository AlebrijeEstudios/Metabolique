﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;

namespace AppVidaSana.Api
{
    public class ApiKeySchemeOptions : AuthenticationSchemeOptions
    {
        public const string Scheme = "ApiKeyScheme";
        public string HeaderName { get; set; } = HeaderNames.Authorization;
    }
}
