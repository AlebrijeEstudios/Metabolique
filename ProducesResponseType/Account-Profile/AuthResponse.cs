﻿using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.ProducesResponseType.Account
{
    public class AuthResponse
    {
        public string message { get; set; } = "Ok.";

        public TokensDto auth { get; set; } = null!;
    }
}