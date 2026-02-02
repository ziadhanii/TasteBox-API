namespace TasteBox.Entities;

public class UserFavorite
{
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public ApplicationUser User { get; set; } = default!;
    public Product Product { get; set; } = default!;

}