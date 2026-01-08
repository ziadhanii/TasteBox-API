namespace TasteBox.Utilities.File;

public static class FileValidationHelper
{
    public static bool IsValidImageSize(IFormFile? file)
    {
        if (file is null)
            return true;

        return file.Length <= FileSettings.MaxImageSizeInBytes;
    }

    public static bool IsValidImageExtension(IFormFile? file)
    {
        if (file is null)
            return true;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return FileSettings.AllowedImageExtensions.Contains(extension);
    }
}