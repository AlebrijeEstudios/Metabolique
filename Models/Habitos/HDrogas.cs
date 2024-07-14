using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Habitos
{
    public class HDrogas
    {
        [Key]
        public Guid habitoID { get; set; } = Guid.NewGuid();

        [ForeignKey("Cuenta")]
        public Guid cuentaID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly fecha { get; set; }

        [Required(ErrorMessage = "El campo cigarros consumidos es obligatorio")]
        public int cigarrosConsumidos { get; set; } 

        [Required(ErrorMessage = "El campo estado emocional predominante es obligatorio")]
        public string estadoEmocionalPredominante { get; set; } = null!;

        public Cuenta? cuenta { get; set; }
    }
}
