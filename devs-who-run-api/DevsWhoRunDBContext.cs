using System;
using Microsoft.EntityFrameworkCore;

namespace devs_who_run_api;

public class DevsWhoRunDbContext : DbContext
{
    public DbSet<Member> Members { get; set; }

    public DevsWhoRunDbContext(DbContextOptions<DevsWhoRunDbContext> options) : base(options) { }

}