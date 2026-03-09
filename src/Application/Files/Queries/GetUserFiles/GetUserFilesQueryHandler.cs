// Application/Files/Queries/GetUserFiles/GetUserFilesQueryHandler.cs

using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using Contract.InClass.Response.Files;
using Domain.File;
using Domain.Users;
using MediatR;
using SharedKernel;

namespace Application.Files.Queries.GetUserFiles;

public class GetUserFilesQueryHandler
    : IRequestHandler<GetUserFilesQuery, ErrorOr<List<UploadFileResult>>>
{
    private readonly IStorageService _storageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IFileUrlResolver _fileUrlResolver;

    public GetUserFilesQueryHandler(
        IStorageService storageService,
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IFileUrlResolver fileUrlResolver)
    {
        _storageService = storageService;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _fileUrlResolver = fileUrlResolver;
    }

    public async Task<ErrorOr<List<UploadFileResult>>> Handle(
        GetUserFilesQuery request,
        CancellationToken cancellationToken)
    {
        ICurrentUser _currentUser = _currentUserService.GetCurrentUser();
        User? user = await _userRepository.GetByIdentityIdAsync(
            _currentUser.Id, cancellationToken);

        if (user is null)
        {
            return Error.NotFound(
                code: "File.UserNotFound",
                description: "User profile not found.");
        }


        ErrorOr<IReadOnlyList<StoredFile>> result = await _storageService.GetByOwnerAsync(user.Id, cancellationToken);

        return result.Match<ErrorOr<List<UploadFileResult>>>(
            files => files.Select(f => new UploadFileResult(
                f.Id.Value,
                f.OriginalFileName,
                _fileUrlResolver.GetAccessUrl(f.Id),
                f.ContentType,
                f.SizeInBytes)).ToList(),
            errors => errors);
    }
}
