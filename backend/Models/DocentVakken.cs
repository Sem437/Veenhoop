using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veenhoop.Models
{
    public class DocentVakken
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Vakken")]
        public int VakId { get; set; }

        [ForeignKey("Docenten")]
        public int DocentId { get; set; }

        [ForeignKey("Klassen")]
        public int KlasId { get; set; }
    }
}
