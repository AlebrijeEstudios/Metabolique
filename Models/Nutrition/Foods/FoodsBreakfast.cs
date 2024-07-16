using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Alimentación.Alimentos
{
    public class FoodsBreakfast
    {
        [Key]
        public Guid foodID { get; set; } = Guid.NewGuid();

        [ForeignKey("Breakfast")]
        public Guid breakfastID { get; set; }

        [Required(ErrorMessage = "El campo nombre del alimento es obligatorio")]
        public string nameFood { get; set; } = null!;

        [Required(ErrorMessage = "El campo cantidad consumida es obligatorio")]
        public float amountConsumed { get; set; }

        public Breakfast? breakfast { get; set; }

    }
}
