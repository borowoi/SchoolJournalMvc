namespace SchoolJournal.Middleware;

public static class DbInitializerMiddlewareExtensions
{
    public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DbInitializerMiddleware>();
    }
}
