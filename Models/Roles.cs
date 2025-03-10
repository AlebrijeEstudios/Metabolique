using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models
{
    public class Roles
    {
        [Key]
        public Guid roleID { get; set; } 

        public string role { get; set; } = null!;

        public ICollection<Doctors> doctor { get; set; } = new List<Doctors>();

    }
}
