using Application.Abstractions.Authentication;
using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using Domain.File;
using Domain.Users;
using MediatR;
using SharedKernel;

namespace Application.Files.Commands.UploadFile;

public class UploadFileCommandHandler
    : IRequestHandler<UploadFileCommand, ErrorOr<UploadFileResult>>
{
    private readonly IStorageService _storageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IFileUrlResolver _fileUrlResolver;

    public UploadFileCommandHandler(
        IStorageService storageService,
        ICurrentUserService currentUser,
        IUserRepository userRepository,
        IFileUrlResolver fileUrlResolver)
    {
        _storageService = storageService;
        _currentUserService = currentUser;
        _userRepository = userRepository;
        _fileUrlResolver = fileUrlResolver;
    }

    public async Task<ErrorOr<UploadFileResult>> Handle(
        UploadFileCommand request,
        CancellationToken cancellationToken)
    {
        ICurrentUser _currentUser =  _currentUserService.GetCurrentUser();
        if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.Id.Value.ToString()))
        {
            return Error.Unauthorized(
                code: "File.Unauthorized",
                description: "User is not authenticated.");
        }


        User? user = await _userRepository.GetByIdentityIdAsync(
            _currentUser.Id, cancellationToken);

        if (user is null)
        {
            return Error.NotFound(
                code: "File.UserNotFound",
                description: "User profile not found.");
        }


        ErrorOr<StoredFile> result = await _storageService.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            user.Id,
            cancellationToken);

        return result.Match<ErrorOr<UploadFileResult>>(
            storedFile => new UploadFileResult(
                storedFile.Id.Value,
                storedFile.OriginalFileName,
                _fileUrlResolver.GetAccessUrl(storedFile.Id),
                storedFile.ContentType,
                storedFile.SizeInBytes),
            errors => errors);
    }
}
