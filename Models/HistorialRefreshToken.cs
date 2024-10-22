using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models
{
    public class HistorialRefreshToken
    {
        [Key]
        public Guid historialTokenID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? refreshToken { get; set; }

        public DateTime? dateExpiration { get; set; }

        public Account? account { get; set; }
    }
}
