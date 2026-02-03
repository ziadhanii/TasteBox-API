namespace TasteBox.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public Cart Cart { get; set; } = default!;
    public Product Product { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
