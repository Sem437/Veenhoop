using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veenhoop.Models
{
    public class RolGebruiker
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Gebruikers")]
        public int userId { get; set; }
        [ForeignKey("Rol")]
        public int rolId { get; set; }
    }
}
