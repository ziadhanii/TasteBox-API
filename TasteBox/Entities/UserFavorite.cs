namespace TasteBox.Entities;

public class UserFavorite
{
    public string UserId { get; init; }
    public int ProductId { get; init; }
    public ApplicationUser User { get; set; } = default!;
    public Product Product { get; set; } = default!;

}