namespace LU1.Models
{
    [Serializable]
    public class AppointmentItem
    {
        public string Id { get; set; }
        public string AppointmentName { get; set; }
        public string AppointmentDate { get; set; }
        public string NameAttendingDoctor { get; set; }
        public string ChildId { get; set; }
        public string LevelId { get; set; }
        public string StatusLevel { get; set; }
        public int LevelStep { get; set; }
    }
}
