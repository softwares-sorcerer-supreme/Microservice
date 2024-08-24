using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartService.Persistence.Configuration;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(s => new { s.CartId, s.ProductId });

        builder.HasOne<Cart>().WithMany()
           .HasForeignKey(sc => sc.CartId);

        builder.HasIndex(x => new { x.CartId, x.ProductId });
    }
}
