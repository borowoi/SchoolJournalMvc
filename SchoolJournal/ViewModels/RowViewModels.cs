namespace SchoolJournal.ViewModels;

public class TeacherRowViewModel
{
    public int TeacherID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string QualificationCategory { get; set; } = string.Empty;
    public int WorkloadHours { get; set; }
    public int HomeroomClassesCount { get; set; }
}

public class SubjectRowViewModel
{
    public int SubjectID { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public int GradesCount { get; set; }
}

public class ClassRowViewModel
{
    public int ClassID { get; set; }
    public int GradeLevel { get; set; }
    public string ClassLetter { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string? HomeroomTeacherName { get; set; }
    public int StudentsCount { get; set; }

    public string Name => $"{GradeLevel}{ClassLetter}";
}

public class StudentRowViewModel
{
    public int StudentID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public DateOnly EnrollmentDate { get; set; }
    public string? ContactInfo { get; set; }
    public int GradeLevel { get; set; }
    public string ClassLetter { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;

    public string ClassName => $"{GradeLevel}{ClassLetter}";
}

public class LessonRowViewModel
{
    public int LessonID { get; set; }
    public DateOnly LessonDate { get; set; }
    public int LessonNumber { get; set; }
    public int GradeLevel { get; set; }
    public string ClassLetter { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string? TeacherName { get; set; }
    public string Topic { get; set; } = string.Empty;

    public string ClassName => $"{GradeLevel}{ClassLetter}";
}

public class JournalRowViewModel
{
    public int JournalID { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateOnly LessonDate { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public string ClassLetter { get; set; } = string.Empty;
    public bool IsPresent { get; set; }
    public string? AbsenceReason { get; set; }
    public int? GradeValue { get; set; }
    public string? WorkType { get; set; }

    public string ClassName => $"{GradeLevel}{ClassLetter}";
}
