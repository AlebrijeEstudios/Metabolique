using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Alimentación.Alimentos
{
    public class Alimentos_Desayuno
    {
        [Key]
        public Guid alimentoID { get; set; } = Guid.NewGuid();

        [ForeignKey("Desayuno")]
        public Guid desayunoID { get; set; }

        [Required(ErrorMessage = "El campo nombre del alimento es obligatorio")]
        public string nombreAlimento { get; set; } = null!;

        [Required(ErrorMessage = "El campo cantidad consumida es obligatorio")]
        public float cantidadConsumida { get; set; }

        public Desayuno? desayuno { get; set; }

    }
}
