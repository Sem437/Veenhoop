using System.ComponentModel.DataAnnotations;

namespace Veenhoop.Models
{
    public class Gebruikers
    {
        [Key]
        public int Id { get; set; }

        public required int StudentenNummer { get; set; }

        public required string Voornaam { get; set; }
        
        public string? Tussenvoegsel { get; set; }

        public required string Achternaam { get; set; }

        [Required]
        public DateOnly GeboorteDatum { get; set; }

        public required string Stad { get; set; }

        public required string Adres { get; set; }

        public required string Postcode { get; set; }

        public required string Email { get; set; }

        public required string Wachtwoord { get; set; }
    }
}
