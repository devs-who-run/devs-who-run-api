using Microsoft.EntityFrameworkCore;

namespace DevsWhoRun.Api;

public class DevsWhoRunDbContext(DbContextOptions<DevsWhoRunDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members { get; set; }
}