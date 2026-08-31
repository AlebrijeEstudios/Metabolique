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
using AppVidaSana.ValidationValues;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWDoctorService : IAWDoctors
    {
        private readonly AppDbContext _bd;
        private static readonly Random random = new();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AWDoctorService(AppDbContext bd, IHttpContextAccessor httpContextAccessor)
        {
            _bd = bd;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AllDoctorsDto> CreateDoctorAsync(AWDoctorDto values, CancellationToken cancellationToken)
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

            var password = GenerateValidPassword();

            Doctors accountDoctor = new Doctors
            {
                username = values.username,
                email = values.email,
                password = BCrypt.Net.BCrypt.HashPassword(password),
                roleID = role.roleID
            };

            ValidationValuesDB.ValidationValues(accountDoctor);

            _bd.Doctors.Add(accountDoctor);

            if (!Save()) { throw new UnstoredValuesException(); }

            await SendEmailDoctorAsync(accountDoctor.email, password);

            AllDoctorsDto doctor = new AllDoctorsDto
            {
                doctorID = accountDoctor.doctorID,
                username = accountDoctor.username,
                email = accountDoctor.email,
                role = role.role
            };

            return doctor;
        }

        public async Task<List<AllDoctorsDto>> GetDoctorsAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken)
        {
            var doctors = await GetQueryDoctorsAsync(filter, page, cancellationToken);

            var doctorDTOs = doctors.Select(doctor => new AllDoctorsDto
            {
                doctorID = doctor.doctorID,
                username = doctor.username ?? "N/A",
                email = doctor.email ?? "N/A",
                role = doctor.roles!.role

            }).ToList();

            return doctorDTOs;
        }

        public async Task<AllDoctorsDto> UpdateDoctorAsync(AllDoctorsDto values, CancellationToken cancellationToken)
        {
            var doctorToUpdate = await _bd.Doctors.FindAsync(new object[] { values.doctorID }, cancellationToken);

            if (doctorToUpdate is null) { throw new UnstoredValuesException(); }

            var currentDoctorID = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst("doctorID")!.Value);
            var currentRole = await _bd.Roles.FirstOrDefaultAsync(e => e.roleID == doctorToUpdate.roleID, cancellationToken);

            if (values.doctorID == currentDoctorID && values.role != currentRole?.role)
            {
                throw new SelfActionNotAllowedException("No puedes cambiar tu propio rol.");
            }


            var role = await _bd.Roles.FirstOrDefaultAsync(e => e.role == values.role, cancellationToken);

            doctorToUpdate.username = values.username;
            doctorToUpdate.email = values.email;
            doctorToUpdate.roleID = role!.roleID;

            ValidationValuesDB.ValidationValues(doctorToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            AllDoctorsDto doctorDTOs = new AllDoctorsDto
            {
                doctorID = doctorToUpdate.doctorID,
                username = doctorToUpdate.username ?? "N/A",
                email = doctorToUpdate.email ?? "N/A",
                role = role!.role
            };

            return doctorDTOs;
        }

        public async Task<string> DeleteDoctorAsync(Guid doctorID, CancellationToken cancellationToken)
        {
            var currentDoctorID = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst("doctorID")!.Value);

            if (doctorID == currentDoctorID)
            {
                throw new SelfActionNotAllowedException("No puedes eliminarte a ti mismo.");
            }

            var patientDoctorToDelete = await _bd.PacientDoctor.Where(e => e.doctorID == doctorID).ToListAsync(cancellationToken);

            _bd.PacientDoctor.RemoveRange(patientDoctorToDelete);

            var doctorToDelete = await _bd.Doctors.FindAsync(new object[] { doctorID }, cancellationToken);

            if (doctorToDelete is null) { throw new UnstoredValuesException(); }

            _bd.Doctors.Remove(doctorToDelete);

            if (!Save()) { throw new UnstoredValuesException(); }

            return "El registro ha sido eliminado correctamente.";
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

        private static async Task SendEmailDoctorAsync(string email, string password)
        {
            List<string?> errors = new List<string?>();

            try
            {
                EmailClient emailClient = new EmailClient(Environment.GetEnvironmentVariable("EMAIL_API"));
                string emailSenderAddress = Environment.GetEnvironmentVariable("EMAIL_SENDER_ADDRESS");
                string linkAW = Environment.GetEnvironmentVariable("ADMIN_WEB");

                await emailClient.SendAsync(
                    WaitUntil.Completed,
                    senderAddress: emailSenderAddress,
                    recipientAddress: email,
                    subject: "Cuenta Administrador Web Metabolique",
                    htmlContent: $"<html><body><h2>Cuenta Admin Web Metabolique</h2><p>Hola,</p><p>Se te ha dado de alta en el administrador web de la app Metabolique.</p><p>Email: {email}</p><p>Password: {password}</p><p>Este es el link para accesar al administrador web:</p><p><a href=\"{linkAW}\">Admin web Metabolique</a></p><p>Gracias,</p><p>Tu equipo de soporte</p></body></html>",
                    plainTextContent: $"Click the link to access admin web: {linkAW}");

            }
            catch (EmailNotSendException ex)
            {
                errors.Add(ex.Message);
            }

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }
        }

        private async Task<List<Doctors>> GetQueryDoctorsAsync(FilterAdminDto? filter, int page, CancellationToken cancellationToken)
        {
            var query = _bd.Doctors
                           .Include(f => f.roles)
                           .AsQueryable();

            if (filter != null)
            {

                if (!string.IsNullOrWhiteSpace(filter.doctorID.ToString()))
                    query = query.Where(f => f.doctorID.ToString().Contains(filter.doctorID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.role))
                    query = query.Where(f => f.roles!.role.Contains(filter.role ?? ""));
            }

            var doctors = await query
                        .Skip((page - 1) * 10)
                        .Take(10)
                        .ToListAsync(cancellationToken);

            return doctors;
        }
    }
}
