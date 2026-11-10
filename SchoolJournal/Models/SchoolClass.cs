using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolJournal.Models;

/// <summary>Класс (таблица Classes, сторона «один»).
/// Назван SchoolClass, чтобы не конфликтовать с ключевым словом class.</summary>
public class SchoolClass
{
    [Key]
    public int ClassID { get; set; }

    public int GradeLevel { get; set; }

    [Column(TypeName = "nchar(1)")]
    public string ClassLetter { get; set; } = string.Empty;

    public int? HomeroomTeacherID { get; set; }

    [StringLength(9)]
    public string AcademicYear { get; set; } = string.Empty;

    public Teacher? HomeroomTeacher { get; set; }
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
