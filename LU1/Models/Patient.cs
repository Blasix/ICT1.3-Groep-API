namespace LU1.Models
{
    public class Patient
    {
        public Guid Id { get; set; } // NVARCHAR(450)
        public Guid OuderVoogd_Id { get; set; } // NVARCHAR(450)
        public Guid TrajectID { get; set; } // NVARCHAR(450)
        public Guid ArtsID { get; set; } // NVARCHAR(450)
        public string Voornaam { get; set; } // NVARCHAR(50)
        public string Achternaam { get; set; } // NVARCHAR(50)
    }
}