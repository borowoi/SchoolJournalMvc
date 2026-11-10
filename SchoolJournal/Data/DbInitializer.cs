using Microsoft.EntityFrameworkCore;
using SchoolJournal.Models;

namespace SchoolJournal.Data;

/// <summary>
/// Заполнение базы данных тестовым набором записей.
/// Объёмы: Subjects = 520 (&gt;500), Classes = 528 (&gt;500), Lessons = 20 500 (&gt;20 000),
/// Journal ≈ 41 000, Students = 2 640, Teachers = 40.
/// Генератор Random инициализирован константой, поэтому данные воспроизводимы.
/// </summary>
public static class DbInitializer
{
    private const int TeachersCount = 40;
    private const int FirstAcademicYear = 2019;     // 2019/2020 ... 2026/2027
    private const int AcademicYearsCount = 8;
    private const int StudentsPerClass = 5;
    private const int LessonsCount = 20_500;
    private const int JournalEntriesPerLesson = 2;
    private const int BatchSize = 5_000;

    private sealed record ClassInfo(int ClassId, int Grade, int StartYear);

    #region Словари для генерации
    private static readonly string[] MaleSurnames = { "Иванов", "Петров", "Сидоров", "Кузнецов", "Смирнов", "Попов", "Васильев", "Новиков", "Морозов", "Волков", "Соколов", "Лебедев" };
    private static readonly string[] FemaleSurnames = { "Иванова", "Петрова", "Сидорова", "Кузнецова", "Смирнова", "Попова", "Васильева", "Новикова", "Морозова", "Волкова", "Соколова", "Лебедева" };
    private static readonly string[] MaleNames = { "Александр", "Дмитрий", "Максим", "Иван", "Артём", "Михаил", "Никита", "Андрей", "Егор", "Кирилл" };
    private static readonly string[] FemaleNames = { "Анна", "Мария", "Елена", "Ольга", "Татьяна", "Наталья", "София", "Дарья", "Полина", "Виктория" };
    private static readonly string[] MalePatronymics = { "Александрович", "Дмитриевич", "Иванович", "Сергеевич", "Андреевич", "Николаевич", "Михайлович", "Петрович" };
    private static readonly string[] FemalePatronymics = { "Александровна", "Дмитриевна", "Ивановна", "Сергеевна", "Андреевна", "Николаевна", "Михайловна", "Петровна" };

    private static readonly string[] Categories = { "Высшая", "Первая", "Вторая", "Без категории" };
    private static readonly string[] ExamTypes = { "Экзамен", "Зачет", "Дифференцированный зачет" };
    private static readonly string[] ClassLetters = { "А", "Б", "В", "Г", "Д", "Е" };

    private static readonly string[] SubjectBases =
    {
        "Математика", "Алгебра", "Геометрия", "Русский язык", "Литература", "Белорусский язык",
        "Английский язык", "Немецкий язык", "История", "Обществоведение", "География", "Биология",
        "Химия", "Физика", "Информатика", "Астрономия", "Физическая культура", "Музыка",
        "Изобразительное искусство", "Трудовое обучение", "Черчение", "Основы безопасности жизнедеятельности",
        "Экология", "Мировая художественная культура", "Допризывная подготовка", "Всемирная история"
    };

    private static readonly string[] SubjectVariants =
    {
        "базовый курс", "углублённый курс", "профильный курс", "практикум", "факультатив",
        "модуль 1", "модуль 2", "модуль 3", "семинар", "консультация",
        "проектная деятельность", "исследовательский курс", "подготовка к экзамену", "основы", "введение",
        "прикладной курс", "лабораторный практикум", "интенсив", "дополнительные главы", "итоговое повторение"
    };

    private static readonly string[] TopicTemplates =
    {
        "Новая тема", "Закрепление материала", "Практическая работа", "Повторение пройденного",
        "Обобщающий урок", "Решение задач", "Работа над ошибками", "Урок-семинар"
    };

