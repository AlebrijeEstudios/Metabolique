﻿using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Account_Profile_Dtos
{
    public class InfoAccountDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public string email { get; set; } = null!;

        [JsonRequired] public DateOnly birthDate { get; set; }

        [JsonRequired] public string sex { get; set; } = null!;

        [JsonRequired] public float stature { get; set; }

        [JsonRequired] public float weight { get; set; }

        [JsonRequired] public string protocolToFollow { get; set; } = null!;

    }
}
