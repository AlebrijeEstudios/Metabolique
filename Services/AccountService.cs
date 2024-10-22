using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace AppVidaSana.Services
{
    public class AccountService : IAccount
    {
        private readonly AppDbContext _bd;
        private ValidationValuesDB _validationValues;
        private VerifyValues _verifyValues;

        public AccountService(AppDbContext bd)
        {
            _bd = bd;
            _validationValues = new ValidationValuesDB();
            _verifyValues = new VerifyValues();
        }

        public async Task<Guid> CreateAccount(AccountDto values, CancellationToken cancellationToken)
        {
            List<string?> errors = new List<string?>();

            string message = "";

            string verifyStatusUsername = await _verifyValues.verifyUsername(values.username, _bd, cancellationToken);

            if (verifyStatusUsername != "") { errors.Add(verifyStatusUsername); }

            try
            {
                string verifyStatusEmail = await _verifyValues.verifyEmail(values.email, _bd, cancellationToken);

                if (verifyStatusEmail != "") { errors.Add(verifyStatusEmail); }

            }
            catch (EmailValidationTimeoutException ex)
            {
                message = ex.Message;
                errors.Add(message);
            }

            try
            {
                string verifyStatusPassword = _verifyValues.verifyPassword(values.password);

                if (verifyStatusPassword != "") { errors.Add(verifyStatusPassword); }

            }
            catch (PasswordValidationTimeoutException ex)
            {
                message = ex.Message;
                errors.Add(message);
            }

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }

            var role = await _bd.Roles.FirstOrDefaultAsync(e => e.role == "User", cancellationToken);

            if (role == null) { throw new NoRoleAssignmentException(); }

            Account account = new Account
            {
                username = values.username,
                email = values.email,
                password = BCrypt.Net.BCrypt.HashPassword(values.password),
                roleID = role.roleID
            };

            _validationValues.ValidationValues(account);

            await _bd.Accounts.AddAsync(account, cancellationToken);

            if (!Save()) { throw new UnstoredValuesException(); }

            var user = await _bd.Accounts.FirstOrDefaultAsync(u =>
                                          u.email.ToLower() == account.email.ToLower(), cancellationToken);

            if (user == null) { throw new UnstoredValuesException(); }

            Guid accountID = user.accountID;

            return accountID;

        }

        public async Task<InfoAccountDto> GetAccount(Guid accountID, CancellationToken cancellationToken)
        {
            var account = await _bd.Accounts.FindAsync(accountID, cancellationToken);
            var profile = await _bd.Profiles.FindAsync(accountID, cancellationToken);

            if (account == null || profile == null) { throw new UserNotFoundException(); }

            InfoAccountDto infoUser = new InfoAccountDto
            {
                accountID = account.accountID,
                username = account.username,
                email = account.email,
                birthDate = profile.birthDate,
                sex = profile.sex,
                stature = profile.stature,
                weight = profile.weight,
                protocolToFollow = profile.protocolToFollow
            };

            return infoUser;
        }

        public async Task<ProfileDto> UpdateAccount(InfoAccountDto values, CancellationToken cancellationToken)
        {
            List<string?> errors = new List<string?>();

            string message = "";

            var user = await _bd.Accounts.FindAsync(values.accountID, cancellationToken);

            if (user == null) { throw new UserNotFoundException(); }

            if (user.username != values.username)
            {
                string verifyStatusUsername = await _verifyValues.verifyUsername(values.username, _bd, cancellationToken);

                if (verifyStatusUsername != "")
                {
                    errors.Add(verifyStatusUsername);
                }
            }

            if (user.email != values.email)
            {
                try
                {
                    string verifyStatusEmail = await _verifyValues.verifyEmail(values.email, _bd, cancellationToken);

                    if (verifyStatusEmail != "")
                    {
                        errors.Add(verifyStatusEmail);
                    }

                }
                catch (EmailValidationTimeoutException ex)
                {
                    message = ex.Message;
                    errors.Add(message);
                }
            }

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }

            user.username = values.username;
            user.email = values.email;

            _validationValues.ValidationValues(user);

            _bd.Accounts.Update(user);

            if (!Save()) { throw new UnstoredValuesException(); }

            ProfileDto profile = new ProfileDto
            {
                accountID = values.accountID,
                birthDate = values.birthDate,
                sex = values.sex,
                stature = values.stature,
                weight = values.weight,
                protocolToFollow = values.protocolToFollow
            };

            return profile;
        }

        public async Task<string> DeleteAccount(Guid accountID, CancellationToken cancellationToken)
        {
            var account = await _bd.Accounts.FindAsync(accountID, cancellationToken);

            if (account == null) { throw new UserNotFoundException(); }

            _bd.Accounts.Remove(account);

            if (!Save()) { throw new UnstoredValuesException(); }

            return "Su cuenta ha sido eliminada correctamente.";
        }

        public bool Save()
        {
            try
            {
                return _bd.SaveChanges() >= 0;
            }
            catch (Exception)
            {
                return false;

            }
        }
    }
}