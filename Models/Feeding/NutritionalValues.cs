using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Feeding
{
    public class NutritionalValues
    {
        [Key]
        public Guid nutritionalValueID { get; set; } = Guid.NewGuid();

        [ForeignKey("Foods")]
        public Guid foodID { get; set; }

        [Required(ErrorMessage = "El campo codigo del valor nutrimental es obligatorio")]
        public string nutritionalValueCode { get; set; } = null!;

        [Required(ErrorMessage = "El campo porcion es obligatorio")]
        public float portion { get; set; }

        [Required(ErrorMessage = "El campo kilocalorias es obligatorio")]
        public float kilocalories { get; set; }

        [Required(ErrorMessage = "El campo proteina es obligatorio")]
        public float protein { get; set; }

        [Required(ErrorMessage = "El campo carbohidratos es obligatorio")]
        public float carbohydrates { get; set; }

        [Required(ErrorMessage = "El campo lipidos totales es obligatorio")]
        public float totalLipids { get; set; }

        public Foods? foods { get; set; }

        public ICollection<UserFeedNutritionalValues> userFeedNutritionalValues { get; set; } = new List<UserFeedNutritionalValues>();
    }
}
