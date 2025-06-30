using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions.Account_Profile.ValidationTimeoutException;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Doctor_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services
{
    public class AccountService : IAccount
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;
        public AccountService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAccountAsync(AccountDto values, CancellationToken cancellationToken)
        {
            List<string?> errors = new List<string?>();

            try
            {
                string verifyStatusEmail = await verifyEmailAsync(values.email, cancellationToken);

                if (verifyStatusEmail != "") { errors.Add(verifyStatusEmail); }
            }
            catch (EmailValidationTimeoutException ex)
            {
                errors.Add(ex.Message);
            }

            try
            {
                string verifyStatusPassword = verifyPassword(values.password);

                if (verifyStatusPassword != "") { errors.Add(verifyStatusPassword); }
            }
            catch (PasswordValidationTimeoutException ex)
            {
                errors.Add(ex.Message);
            }

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }

            Account account = new Account
            {
                username = values.username,
                email = values.email,
                password = BCrypt.Net.BCrypt.HashPassword(values.password)
            };

            ValidationValuesDB.ValidationValues(account);

            PacientDoctor pd = new PacientDoctor
            {
                accountID = account.accountID,
                doctorID = values.doctorID
            };

            _bd.Accounts.Add(account);
            _bd.PacientDoctor.Add(pd);

            if (!Save()) { throw new UnstoredValuesException(); }

            Guid accountID = account.accountID;

            return accountID;
        }

        public async Task<InfoAccountDto> GetAccountAsync(Guid accountID, CancellationToken cancellationToken)
        {
            DoctorDto doctorDto = new DoctorDto();

            var account = await _bd.Accounts.FindAsync(new object[] { accountID }, cancellationToken);
            var profile = await _bd.Profiles.FirstOrDefaultAsync(e => e.accountID == accountID, cancellationToken);

            if (account is null || profile is null) { throw new UserNotFoundException(); }

            InfoAccountDto infoUser = _mapper.Map<InfoAccountDto>(account);
            _mapper.Map(profile, infoUser);

            var protocol = await _bd.Protocols.FindAsync(new object[] { profile.protocolID! }, cancellationToken);

            var doctorPatient = await _bd.PacientDoctor.FirstOrDefaultAsync(e => e.accountID == accountID, cancellationToken);

            if (doctorPatient!.doctorID is null)
            {
                doctorDto = null;
            }
            else 
            {  
                var doctor = await _bd.Doctors.FindAsync(new object[] { doctorPatient.doctorID }, cancellationToken);

                doctorDto.doctorID = doctor!.doctorID;
                doctorDto.username = doctor!.username;
            }

            infoUser.protocolToFollow = protocol!.protocolToFollow;
            infoUser.doctor = doctorDto;

            return infoUser;
        }

        public async Task<ProfileDto> UpdateAccountAsync(InfoAccountDto values, CancellationToken cancellationToken)
        {
            List<string?> errors = new List<string?>();

            var user = await _bd.Accounts.FindAsync(new object[] { values.accountID }, cancellationToken);

            if (user is null) { throw new UserNotFoundException(); }

            if (user.email != values.email)
            {
                try
                {
                    string verifyStatusEmail = await verifyEmailAsync(values.email, cancellationToken);

                    if (verifyStatusEmail != "") { errors.Add(verifyStatusEmail); }
                }
                catch (EmailValidationTimeoutException ex)
                {
                    errors.Add(ex.Message);
                }
            }

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }

            user.username = values.username;
            user.email = values.email;

            ValidationValuesDB.ValidationValues(user);

            var doctorPatient = await _bd.PacientDoctor.FirstOrDefaultAsync(e => e.accountID == values.accountID, cancellationToken);

            doctorPatient!.doctorID = values.doctor?.doctorID;

            if (!Save()) { throw new UnstoredValuesException(); }

            ProfileDto profile = new ProfileDto
            {
                accountID = values.accountID,
                birthDate = values.birthDate,
                sex = values.sex,
                stature = values.stature,
                weight = values.weight,
                protocolToFollow = values.protocolToFollow,
                uiemID = values.uiemID
            };

            return profile;
        }

        public async Task<string> DeleteAccountAsync(Guid accountID, CancellationToken cancellationToken)
        {
            var account = await _bd.Accounts.FindAsync(new object[] { accountID }, cancellationToken);

            if (account is null) { throw new UserNotFoundException(); }

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

        private async Task<string> verifyEmailAsync(string email, CancellationToken cancellationToken)
        {
            var existingEmail = await _bd.Accounts.AnyAsync(c => c.email == email, cancellationToken);

            if (!RegexPatterns.RegexPatterns.Emailregex.IsMatch(email))
            {
                return "El correo electr&oacute;nico no tiene un formato v&aacute;lido.";
            }

            if (existingEmail!)
            {
                return "Este correo electr&oacute;nico est&aacute; ligado a una cuenta existente.";
            }

            return "";
        }

        private static string verifyPassword(string password)
        {
            if (password.Length < 8)
            {
                return "La contrase&ntilde;a debe tener al menos 8 caracteres.";
            }

            if (!RegexPatterns.RegexPatterns.Passwordregex.IsMatch(password))
            {
                return "La contrase&ntilde;a debe contener al menos un n&uacute;mero, una letra min&uacute;scula o letra may&uacute;scula y un car&aacute;cter alfanum&eacute;rico.";
            }

            return "";
        }
    }
}