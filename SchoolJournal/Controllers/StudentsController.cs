using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Data;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Controllers;

public class StudentsController : Controller
{
    private readonly SchoolJournalContext _context;

    public StudentsController(SchoolJournalContext context)
    {
        _context = context;
    }

    [ResponseCache(CacheProfileName = AppConstants.CacheProfileName)]
    public async Task<IActionResult> Index()
    {
        var query = _context.Students.AsNoTracking();
        int total = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.FullName)
            .ThenBy(s => s.StudentID)
            .Take(AppConstants.MaxRows)
            .Select(s => new StudentRowViewModel
            {
                StudentID = s.StudentID,
                FullName = s.FullName,
                BirthDate = s.BirthDate,
                EnrollmentDate = s.EnrollmentDate,
                ContactInfo = s.ContactInfo,
                // вместо CurrentClassID — смысловые значения из таблицы Classes
                GradeLevel = s.CurrentClass!.GradeLevel,
                ClassLetter = s.CurrentClass!.ClassLetter,
                AcademicYear = s.CurrentClass!.AcademicYear
            })
            .ToListAsync();

        return View(new ListViewModel<StudentRowViewModel>(items, total));
    }
}
