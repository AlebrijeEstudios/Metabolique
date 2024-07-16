using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Alimentación.Alimentos;

namespace AppVidaSana.Models.Alimentación
{
    public class Lunch
    {
        [Key]
        public Guid lunchID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly dateLunch { get; set; }

        [Required(ErrorMessage = "El campo hora es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly hourLunch { get; set; }

        [Required(ErrorMessage = "El campo nivel de saciedad es obligatoria")]
        public string satietyLevel { get; set; } = null!;

        [Required(ErrorMessage = "El campo emociones ligadas es obligatoria")]
        public string linkedEmotions { get; set; } = null!;

        public Account? account { get; set; }

        public ICollection<FoodsLunch> foodsLunch { get; set; } = new List<FoodsLunch>();
    }
}
