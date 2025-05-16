namespace Veenhoop.Dto
{
    public class StudentDto
    {
        public int    Id   { get; set; }
        public required string Voornaam { get; set; }
        public string? Tussenvoegsel { get; set; }
        public required string Achternaam{ get; set; }
    }

    public class KlasStudentDto
    {
        public int KlasId { get; set; }
        public required string KlasNaam { get; set; }
        public int vakId { get; set; }
        public required string VakNaam { get; set; }
        public List<StudentDto> Studenten { get; set; } = new();
    }
}
