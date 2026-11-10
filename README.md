# Школьный журнал (ASP.NET Core MVC + EF Core)

   [![.NET CI](https://github.com/borowoi/SchoolJournalMvc/actions/workflows/dotnet.yml/badge.svg)](https://github.com/borowoi/SchoolJournalMvc/actions/workflows/dotnet.yml)

Лабораторная работа №4 — «Разработка моделей и контроллеров ASP.NET MVC приложения баз данных».

## Возможности

- Модель предметной области «Школьный журнал»: учителя, предметы, нагрузка по часам, классы, учащиеся, уроки, журнал.
- Доступ к SQL Server через Entity Framework Core (Code First, миграции), контекст внедряется через DI.
- Собственный middleware `DbInitializerMiddleware`: применяет миграции и заполняет БД тестовыми данными
  (предметов — 520, классов — 528, уроков — 20 500, записей журнала ≈ 41 000).
- Контроллеры и представления для шести таблиц; вместо внешних ключей выводятся смысловые значения.
- ASP.NET Core Identity (индивидуальные учётные записи): регистрация, вход, роли `Admin` и `User`;
  администратор по умолчанию — `admin@school.local` / `Admin123!`.
- Кэширование вывода атрибутом `[ResponseCache]` по профилю `Default` (вариант 3: 2·3 + 240 = 246 секунд).

## Запуск

```bash
dotnet tool install --global dotnet-ef        # один раз
cd SchoolJournal
dotnet ef migrations add Initial
dotnet ef database update
dotnet run
```

Строка подключения — в `SchoolJournal/appsettings.json` (`ConnectionStrings:DefaultConnection`).

## Структура

| Папка | Назначение |
|---|---|
| `Models` | классы сущностей |
| `Data` | `SchoolJournalContext`, `DbInitializer` |
| `Middleware` | `DbInitializerMiddleware` и метод расширения `UseDbInitializer` |
| `Controllers` | контроллеры |
| `ViewModels` | модели представлений |
| `Views` | представления Razor |
