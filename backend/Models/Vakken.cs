using System.ComponentModel.DataAnnotations;

namespace Veenhoop.Models
{
    public class Vakken
    {
        [Key]
        public int Id { get; set; }

        public required string VakNaam { get; set; }
    }
}
