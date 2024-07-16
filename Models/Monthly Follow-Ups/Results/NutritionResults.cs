using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Seguimientos_Mensuales.Respuestas
{
    public class NutritionResults
    {
        [Key]
        public Guid resultsID { get; set; } = Guid.NewGuid();

        [ForeignKey("MFUsNutrition")]
        public Guid monthlyFollowUpID { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 1 es obligatorio")]
        public int pointsQuestion1 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 2 es obligatorio")]
        public int pointsQuestion2 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 3 es obligatorio")]
        public int pointsQuestion3 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 4 es obligatorio")]
        public int pointsQuestion4 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 5 es obligatorio")]
        public int pointsQuestion5 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 6 es obligatorio")]
        public int pointsQuestion6 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 7 es obligatorio")]
        public int pointsQuestion7 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 8 es obligatorio")]
        public int pointsQuestion8 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 9 es obligatorio")]
        public int pointsQuestion9 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 10 es obligatorio")]
        public int pointsQuestion10 { get; set; }

        [Required(ErrorMessage = "El campo total de puntos es obligatorio")]
        public int totalPoints { get; set; }

        [Required(ErrorMessage = "El campo clasificacion es obligatorio")]
        public string classification { get; set; } = null!;

        public MFUsNutrition? MFUsNutrition { get; set; }

    }
}
