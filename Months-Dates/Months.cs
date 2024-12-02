using AppVidaSana.Exceptions;

namespace AppVidaSana.Months_Dates
{
    public class Months
    {
        public string VerifyExistMonth(int month)
        {
            var months = new Dictionary<int, string>
            {
                { 1, "Enero" },
                { 2, "Febrero" },
                { 3, "Marzo" },
                { 4, "Abril" },
                { 5, "Mayo" },
                { 6, "Junio" },
                { 7, "Julio" },
                { 8, "Agosto" },
                { 9, "Septiembre" },
                { 10, "Octubre" },
                { 11, "Noviembre" },
                { 12, "Diciembre" }
            };

            var getMonth = months.GetValueOrDefault(month, "Mes no existente");

            if (getMonth == "Mes no existente") { throw new UnstoredValuesException(); }

            return months[month];
        }
    }
}
