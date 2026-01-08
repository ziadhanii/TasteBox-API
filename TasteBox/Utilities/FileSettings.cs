namespace TasteBox.Utilities;

public static class FileSettings
{
    public const long MaxImageSizeInBytes = 5 * 1024 * 1024;

    public static readonly string[] AllowedImageExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png"
    };
}