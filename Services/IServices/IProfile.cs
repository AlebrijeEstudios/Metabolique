﻿using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IProfile
    {
        void CreateProfile(Guid accountID, AccountDto values);

        Task<string> UpdateProfileAsync(ProfileDto values, CancellationToken cancellationToken);

        bool Save();
    }
}
