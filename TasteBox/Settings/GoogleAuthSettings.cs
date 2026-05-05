using System.ComponentModel.DataAnnotations;

namespace TasteBox.Settings;

public class GoogleAuthSettings
{
    public const string SectionName = "GoogleAuth";

    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public string ClientSecret { get; set; } = string.Empty;
}
