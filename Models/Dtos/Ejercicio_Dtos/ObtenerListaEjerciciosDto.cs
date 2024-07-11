using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Dtos.Ejercicio_Dtos
{
    public class ObtenerListaEjerciciosDto
    {
        public Guid id { get; set; }

        [DataType(DataType.Date)]
        public DateOnly fecha { get; set; }
    }
}
