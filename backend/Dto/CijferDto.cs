namespace Veenhoop.Dto
{
    public class CijferInvoerDto
    {
        public int Leerjaar { get; set; }
        public int Periode { get; set; }
        public int ToetsId { get; set; }
        public int KlasId { get; set; }
        public required List<StudentCijferDto> Cijfers { get; set; } 
    }

    public class StudentCijferDto
    {
        public int StudentId { get; set; } 
        public decimal Cijfer { get; set; }
    }

}
