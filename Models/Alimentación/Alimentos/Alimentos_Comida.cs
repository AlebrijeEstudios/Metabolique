using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Alimentación.Alimentos
{
    public class Alimentos_Comida
    {
        [Key]
        public Guid alimentoID { get; set; } = Guid.NewGuid();

        [ForeignKey("Comida")]
        public Guid comidaID { get; set; }

        [Required(ErrorMessage = "El campo nombre del alimento es obligatorio")]
        public string nombreAlimento { get; set; } = null!;

        [Required(ErrorMessage = "El campo cantidad consumida es obligatorio")]
        public float cantidadConsumida { get; set; }

        public Comida? comida { get; set; }

    }
}
