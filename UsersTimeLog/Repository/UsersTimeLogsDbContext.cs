using Microsoft.EntityFrameworkCore;
using UsersTimeLog.Models;

namespace UsersTimeLog.Repository;

public class UsersTimeLogsDbContext : DbContext
{
    protected readonly IConfiguration _configuration;

    public UsersTimeLogsDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Timelogs>(e =>
        {
            e.HasKey(k => k.Id);
        });

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("TimeLogsDB"), option =>
        {
            option.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName);
        });

        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Users> Users { get; set; }
    public DbSet<Projects> Projects { get; set; }
    public DbSet<Timelogs> Timelogs { get; set; }
}

