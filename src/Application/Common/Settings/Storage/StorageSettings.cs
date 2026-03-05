using System.ComponentModel.DataAnnotations;

namespace Application.Common.Settings.Storage;

public class StorageSettings
{
    public const string SectionName = "Storage";

    [Required] public AzureBlobSettings AzureBlob { get; set; } = new();
    [Required] public LocalStorageSettings LocalStorage { get; set; }  = new();
    public FileValidationSettings Validation { get; set; } = new FileValidationSettings();


    public bool EnableFallback { get; set; } = true;
    public string PrimaryProvider { get; set; } = "AzureBlob";
}
