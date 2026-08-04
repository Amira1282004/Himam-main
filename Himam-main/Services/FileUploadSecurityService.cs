namespace Himam_main.Services;

public class FileUploadSecurityService
{
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static readonly string[] AllowedDocumentExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
    private static readonly string[] AllowedMimeTypes = 
    {
        "image/jpeg", "image/png", "image/gif", "image/webp",
        "application/pdf", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };

    public (bool IsValid, string ErrorMessage) ValidateFile(IFormFile file)
    {
        // Check file size
        if (file.Length > MaxFileSize)
            return (false, $"File size exceeds maximum allowed size of {MaxFileSize / (1024 * 1024)}MB");

        if (file.Length == 0)
            return (false, "File is empty");

        // Check file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension) && !AllowedDocumentExtensions.Contains(extension))
            return (false, "File type not allowed");

        // Check MIME type
        var contentType = file.ContentType.ToLowerInvariant();
        if (!AllowedMimeTypes.Contains(contentType))
            return (false, "Invalid file type");

        // Validate extension matches MIME type
        if (!IsExtensionMatchesMimeType(extension, contentType))
            return (false, "File extension does not match content type");

        // Check for malicious patterns in filename
        if (ContainsMaliciousPatterns(file.FileName))
            return (false, "Filename contains invalid characters");

        return (true, string.Empty);
    }

    public string GenerateSafeFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var safeName = Guid.NewGuid().ToString("N") + extension;
        return safeName;
    }

    private bool IsExtensionMatchesMimeType(string extension, string mimeType)
    {
        return (extension, mimeType) switch
        {
            (".jpg", "image/jpeg") => true,
            (".jpeg", "image/jpeg") => true,
            (".png", "image/png") => true,
            (".gif", "image/gif") => true,
            (".webp", "image/webp") => true,
            (".pdf", "application/pdf") => true,
            (".doc", "application/msword") => true,
            (".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document") => true,
            (".xls", "application/vnd.ms-excel") => true,
            (".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") => true,
            _ => false
        };
    }

    private bool ContainsMaliciousPatterns(string filename)
    {
        var maliciousPatterns = new[] { "..", "/", "\\", "<", ">", ":", "*", "?", "\"", "|" };
        return maliciousPatterns.Any(pattern => filename.Contains(pattern));
    }
}
