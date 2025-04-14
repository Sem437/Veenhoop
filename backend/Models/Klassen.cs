using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veenhoop.Models
{
    public class Klassen
    {
        [Key]
        public int Id { get; set; }

        public required string KlasNaam { get; set; }

        [ForeignKey("Docenten")]
        public int DocentId { get; set; }

        public required Docenten Docent { get; set; }

        public List<Gebruikers> Studenten { get; set; } = new();
    }
}
