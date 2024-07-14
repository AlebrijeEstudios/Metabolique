using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Alimentación.Alimentos
{
    public class Alimentos_Almuerzo
    {
        [Key]
        public Guid alimentoID { get; set; } = Guid.NewGuid();

        [ForeignKey("Almuerzo")]
        public Guid almuerzoID { get; set; }

        [Required(ErrorMessage = "El campo nombre del alimento es obligatorio")]
        public string nombreAlimento { get; set; } = null!;

        [Required(ErrorMessage = "El campo cantidad consumida es obligatorio")]
        public float cantidadConsumida { get; set; }

        public Almuerzo? almuerzo { get; set; }

    }
}
