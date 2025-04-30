using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Migrations
{
    public sealed class DBMigrator
    {
        public static void MigrateDB(WebApplication app) {
            using var scoped = app.Services.CreateScope();

            var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scoped.ServiceProvider.GetRequiredService<ILogger<DBMigrator>>();

            try {
                context.Database.Migrate();
                logger.LogInformation("---> Migrations were applied successfully");
            }   catch (System.Exception ex) {
                logger.LogError("---> Could not apply migrations: " + ex.Message);
            } 
        }
    }
}