using Application.Abstractions.Messaging;
using Contract.InClass.Response.School;

namespace Application.Schools.Queries.GetNearestSchools;

public sealed record GetNearestSchoolsQuery(double Latitude,double Longitude,int Count = 5) : IPublicQuery<ErrorOr<List<NearestSchoolDto>>>;
