using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models;

/// <summary>Учитель (таблица Teachers).</summary>
public class Teacher
{
    public int TeacherID { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    public string QualificationCategory { get; set; } = string.Empty;

    public int WorkloadHours { get; set; }

    public ICollection<SchoolClass> HomeroomClasses { get; set; } = new List<SchoolClass>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
