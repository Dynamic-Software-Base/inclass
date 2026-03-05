namespace Application.Common.Settings.Storage;

public class AzureBlobSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string? BaseUrl { get; set; }
    public bool CreateContainerIfNoExists { get; set; }  = true;
}
