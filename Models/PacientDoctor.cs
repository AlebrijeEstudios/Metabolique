using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models
{
    public class PacientDoctor
    {
        [Key]
        public Guid pacientDoctorID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [ForeignKey("Doctors")]
        public Guid doctorID { get; set; }

        public Account? account { get; set; }

        public Doctors? doctor { get; set; }
    }
}
