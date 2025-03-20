namespace LU1.Models
{
    public class Child
    {
        public Guid Id { get; set; } // NVARCHAR(450)
        public Guid UserId { get; set; } // NVARCHAR(450)
        public string TrajectName { get; set; } // NVARCHAR(50)
        public string ArtsName { get; set; } // NVARCHAR(50)
        public string Name { get; set; } // NVARCHAR(50)
        public int PrefabId { get; set; } // INT
    }
}