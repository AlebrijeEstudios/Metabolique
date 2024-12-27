using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Feeding
{
    public class DailyMeals
    {
        [Key]
        public Guid dailyMealID { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El campo comida del dia es obligatorio")]
        public string dailyMeal { get; set; } = null!;

        public ICollection<UserFeeds> userFeeds { get; set; } = new List<UserFeeds>();

    }
}
