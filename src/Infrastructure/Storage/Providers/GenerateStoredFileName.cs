namespace Infrastructure.Storage.Providers;

public static class StoredFileHelpers
{
    public static string GenerateStoredFileName(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss",System.Globalization.CultureInfo.CurrentCulture);
        string uniqueId = Guid.NewGuid().ToString("N")[..8];
        return $"{timestamp}_{uniqueId}{extension}";
    }
}
