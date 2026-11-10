using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models;

/// <summary>Учащийся (таблица Students).</summary>
public class Student
{
    public int StudentID { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    public DateOnly BirthDate { get; set; }

    public int? CurrentClassID { get; set; }

    [StringLength(250)]
    public string? ContactInfo { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public SchoolClass? CurrentClass { get; set; }
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}
