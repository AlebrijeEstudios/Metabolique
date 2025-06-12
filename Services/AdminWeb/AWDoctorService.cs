using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions.Account_Profile.ResetPasswordException;
using AppVidaSana.Exceptions.Account_Profile.ValidationTimeoutException;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos;
using AppVidaSana.Services.IServices.IAdminWeb;
using Azure.Communication.Email;
using Azure;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWDoctorService : IAWDoctors
    {
        private readonly AppDbContext _bd;
        private static readonly Random random = new();

        public AWDoctorService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<string> InsertDoctorAsync(DoctorDto values, CancellationToken cancellationToken)
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

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }

            var role = await _bd.Roles.FirstOrDefaultAsync(e => e.role == values.role, cancellationToken);

            if (role is null) { throw new NoRoleAssignmentException(); }

            Doctors accountDoctor = new Doctors
            {
                username = values.username,
                email = values.email,
                password = BCrypt.Net.BCrypt.HashPassword(GenerateValidPassword()),
                roleID = role.roleID
            };

            _bd.Doctors.Add(accountDoctor);

            if (!Save()) { throw new UnstoredValuesException(); }

            SendEmailDoctorAsync(accountDoctor.email, accountDoctor.password);

            return "Registro de nuevo doctor completado con éxito.";
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
            var existingEmail = await _bd.Doctors.AnyAsync(c => c.email == email, cancellationToken);

            if (!RegexPatterns.RegexPatterns.Emailregex.IsMatch(email))
            {
                return "El correo electrónico no tiene un formato válido.";
            }

            if (existingEmail!)
            {
                return "Este correo electrónico está ligado a una cuenta existente.";
            }

            return "";
        }

        private static string GenerateValidPassword(int length = 12)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specials = @"!""#$%&'()*+,-./:;=?@[\]^_`{|}~";
            const string all = letters + digits + specials;

            string password;

            do
            {
                var sb = new StringBuilder();

                sb.Append(letters[random.Next(letters.Length)]);
                sb.Append(digits[random.Next(digits.Length)]);
                sb.Append(specials[random.Next(specials.Length)]);

                for (int i = 3; i < length; i++)
                {
                    sb.Append(all[random.Next(all.Length)]);
                }

                password = new string(sb.ToString().ToCharArray().OrderBy(c => random.Next()).ToArray());

            } while (!RegexPatterns.RegexPatterns.Passwordregex.IsMatch(password));

            return password;
        }

        private static async void SendEmailDoctorAsync(string email, string password)
        {
            List<string?> errors = new List<string?>();
            string linkAW = "https://ambitious-river-0965b2e10.6.azurestaticapps.net/";

            try
            {
                EmailClient emailClient = new EmailClient(Environment.GetEnvironmentVariable("EMAIL_API"));
                await emailClient.SendAsync(
                    WaitUntil.Completed,
                    senderAddress: "DoNotReply@6895ce04-ff2e-4cd1-b2fa-4544b971a71e.azurecomm.net",
                    recipientAddress: email,
                    subject: "Cuenta Administrador Web Metabolique",
                    htmlContent: $"<html><body><h2>Cuenta Admin Web Metabolique</h2><p>Hola,</p><p>Se te ha dado de alta en el administrador web de la app Metabolique.</p><p>Email:{email}</p><p>Password:{password}</p><p>Este es el link para accesar al administrador web:</p><p><a href=\"{linkAW}\">Admin web Metabolique</a></p><p>Gracias,</p><p>Tu equipo de soporte</p></body></html>",
                    plainTextContent: $"Click the link to access admin web: {linkAW}");

            }
            catch (EmailNotSendException ex)
            {
                errors.Add(ex.Message);
            }

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }
        }
    }
}
