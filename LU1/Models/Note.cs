namespace LU1.Models;

public class Note
{
    public Guid Id { get; set; } // NVARCHAR(450)
    public Guid ChildId { get; set; } // NVARCHAR(450)
    public DateTime NoteDate { get; set; } // DATETIME
    public string Title { get; set; } // NVARCHAR(255)
    public string Content { get; set; } // NVARCHAR(MAX)
}