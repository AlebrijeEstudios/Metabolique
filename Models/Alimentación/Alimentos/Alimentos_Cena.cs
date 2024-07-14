using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Alimentación.Alimentos
{
    public class Alimentos_Cena
    {
        [Key]
        public Guid alimentoID { get; set; } = Guid.NewGuid();

        [ForeignKey("Cena")]
        public Guid cenaID { get; set; }

        [Required(ErrorMessage = "El campo nombre del alimento es obligatorio")]
        public string nombreAlimento { get; set; } = null!;

        [Required(ErrorMessage = "El campo cantidad consumida es obligatorio")]
        public float cantidadConsumida { get; set; }

        public Cena? cena { get; set; }

    }
}
