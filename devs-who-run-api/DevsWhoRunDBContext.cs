using Microsoft.EntityFrameworkCore;

namespace devs_who_run_api;

public class DevsWhoRunDbContext: DbContext
{
    public DbSet<Member> Members { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("MyDatabaseConnectionString");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string is not set in environment variables.");
        }

        optionsBuilder.UseNpgsql(connectionString);
    }
}