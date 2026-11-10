using Microsoft.EntityFrameworkCore;

namespace SchoolJournal.Models;

/// <summary>Нагрузка по часам (таблица SubjectHours, составной ключ SubjectID + GradeLevel).</summary>
[PrimaryKey(nameof(SubjectID), nameof(GradeLevel))]
public class SubjectHour
{
    public int SubjectID { get; set; }
    public int GradeLevel { get; set; }
    public int HoursPerWeek { get; set; }

    public Subject? Subject { get; set; }
}
