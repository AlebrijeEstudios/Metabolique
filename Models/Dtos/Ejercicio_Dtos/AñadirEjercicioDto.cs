using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Dtos.Ejercicio_Dtos
{
    public class AñadirEjercicioDto
    { 

        public Guid id { get; set; }

        public DateOnly fecha { get; set; }

        public string tipo { get; set; } = null!;

        public string intensidad { get; set; } = null!;

        public int tiempo { get; set; }

    }
}
