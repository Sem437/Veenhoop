using System.ComponentModel.DataAnnotations;

namespace Veenhoop.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }
        public required string Naam { get; set; }
    }
}
