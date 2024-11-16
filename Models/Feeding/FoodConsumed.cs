using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Feeding
{
    public class FoodConsumed
    {
        [Key]
        public Guid foodConsumedID { get; set; } = Guid.NewGuid();

        [ForeignKey("UserFeeds")]
        public Guid userFeedID { get; set; }

        [Required(ErrorMessage = "El campo nombre de alimento es obligatorio")]
        public string foodName { get; set; } = null!;

        [Required(ErrorMessage = "El campo porcion es obligatorio")]
        public float portion { get; set; }

        [Required(ErrorMessage = "El campo unidad es obligatorio")]
        public string unit { get; set; } = null!;

        [Required(ErrorMessage = "El campo peso neto es obligatorio")]
        public int netWeight { get; set; }

        [Required(ErrorMessage = "El campo kilocalorias es obligatorio")]
        public float kilocalories { get; set; }

        [Required(ErrorMessage = "El campo proteina es obligatorio")]
        public float protein { get; set; }

        [Required(ErrorMessage = "El campo carbohidratos es obligatorio")]
        public float carbohydrates { get; set; }

        [Required(ErrorMessage = "El campo lipidos totales es obligatorio")]
        public float totalLipids { get; set; }

        public UserFeeds? userFeeds { get; set; }

    }
}
