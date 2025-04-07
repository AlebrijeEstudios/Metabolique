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

        [Required(ErrorMessage = "El campo porcion es obligatorio")]
        public string portion { get; set; } = null!;

        public float? netWeight { get; set; }

        [Required(ErrorMessage = "El campo kilocalorias es obligatorio")]
        public double kilocalories { get; set; }

        [Required(ErrorMessage = "El campo proteina es obligatorio")]
        public double protein { get; set; }

        [Required(ErrorMessage = "El campo carbohidratos es obligatorio")]
        public double carbohydrates { get; set; }

        [Required(ErrorMessage = "El campo lipidos totales es obligatorio")]
        public double totalLipids { get; set; }

        public Foods? foods { get; set; }

        public ICollection<UserFeedNutritionalValues> userFeedNutritionalValues { get; set; } = new List<UserFeedNutritionalValues>();
    }
}
