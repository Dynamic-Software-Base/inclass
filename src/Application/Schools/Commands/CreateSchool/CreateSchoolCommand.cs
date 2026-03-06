using Application.Abstractions.Messaging;
using Application.Schools.Contracts;
using MediatR;
using SharedKernel.ValueObjects.Schools;

namespace Application.Schools.Commands.CreateSchool;

public sealed record CreateSchoolCommand(
    string Name,
    string? Ar_Name,
    string? Description,
    CreateSchoolAddress Address,
    CreateSchoolContactInfo  ContactInfo,
    CreateSchoolGradeLevelOffering GradeLevels,
    List<CreateSchoolPicturesDto> Pictures) : ICommand<ErrorOr<CreateSchoolResponse>>;



public sealed record CreateSchoolAddress(
string StreetAddress,
string? BuildingNumber,
string? ApartmentNumber,
string City,
string Region,
string PostalCode,
string Province
);

public sealed record CreateSchoolContactInfo(
    string Email,
    string PrimaryPhoneNumber,
    string? SecondaryPhoneNumber
);

public sealed record CreateSchoolGradeLevelOffering(
    bool hasPreSchool,
    bool hasPrimarySchool,
    bool hasMiddleSchool,
    bool hasHighSchool
);

public sealed record CreateSchoolPicturesDto(
    Guid PictureId,
    bool IsMain
    );
