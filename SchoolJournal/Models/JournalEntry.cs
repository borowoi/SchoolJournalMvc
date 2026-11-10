using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models;

/// <summary>Запись журнала: посещаемость + оценка (таблица Journal, сторона «многие»).</summary>
public class JournalEntry
{
    [Key]
    public int JournalID { get; set; }
    public int? StudentID { get; set; }
    public int? LessonID { get; set; }

    /// <summary>true — присутствовал, false — пропустил.</summary>
    public bool IsPresent { get; set; }

    [StringLength(50)]
    public string? AbsenceReason { get; set; }

    public int? GradeValue { get; set; }

    [StringLength(50)]
    public string? WorkType { get; set; }

    public Student? Student { get; set; }
    public Lesson? Lesson { get; set; }
}
