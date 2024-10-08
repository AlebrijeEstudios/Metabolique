using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;

namespace AppVidaSana.Models.Monthly_Follow_Ups
{
    public class MFUsFood
    {
        [Key]
        public Guid monthlyFollowUpID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [ForeignKey("MFUsMonths")]
        public Guid monthID { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 1 es obligatorio")]
        public float answerQuestion1 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 2 es obligatorio")]
        public float answerQuestion2 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 3 es obligatorio")]
        public float answerQuestion3 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 4 es obligatorio")]
        public float answerQuestion4 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 5 es obligatorio")]
        public float answerQuestion5 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 6 es obligatorio")]
        public float answerQuestion6 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 7 es obligatorio")]
        public float answerQuestion7 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 8 es obligatorio")]
        public float answerQuestion8 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 9 es obligatorio")]
        public float answerQuestion9 { get; set; }

        public Account? account { get; set; }

        public MFUsMonths? months { get; set; }

        public FoodResults? results { get; set; }

    }
}
