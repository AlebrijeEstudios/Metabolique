using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Feeding
{
    public class UserFeeds
    {
        [Key]
        public Guid userFeedID { get; set; } = Guid.NewGuid(); 

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [ForeignKey("DailyMeals")]
        public Guid dailyMealID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateOnly userFeedDate { get; set; }

        [Required(ErrorMessage = "El campo hora es obligatorio")]
        public TimeOnly userFeedTime { get; set; }

        [Required(ErrorMessage = "El campo nivel de saciedad es obligatorio")]
        public string satietyLevel { get; set; } = null!;

        [Required(ErrorMessage = "El campo emociones ligadas es obligatorio")]
        public string emotionsLinked { get; set; } = null!;

        [Required(ErrorMessage = "El campo total de calorias es obligatorio")]
        public float totalCalories { get; set; }

        [ForeignKey("SaucerPictures")]
        public Guid? saucerPictureID { get; set; }

        public Account? account { get; set; }

        public DailyMeals? dailyMeals { get; set; }

        public SaucerPictures? saucerPicture { get; set; }

        public ICollection<UserFeedNutritionalValues> userFeedNutritionalValues { get; set; } = new List<UserFeedNutritionalValues>();

    }
}
