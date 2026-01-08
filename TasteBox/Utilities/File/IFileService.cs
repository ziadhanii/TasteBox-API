namespace TasteBox.Utilities.File;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);

    Task<string> ReplaceAsync(IFormFile file, string existingFilePath, string folderName,
        CancellationToken cancellationToken = default);
}