using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models;

/// <summary>Учебный предмет (таблица Subjects, сторона «один»).</summary>
public class Subject
{
    public int SubjectID { get; set; }

    [StringLength(100)]
    public string SubjectName { get; set; } = string.Empty;

    [StringLength(50)]
    public string ExamType { get; set; } = string.Empty;

    public ICollection<SubjectHour> SubjectHours { get; set; } = new List<SubjectHour>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
