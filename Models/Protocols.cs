using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models
{
    public class Protocols
    {
        [Key]
        public Guid? protocolID { get; set; }

        [Required(ErrorMessage = "El campo protocolo es obligatorio.")]
        public string protocolToFollow { get; set; } = null!;

        public ICollection<Profiles> profiles { get; set; } = new List<Profiles>();
    }
}
