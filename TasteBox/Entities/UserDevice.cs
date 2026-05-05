namespace TasteBox.Entities;

public class UserDevice
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FcmToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
}