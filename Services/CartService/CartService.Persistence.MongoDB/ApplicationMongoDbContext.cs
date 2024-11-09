using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace CartService.Persistence.MongoDB;

//Use EF Core to interact with MongoDB
//currently not used
public class ApplicationMongoDbContext : DbContext
{
    public DbSet<Cart> Carts { get; init; }
    public DbSet<CartItem> CartItems { get; init; }

    public ApplicationMongoDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>().ToCollection(nameof(Cart)).HasKey(x => x.Id);
        modelBuilder.Entity<CartItem>().ToCollection(nameof(CartItem)).HasKey(x => new { x.CartId, x.ProductId });
    }
}