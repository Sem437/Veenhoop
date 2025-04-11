using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veenhoop.Models
{
    public class Cijfers
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Docenten")]
        public int DocentId { get; set; }

        [ForeignKey("Gebruikers")]
        public int GebruikersId { get; set; }

        [ForeignKey("Toetsen")]
        public int ToetsId { get; set; }

        [Required]
        public decimal Cijfer { get; set; }

        public DateTimeOffset Datum { get; set; }

        [Required]
        public int Leerjaar { get; set; }

        [Required]
        public int Periode { get; set; }
    }
}
