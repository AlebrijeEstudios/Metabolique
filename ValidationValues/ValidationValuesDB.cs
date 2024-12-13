using AppVidaSana.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.ValidationValues
{
    public static class ValidationValuesDB
    {
        public static void ValidationValues(object obj)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj, null, null);

            if (!Validator.TryValidateObject(obj, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
        }
    }
}
