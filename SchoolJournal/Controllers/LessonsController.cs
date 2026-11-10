using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Data;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Controllers;

public class LessonsController : Controller
{
    private readonly SchoolJournalContext _context;

    public LessonsController(SchoolJournalContext context)
    {
        _context = context;
    }

    [ResponseCache(CacheProfileName = AppConstants.CacheProfileName)]
    public async Task<IActionResult> Index()
    {
        var query = _context.Lessons.AsNoTracking();
        int total = await query.CountAsync();

        var items = await query
            .OrderByDescending(l => l.LessonDate)
            .ThenBy(l => l.LessonNumber)
            .ThenBy(l => l.LessonID)
            .Take(AppConstants.MaxRows)
            .Select(l => new LessonRowViewModel
            {
                LessonID = l.LessonID,
                LessonDate = l.LessonDate,
                LessonNumber = l.LessonNumber,
                Topic = l.Topic,
                // вместо ClassID / SubjectID / TeacherID — значения из связанных таблиц («один»)
                GradeLevel = l.Class!.GradeLevel,
                ClassLetter = l.Class!.ClassLetter,
                AcademicYear = l.Class!.AcademicYear,
                SubjectName = l.Subject!.SubjectName,
                TeacherName = l.Teacher != null ? l.Teacher.FullName : null
            })
            .ToListAsync();

        return View(new ListViewModel<LessonRowViewModel>(items, total));
    }
}
