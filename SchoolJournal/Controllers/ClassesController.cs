using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Data;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Controllers;

public class ClassesController : Controller
{
    private readonly SchoolJournalContext _context;

    public ClassesController(SchoolJournalContext context)
    {
        _context = context;
    }

    [ResponseCache(CacheProfileName = AppConstants.CacheProfileName)]
    public async Task<IActionResult> Index()
    {
        var query = _context.Classes.AsNoTracking();
        int total = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.AcademicYear)
            .ThenBy(c => c.GradeLevel)
            .ThenBy(c => c.ClassLetter)
            .Take(AppConstants.MaxRows)
            .Select(c => new ClassRowViewModel
            {
                ClassID = c.ClassID,
                GradeLevel = c.GradeLevel,
                ClassLetter = c.ClassLetter,
                AcademicYear = c.AcademicYear,
                // вместо кода внешнего ключа — ФИО классного руководителя
                HomeroomTeacherName = c.HomeroomTeacher != null ? c.HomeroomTeacher.FullName : null,
                StudentsCount = c.Students.Count
            })
            .ToListAsync();

        return View(new ListViewModel<ClassRowViewModel>(items, total));
    }
}
