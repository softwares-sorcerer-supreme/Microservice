namespace CartService.Domain.Entities;

public class Cart
{
    public Guid Id { get; set; }
    public decimal TotalPrice { get; set; }
}