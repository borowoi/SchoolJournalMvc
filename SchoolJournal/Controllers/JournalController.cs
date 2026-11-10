using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Data;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Controllers;

public class JournalController : Controller
{
    private readonly SchoolJournalContext _context;

    public JournalController(SchoolJournalContext context)
    {
        _context = context;
    }

    [ResponseCache(CacheProfileName = AppConstants.CacheProfileName)]
    public async Task<IActionResult> Index()
    {
        var query = _context.Journal.AsNoTracking();
        int total = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.Lesson!.LessonDate)
            .ThenByDescending(j => j.JournalID)
            .Take(AppConstants.MaxRows)
            .Select(j => new JournalRowViewModel
            {
                JournalID = j.JournalID,
                IsPresent = j.IsPresent,
                AbsenceReason = j.AbsenceReason,
                GradeValue = j.GradeValue,
                WorkType = j.WorkType,
                // вместо StudentID / LessonID — ФИО учащегося, дата, предмет и класс
                StudentName = j.Student!.FullName,
                LessonDate = j.Lesson!.LessonDate,
                SubjectName = j.Lesson!.Subject!.SubjectName,
                GradeLevel = j.Lesson!.Class!.GradeLevel,
                ClassLetter = j.Lesson!.Class!.ClassLetter
            })
            .ToListAsync();

        return View(new ListViewModel<JournalRowViewModel>(items, total));
    }
}
