using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Data;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Controllers;

public class TeachersController : Controller
{
    private readonly SchoolJournalContext _context;

    // Контекст данных внедряется через конструктор (Dependency Injection)
    public TeachersController(SchoolJournalContext context)
    {
        _context = context;
    }

    // Вывод кэшируется по профилю "Default" (2*N+240 секунд)
    [ResponseCache(CacheProfileName = AppConstants.CacheProfileName)]
    public async Task<IActionResult> Index()
    {
        var query = _context.Teachers.AsNoTracking();
        int total = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.FullName)
            .ThenBy(t => t.TeacherID)
            .Take(AppConstants.MaxRows)
            .Select(t => new TeacherRowViewModel
            {
                TeacherID = t.TeacherID,
                FullName = t.FullName,
                QualificationCategory = t.QualificationCategory,
                WorkloadHours = t.WorkloadHours,
                HomeroomClassesCount = t.HomeroomClasses.Count
            })
            .ToListAsync();

        return View(new ListViewModel<TeacherRowViewModel>(items, total));
    }
}
