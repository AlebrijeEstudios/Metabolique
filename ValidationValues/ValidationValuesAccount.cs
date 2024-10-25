using AppVidaSana.Data;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.ValidationValues
{
    public class ValidationValuesAccount
    {
        public async Task<string> verifyEmail(string email, AppDbContext bd, CancellationToken cancellationToken)
        {
            var task = bd.Accounts.AnyAsync(c => c.email == email, cancellationToken);
            
            if (!RegexPatterns.RegexPatterns.Emailregex.IsMatch(email))
            {
                return "El correo electrónico no tiene un formato válido.";
            }

            var existingEmail = await task;

            if (existingEmail!)
            {
                return "Este correo electrónico está ligado a una cuenta existente.";
            }

            return "";
        }

        public string verifyPassword(string password)
        {
            if (password.Length < 8)
            {
                return "La contraseña debe tener al menos 8 caracteres.";
            }

            if (!RegexPatterns.RegexPatterns.Passwordregex.IsMatch(password))
            {
                return "La contraseña debe contener al menos un número, una letra minúscula o letra mayúscula y un carácter alfanumérico.";
            }

            return "";
        }
    }
}
