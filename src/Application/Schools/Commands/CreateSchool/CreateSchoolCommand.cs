using Application.Abstractions.Messaging;
using Application.Schools.Contracts;

namespace Application.Schools.Commands.CreateSchool;

public sealed record CreateSchoolCommand(
    string Name,
    string? Ar_Name,
    string? Description,
    Guid EducationalSystemId,
    List<Guid> SupportedGradeIds,
    CreateSchoolAddress Address,
    CreateSchoolContactInfo ContactInfo,
    List<CreateSchoolPicturesDto> Pictures
) : ICommand<ErrorOr<CreateSchoolResponse>>;

public sealed record CreateSchoolAddress(
    string StreetAddress,
    string? BuildingNumber,
    string? ApartmentNumber,
    string City,
    string Province,
    string Region,
    string PostalCode);

public sealed record CreateSchoolContactInfo(
    string Email,
    string PrimaryPhoneNumber,
    string? SecondaryPhoneNumber);

public sealed record CreateSchoolPicturesDto(
    Guid PictureId,
    bool IsMain);
