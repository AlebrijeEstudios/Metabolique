using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Feeding
{
    public class SaucerPictures
    {
        [Key]
        public Guid saucerPictureID { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El campo hash de la imagen es obligatorio")]
        public string hashPicture { get; set; } = null!;

        [Required(ErrorMessage = "El campo url de imagen del platillo es obligatorio")]
        public string saucerPictureUrl { get; set; } = null!;

        public ICollection<UserFeeds> userFeeds { get; set; } = new List<UserFeeds>();
    }
}
