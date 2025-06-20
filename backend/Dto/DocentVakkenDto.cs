namespace Veenhoop.Dto
{
    public class DocentVakkenDto
    {
        public int id { get; set; }
        public int vakId { get; set; }
        public int docentId { get; set; }
        public int klasId { get; set; }
        public required string vakNaam { get; set; }
        public required string docentNaam { get; set; }
        public required string klasNaam { get; set; }
    }
}
