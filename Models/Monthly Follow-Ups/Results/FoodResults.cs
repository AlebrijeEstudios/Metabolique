using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Monthly_Follow_Ups.Results
{
    public class FoodResults
    {
        [Key]
        public Guid resultsID { get; set; } = Guid.NewGuid();
         
        [ForeignKey("MFUsFood")]
        public Guid monthlyFollowUpID { get; set; }

        [Required(ErrorMessage = "El campo total de puntos es obligatorio")]
        public float totalPts { get; set; }

        [Required(ErrorMessage = "El campo clasificacion es obligatorio")]
        public string classification { get; set; } = null!;

        public MFUsFood? MFUsFood { get; set; }
    }
}
