namespace LU1.Models
{
    public class Level
    {
        public Guid Id { get; set; }
        public Guid TrajectId { get; set; }
        public int Step { get; set; }
        public string Url { get; set; }
        public string Tekst { get; set; }
        public int TotalSteps { get; set; }
    }
}
