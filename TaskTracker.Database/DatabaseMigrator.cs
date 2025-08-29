using DbUp;
using System.Reflection;

namespace TaskTracker.Database;

public static class DatabaseMigrator
{
    public static void MigrateDatabase(string connectionString)
    {
        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Migration failed:");
            Console.WriteLine(result.Error);
            Console.ResetColor();
            throw new Exception("Database migration failed", result.Error);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Database successfully migrated.");
        Console.ResetColor();
    }
}
