using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using System.Data;

namespace ProductService.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public IDbConnection Connection => Database.GetDbConnection();
    public DbSet<Product> Products { get; set; }
}