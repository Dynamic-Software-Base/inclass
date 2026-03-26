using Application.Abstractions.Data;
using Application.Abstractions.Interfaces;
using Domain.EducationalSystem;
using Domain.EducationalSystem.Entities;
using Domain.File;
using Domain.Invitations;
using Domain.Registrations;
using Domain.Schools;
using Domain.Students;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork,IApplicationDbContext
{
    private IDbContextTransaction? _currentTransaction;

    public DbSet<User> Users => Set<User>();
    public DbSet<School> Schools => Set<School>();
    public DbSet<UserSchoolMembership> UserSchoolMemberships => Set<UserSchoolMembership>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<EducationalSystem>      EducationalSystems      { get; set; }
    public DbSet<GradeCycleDefinition>   GradeCycleDefinitions   { get; set; }
    public DbSet<GradeDefinition>        GradeDefinitions        { get; set; }
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();

    public DbSet<RegistrationFormSchema> RegistrationFormSchemas { get; set; }
    public DbSet<RegistrationSession>    RegistrationSessions    { get; set; }
    public DbSet<StudentApplication>    StudentApplications    { get; set; }

    public DbSet<Student>               Students                { get; set; }
    public DbSet<ParentTuteur>          ParentTuteurs           { get; set; }
    public DbSet<StudentExtendedData>   StudentExtendedData     { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        int result = await base.SaveChangesAsync(cancellationToken);


        return result;
    }
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            if (_currentTransaction is not null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch (Exception)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }


        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
    }
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }
    }


}
