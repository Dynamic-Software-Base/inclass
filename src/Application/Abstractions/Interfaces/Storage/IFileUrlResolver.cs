using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Abstractions.Interfaces.Storage;

public interface IFileUrlResolver
{
#pragma warning disable CA1055
    string GetAccessUrl(StoredFileId storedFile);
#pragma warning restore CA1055
}
