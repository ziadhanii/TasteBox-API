namespace TasteBox.Entities;

public class OtpCode
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpireAt { get; set; }
    public bool IsUsed { get; set; }
}