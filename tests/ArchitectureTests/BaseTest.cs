using System.Reflection;
using Application.Abstractions.Authentication;
using Infrastructure.Database;
using Web.Api;

namespace ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Domain.Domain).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IIdentityService).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
}
