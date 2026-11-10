using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Data;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Controllers;

public class SubjectsController : Controller
{
    private readonly SchoolJournalContext _context;

    public SubjectsController(SchoolJournalContext context)
    {
        _context = context;
    }

    [ResponseCache(CacheProfileName = AppConstants.CacheProfileName)]
    public async Task<IActionResult> Index()
    {
        var query = _context.Subjects.AsNoTracking();
        int total = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.SubjectName)
            .ThenBy(s => s.SubjectID)
            .Take(AppConstants.MaxRows)
            .Select(s => new SubjectRowViewModel
            {
                SubjectID = s.SubjectID,
                SubjectName = s.SubjectName,
                ExamType = s.ExamType,
                GradesCount = s.SubjectHours.Count
            })
            .ToListAsync();

        return View(new ListViewModel<SubjectRowViewModel>(items, total));
    }
}
