namespace TasteBox.Entities;

public class Cart
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;
    public ICollection<CartItem> CartItems { get; set; } = [];
}
