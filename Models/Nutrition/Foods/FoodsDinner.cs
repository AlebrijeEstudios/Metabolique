using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Alimentación.Alimentos
{
    public class FoodsDinner
    {
        [Key]
        public Guid foodID { get; set; } = Guid.NewGuid();

        [ForeignKey("Dinner")]
        public Guid dinnerID { get; set; }

        [Required(ErrorMessage = "El campo nombre del alimento es obligatorio")]
        public string nameFood { get; set; } = null!;

        [Required(ErrorMessage = "El campo cantidad consumida es obligatorio")]
        public float amountConsumed { get; set; }

        public Dinner? dinner { get; set; }

    }
}
