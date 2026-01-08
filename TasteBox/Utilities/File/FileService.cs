namespace TasteBox.Utilities.File;

public class FileService(
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor) : IFileService
{
    public async Task<string> UploadAsync(
        IFormFile file,
        string folderName,
        CancellationToken cancellationToken = default)
    {
        var uploadsFolder = Path.Combine(
            environment.ContentRootPath,
            "wwwroot",
            "uploads",
            folderName);

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        var request = httpContextAccessor.HttpContext!.Request;

        return $"{request.Scheme}://{request.Host}/uploads/{folderName}/{fileName}";
    }

    public async Task<string> ReplaceAsync(
        IFormFile file,
        string existingFilePath,
        string folderName,
        CancellationToken cancellationToken = default)
    {
        var newFilePath = await UploadAsync(
            file,
            folderName,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(existingFilePath))
            Delete(existingFilePath);

        return newFilePath;
    }

    private void Delete(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var relativePath = Uri.TryCreate(filePath, UriKind.Absolute, out var uri) ? uri.AbsolutePath : filePath;

        relativePath = relativePath.TrimStart('/');

        var fullPath = Path.Combine(
            environment.ContentRootPath,
            "wwwroot",
            relativePath);

        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }
    }
}