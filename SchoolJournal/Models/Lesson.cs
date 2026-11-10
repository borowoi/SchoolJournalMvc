using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models;

/// <summary>Урок (таблица Lessons, сторона «многие»).</summary>
public class Lesson
{
    public int LessonID { get; set; }
    public int? ClassID { get; set; }
    public int? SubjectID { get; set; }
    public int? TeacherID { get; set; }
    public DateOnly LessonDate { get; set; }
    public int LessonNumber { get; set; }

    [StringLength(250)]
    public string Topic { get; set; } = string.Empty;

    public SchoolClass? Class { get; set; }
    public Subject? Subject { get; set; }
    public Teacher? Teacher { get; set; }
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}
