using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Feeding
{
    public class Foods
    {
        [Key]
        public Guid foodID { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El campo codigo del alimento es obligatorio")]
        public string foodCode { get; set; } = null!;

        [Required(ErrorMessage = "El campo nombre del alimento es obligatorio")]
        public string nameFood { get; set; } = null!;

        public string? unit { get; set; }

        public ICollection<NutritionalValues> nutritionalValues { get; set; } = new List<NutritionalValues>();

    }
}
