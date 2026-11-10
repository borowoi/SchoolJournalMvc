using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Models;

namespace SchoolJournal.Data;

/// <summary>
/// Единый контекст данных: таблицы предметной области + таблицы ASP.NET Core Identity
/// (AspNetUsers, AspNetRoles и др.). Схема строится по моделям (соглашения EF Core и аннотации).
/// </summary>
public class SchoolJournalContext : IdentityDbContext<IdentityUser>
{
    public SchoolJournalContext(DbContextOptions<SchoolJournalContext> options) : base(options)
    {
    }

    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<SubjectHour> SubjectHours { get; set; }
    public DbSet<SchoolClass> Classes { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<JournalEntry> Journal { get; set; }
}
