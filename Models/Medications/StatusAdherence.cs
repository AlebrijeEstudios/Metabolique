using AppVidaSana.Models.Monthly_Follow_Ups;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Medications
{
    public class StatusAdherence
    {
        [Key]
        public Guid statusID { get; set; } 

        public string statusAdherence { get; set; } = null!;

        public ICollection<MFUsMedication> mfuMed { get; set; } = new List<MFUsMedication>();    

    }
}
