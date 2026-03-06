using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Interfaces.Repositories;
using Application.Schools.Contracts;
using Domain.Schools;
using MediatR;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.ValueObjects;
using SharedKernel.ValueObjects.Schools;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Application.Schools.Commands.CreateSchool;

public sealed class CreateSchoolCommandHandler
    : IRequestHandler<CreateSchoolCommand, ErrorOr<CreateSchoolResponse>>
{
        private readonly ICurrentUserService _currentUserService;
        private readonly ISchoolRepository schoolRepo;
        private readonly IMemberShipReposiory memberShipReposiory;
        public CreateSchoolCommandHandler(ICurrentUserService currentUserService, ISchoolRepository schoolRepo, IMemberShipReposiory memberShipReposiory)
        {
            _currentUserService = currentUserService;
            this.schoolRepo = schoolRepo;
            this.memberShipReposiory = memberShipReposiory;
        }

        public async Task<ErrorOr<CreateSchoolResponse>> Handle(
        CreateSchoolCommand request,
        CancellationToken cancellationToken)
        {
            ICurrentUser currentUser = _currentUserService.GetCurrentUser();
            UserId currentUserId = currentUser.Id;
            string? arabicName = request.Name;
            string? description = request.Description;


            CreateSchoolAddress addressDto = request.Address;
            ErrorOr<Address> addressResult = Address.Create(
                addressDto.StreetAddress,
                addressDto.BuildingNumber,
                addressDto.ApartmentNumber,
                addressDto.City,
                addressDto.Province,
                addressDto.Region,
                addressDto.PostalCode,
                null
            );
            if (addressResult.IsError)
            {
                return addressResult.Errors;
            }

            Address address = addressResult.Value;
        //todo : replace the coordinates with an actual service value
            CreateSchoolContactInfo contactInfoDto = request.ContactInfo;
            ErrorOr<SchoolContactInfo> schoolContactInfoResult = SchoolContactInfo.Create(contactInfoDto.PrimaryPhoneNumber, contactInfoDto.SecondaryPhoneNumber,contactInfoDto.Email);
            if (schoolContactInfoResult.IsError)
            {
                return schoolContactInfoResult.Errors;
            }

            SchoolContactInfo schoolContactInfo = schoolContactInfoResult.Value;


            CreateSchoolGradeLevelOffering GradeLevelOfferingDto = request.GradeLevels;
            ErrorOr<GradeLevelOffering> gradeLevelOfferingDto = GradeLevelOffering.Create(
                GradeLevelOfferingDto.hasPreSchool,
                GradeLevelOfferingDto.hasPrimarySchool,
                GradeLevelOfferingDto.hasMiddleSchool,
                GradeLevelOfferingDto.hasHighSchool);

            if (gradeLevelOfferingDto.IsError)
            {
                return gradeLevelOfferingDto.Errors;
            }

            GradeLevelOffering gradeLevelOffering = gradeLevelOfferingDto.Value;


            ErrorOr<School> schoolResult = School.Create(
                SchoolId.New(),
                currentUserId,
                request.Name,
                arabicName,
                currentUserId,
                address,
                schoolContactInfo,
                gradeLevelOffering,
                description
            );
            if (schoolResult.IsError)
            {
                return schoolResult.Errors;
            }
            School school = schoolResult.Value;




            List<CreateSchoolPicturesDto> picturesDto = request.Pictures;
            foreach (CreateSchoolPicturesDto pictureDto in picturesDto)
            {
                var picture = SchoolPicture.Create((StoredFileId)pictureDto.PictureId, null, pictureDto.IsMain);

                ErrorOr<Updated> addPictureToSchoolResult = school.AddPicture(picture);
                if (addPictureToSchoolResult.IsError)
                {
                    return addPictureToSchoolResult.Errors;
                }
            }


        var membership = UserSchoolMembership.Create(
            UserSchoolMembershipId.New(),
            currentUser.Id,
            school.Id,
            UserRole.SchoolOwner,
            isActive: true);


        await schoolRepo.AddAsync(school, cancellationToken);
        await memberShipReposiory.AddAsync(membership, cancellationToken);
        return new CreateSchoolResponse(school.Id, school.Name);
    }


}