    private static readonly int[] Grades = { 2, 3, 3, 4, 4, 4, 5, 5, 5, 5 };   // смещение к «хорошим» оценкам
    private static readonly string[] WorkTypes = { "Устный ответ", "Самостоятельная", "Контрольная", "Лабораторная" };
    private static readonly string[] AbsenceReasons = { "Болезнь", "Уважительная", "Без уважительной причины" };
    #endregion

    public static async Task InitializeAsync(SchoolJournalContext db, CancellationToken ct = default)
    {
        // Идемпотентность: если данные уже есть — ничего не делаем
        if (await db.Teachers.AnyAsync(ct))
        {
            return;
        }

        var rnd = new Random(1);
        db.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

        bool autoDetect = db.ChangeTracker.AutoDetectChangesEnabled;
        db.ChangeTracker.AutoDetectChangesEnabled = false;

        // Вся инициализация — в одной транзакции: либо заполнено всё, либо ничего
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            // 1. Учителя
            var teachers = Enumerable.Range(1, TeachersCount).Select(_ => new Teacher
            {
                FullName = RandomFullName(rnd),
                QualificationCategory = Pick(Categories, rnd),
                WorkloadHours = rnd.Next(10, 37)
            }).ToList();
            await SaveInBatchesAsync(db, teachers, ct);
            var teacherIds = teachers.Select(t => t.TeacherID).ToList();

            // 2. Предметы (26 × 20 = 520)
            var subjects = new List<Subject>();
            foreach (var b in SubjectBases)
            {
                foreach (var v in SubjectVariants)
                {
                    subjects.Add(new Subject { SubjectName = $"{b}: {v}", ExamType = Pick(ExamTypes, rnd) });
                }
            }
            await SaveInBatchesAsync(db, subjects, ct);

            // 3. Нагрузка по часам: каждый предмет изучается в 3 случайных классах (параллелях)
            var gradeSubjects = Enumerable.Range(1, 11).ToDictionary(g => g, _ => new List<int>());
            var hours = new List<SubjectHour>();
            foreach (var s in subjects)
            {
                foreach (var g in Enumerable.Range(1, 11).OrderBy(_ => rnd.Next()).Take(3))
                {
                    hours.Add(new SubjectHour { SubjectID = s.SubjectID, GradeLevel = g, HoursPerWeek = rnd.Next(1, 6) });
                    gradeSubjects[g].Add(s.SubjectID);
                }
            }
            await SaveInBatchesAsync(db, hours, ct);

            // 4. Классы (8 лет × 11 параллелей × 6 букв = 528)
            var classes = new List<SchoolClass>();
            for (int y = FirstAcademicYear; y < FirstAcademicYear + AcademicYearsCount; y++)
            {
                for (int grade = 1; grade <= 11; grade++)
                {
                    foreach (var letter in ClassLetters)
                    {
                        classes.Add(new SchoolClass
                        {
                            GradeLevel = grade,
                            ClassLetter = letter,
                            AcademicYear = $"{y}/{y + 1}",
                            HomeroomTeacherID = rnd.Next(10) == 0 ? null : (int?)Pick(teacherIds, rnd)
                        });
                    }
                }
            }
            await SaveInBatchesAsync(db, classes, ct);
            var classInfos = classes
                .Select(c => new ClassInfo(c.ClassID, c.GradeLevel, int.Parse(c.AcademicYear[..4])))
                .ToList();

            // 5. Учащиеся
            var students = new List<Student>();
            foreach (var c in classInfos)
            {
                for (int i = 0; i < StudentsPerClass; i++)
                {
                    students.Add(new Student
                    {
                        FullName = RandomFullName(rnd),
                        BirthDate = new DateOnly(c.StartYear - 6 - c.Grade, rnd.Next(1, 13), rnd.Next(1, 29)),
                        CurrentClassID = c.ClassId,
                        ContactInfo = rnd.Next(5) == 0 ? null : $"+375 29 {rnd.Next(100, 1000)}-{rnd.Next(10, 100)}-{rnd.Next(10, 100)}",
                        EnrollmentDate = new DateOnly(c.StartYear, 9, 1)
                    });
                }
            }
            await SaveInBatchesAsync(db, students, ct);

            // 6. Уроки (> 20 000): предмет выбирается из тех, что есть в нагрузке для параллели класса
            var lessons = new List<Lesson>(LessonsCount);
            for (int i = 0; i < LessonsCount; i++)
            {
                var c = classInfos[rnd.Next(classInfos.Count)];
                lessons.Add(new Lesson
                {
                    ClassID = c.ClassId,
                    SubjectID = Pick(gradeSubjects[c.Grade], rnd),
                    TeacherID = rnd.Next(10) == 0 ? null : (int?)Pick(teacherIds, rnd),
                    LessonDate = RandomSchoolDay(rnd, c.StartYear),
                    LessonNumber = rnd.Next(1, 9),
                    Topic = $"{Pick(TopicTemplates, rnd)} №{rnd.Next(1, 41)}"
                });
            }
            await SaveInBatchesAsync(db, lessons, ct);

            // 7. Журнал: по нескольку записей на урок для учащихся этого класса
            var studentsByClass = students
                .GroupBy(s => s.CurrentClassID!.Value)
                .ToDictionary(g => g.Key, g => g.Select(s => s.StudentID).ToList());

            var journal = new List<JournalEntry>(LessonsCount * JournalEntriesPerLesson);
            foreach (var lesson in lessons)
            {
                var pool = studentsByClass[lesson.ClassID!.Value];
                foreach (var studentId in pool.OrderBy(_ => rnd.Next()).Take(JournalEntriesPerLesson))
                {
                    bool present = rnd.Next(100) >= 12;
                    var entry = new JournalEntry
                    {
                        StudentID = studentId,
                        LessonID = lesson.LessonID,
                        IsPresent = present
                    };

                    if (present)
                    {
                        if (rnd.Next(100) < 70)
                        {
                            entry.GradeValue = Pick(Grades, rnd);
                            entry.WorkType = Pick(WorkTypes, rnd);
                        }
                    }
                    else
                    {
                        // CHECK-ограничение: при пропуске причина обязательна, оценки нет
                        entry.AbsenceReason = Pick(AbsenceReasons, rnd);
                    }

                    journal.Add(entry);
                }
            }
            await SaveInBatchesAsync(db, journal, ct);

            await tx.CommitAsync(ct);
        }
        finally
        {
            db.ChangeTracker.AutoDetectChangesEnabled = autoDetect;
            db.ChangeTracker.Clear();
        }
    }

    /// <summary>Сохраняет сущности пакетами и очищает ChangeTracker (значения ключей в объектах сохраняются).</summary>
    private static async Task SaveInBatchesAsync<T>(SchoolJournalContext db, IReadOnlyList<T> entities, CancellationToken ct)
        where T : class
    {
        for (int i = 0; i < entities.Count; i += BatchSize)
        {
            db.Set<T>().AddRange(entities.Skip(i).Take(BatchSize));
            await db.SaveChangesAsync(ct);
            db.ChangeTracker.Clear();
        }
    }

    private static T Pick<T>(IReadOnlyList<T> items, Random rnd) => items[rnd.Next(items.Count)];

    private static string RandomFullName(Random rnd)
    {
        return rnd.Next(2) == 0
            ? $"{Pick(MaleSurnames, rnd)} {Pick(MaleNames, rnd)} {Pick(MalePatronymics, rnd)}"
            : $"{Pick(FemaleSurnames, rnd)} {Pick(FemaleNames, rnd)} {Pick(FemalePatronymics, rnd)}";
    }

    /// <summary>Случайный будний день учебного года (с 1 сентября по 25 мая).</summary>
    private static DateOnly RandomSchoolDay(Random rnd, int startYear)
    {
        var start = new DateTime(startYear, 9, 1);
        var end = new DateTime(startYear + 1, 5, 25);
        var day = start.AddDays(rnd.Next((end - start).Days + 1));

        if (day.DayOfWeek == DayOfWeek.Saturday) day = day.AddDays(2);
        else if (day.DayOfWeek == DayOfWeek.Sunday) day = day.AddDays(1);

        return DateOnly.FromDateTime(day);
    }
}
