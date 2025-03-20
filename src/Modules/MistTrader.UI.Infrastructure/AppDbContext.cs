using Microsoft.EntityFrameworkCore;

namespace MistTrader.UI.Infrastructure;

public class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dbName = "sqlite.db";
        var dbFullPath = Path.Combine(appDataPath, "MistTrader", dbName);
        
        var connString = $"Data Source={dbFullPath}";

        optionsBuilder.UseSqlite(connString);
    }
}