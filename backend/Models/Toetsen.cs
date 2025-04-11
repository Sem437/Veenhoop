using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veenhoop.Models
{
    public class Toetsen
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Vakken")]
        public int VakId { get; set; }

        public required string Naam { get; set; }

        [Required]
        public int Weging { get; set; }
    }
}
