using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Monthly_Follow_Ups.Results
{
    public class HabitsResults
    {
        [Key]
        public Guid resultsID { get; set; } = Guid.NewGuid();

        [ForeignKey("MFUsHabits")]
        public Guid monthlyFollowUpID { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 1 es obligatorio")]
        public byte resultComponent1 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 2 es obligatorio")]
        public byte resultComponent2 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 3 es obligatorio")]
        public byte resultComponent3 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 4 es obligatorio")]
        public byte resultComponent4 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 5 es obligatorio")]
        public byte resultComponent5 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 6 es obligatorio")]
        public byte resultComponent6 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 7 es obligatorio")]
        public byte resultComponent7 { get; set; }

        [Required(ErrorMessage = "El campo clasificacion global es obligatorio")]
        public int globalClassification { get; set; }

        [Required(ErrorMessage = "El campo clasificacion es obligatorio")]
        public string classification { get; set; } = null!;

        public MFUsHabits? MFUsHabits { get; set; }

    }
}
