using devs_who_run_api.Models;
using Microsoft.EntityFrameworkCore;

namespace devs_who_run_api;

public class DevsWhoRunDbContext(DbContextOptions<DevsWhoRunDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Conference> Conferences { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresEnum<UserType>();
    }
}