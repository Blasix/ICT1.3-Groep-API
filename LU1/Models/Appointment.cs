namespace LU1_1._3.Models
{
    public class Appointment
    {
        public Guid Id { get; set; } // NVARCHAR(450)
        public string Name { get; set; } // NVARCHAR(50)
        public DateTime Date { get; set; } // DATETIME
        public string? Url { get; set; } // NVARCHAR(256)
        public byte[]? Image { get; set; } // VARBINARY(MAX)
        public Guid ChildId { get; set; } // NVARCHAR(450)
    }
}
