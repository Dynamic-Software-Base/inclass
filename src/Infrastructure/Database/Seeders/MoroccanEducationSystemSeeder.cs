using Domain.EducationalSystem;
using Domain.EducationalSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Enums;
using SharedKernel.ValueObjects.StronglyTypedIds;

namespace Infrastructure.Database.Seeders;

public sealed class MoroccanEducationSystemSeeder : ISeeder
{
    public int Order => 10; // Runs before any seeder that depends on grade definitions

    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<MoroccanEducationSystemSeeder> _logger;

    public MoroccanEducationSystemSeeder(
        ApplicationDbContext dbContext,
        ILogger<MoroccanEducationSystemSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // ── Step 1: EducationalSystem ────────────────────────────────────────
        EducationalSystem? system = await _dbContext.EducationalSystems
            .FirstOrDefaultAsync(e => e.Code == SeedData.SystemCode, cancellationToken);

        if (system is null)
        {
            system = EducationalSystem.Create(
                EducationalSystemId.From(SeedData.SystemId),
                SeedData.SystemCode,
                SeedData.SystemNameFr,
                SeedData.SystemNameAr);

            await _dbContext.EducationalSystems.AddAsync(system, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded EducationalSystem: {Code}", SeedData.SystemCode);
        }

        // ── Step 2: Cycles ───────────────────────────────────────────────────
        foreach (CycleSeedData cycleSeed in SeedData.Cycles)
        {
            GradeCycleDefinition? cycle = await _dbContext.GradeCycleDefinitions
                .FirstOrDefaultAsync(c => c.Code == cycleSeed.Code, cancellationToken);

            if (cycle is null)
            {
                cycle = GradeCycleDefinition.Create(
                    GradeCycleDefinitionId.From(cycleSeed.Id),
                    EducationalSystemId.From(SeedData.SystemId),
                    cycleSeed.Code,
                    cycleSeed.NameFr,
                    cycleSeed.NameAr,
                    cycleSeed.BroadLevel,
                    cycleSeed.SortOrder);

                await _dbContext.GradeCycleDefinitions.AddAsync(cycle, cancellationToken);
                _logger.LogInformation("  Seeded cycle: {Code}", cycleSeed.Code);
            }

            // ── Step 3: GradeDefinitions within the cycle ────────────────────
            foreach (GradeSeedData gradeSeed in cycleSeed.Grades)
            {
                bool gradeExists = await _dbContext.GradeDefinitions
                    .AnyAsync(g => g.Code == gradeSeed.Code, cancellationToken);

                if (!gradeExists)
                {
                    var grade = GradeDefinition.Create(
                        GradeDefinitionId.From(gradeSeed.Id),
                        GradeCycleDefinitionId.From(cycleSeed.Id),
                        gradeSeed.Code,
                        gradeSeed.NameFr,
                        gradeSeed.NameAr,
                        gradeSeed.SortOrder);

                    await _dbContext.GradeDefinitions.AddAsync(grade, cancellationToken);
                    _logger.LogInformation("    Seeded grade: {Code}", gradeSeed.Code);
                }
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// Seed data — all IDs are fixed GUIDs so they are stable across environments.
// Never regenerate these GUIDs. Add new rows at the bottom of each list.
// ══════════════════════════════════════════════════════════════════════════════

file static class SeedData
{
    // ── EducationalSystem ────────────────────────────────────────────────────

    public const string SystemCode   = "MEN_MA";
    public const string SystemNameFr = "Système national marocain (MEN)";
    public const string SystemNameAr = "المنظومة التربوية الوطنية المغربية";
    public static readonly Guid SystemId = new("10000000-0000-0000-0000-000000000001");

    // ── Cycles + Grades ──────────────────────────────────────────────────────

    public static readonly IReadOnlyList<CycleSeedData> Cycles =
    [
        new CycleSeedData(
            Id:         new("20000000-0000-0000-0000-000000000001"),
            Code:       "CYCLE_PRESCOLAIRE",
            NameFr:     "Cycle préscolaire",
            NameAr:     "التعليم ما قبل المدرسي",
            BroadLevel: GradeLevel.Prescolaire,
            SortOrder:  1,
            Grades:
            [
                new(new("30000000-0000-0000-0000-000000000001"), "PRESC_PET",   "Petite section",   "القسم الصغير",  1),
                new(new("30000000-0000-0000-0000-000000000002"), "PRESC_MOY",   "Moyenne section",  "القسم المتوسط", 2),
                new(new("30000000-0000-0000-0000-000000000003"), "PRESC_GRAND", "Grande section",   "القسم الكبير",  3),
            ]),

        new CycleSeedData(
            Id:         new("20000000-0000-0000-0000-000000000002"),
            Code:       "CYCLE_PRIMAIRE",
            NameFr:     "Cycle primaire",
            NameAr:     "التعليم الابتدائي",
            BroadLevel: GradeLevel.Primaire,
            SortOrder:  2,
            Grades:
            [
                new(new("30000000-0000-0000-0000-000000000011"), "PRIM_A1", "1ère année primaire", "السنة الأولى ابتدائي",  1),
                new(new("30000000-0000-0000-0000-000000000012"), "PRIM_A2", "2ème année primaire", "السنة الثانية ابتدائي", 2),
                new(new("30000000-0000-0000-0000-000000000013"), "PRIM_A3", "3ème année primaire", "السنة الثالثة ابتدائي", 3),
                new(new("30000000-0000-0000-0000-000000000014"), "PRIM_A4", "4ème année primaire", "السنة الرابعة ابتدائي",  4),
                new(new("30000000-0000-0000-0000-000000000015"), "PRIM_A5", "5ème année primaire", "السنة الخامسة ابتدائي", 5),
                new(new("30000000-0000-0000-0000-000000000016"), "PRIM_A6", "6ème année primaire", "السنة السادسة ابتدائي", 6),
            ]),

        new CycleSeedData(
            Id:         new("20000000-0000-0000-0000-000000000003"),
            Code:       "CYCLE_COLLEGIAL",
            NameFr:     "Cycle collégial",
            NameAr:     "التعليم الإعدادي",
            BroadLevel: GradeLevel.Collegial,
            SortOrder:  3,
            Grades:
            [
                new(new("30000000-0000-0000-0000-000000000021"), "COLL_A1", "1ère année collège (7ème)", "السنة الأولى إعدادي",  1),
                new(new("30000000-0000-0000-0000-000000000022"), "COLL_A2", "2ème année collège (8ème)", "السنة الثانية إعدادي", 2),
                new(new("30000000-0000-0000-0000-000000000023"), "COLL_A3", "3ème année collège (9ème)", "السنة الثالثة إعدادي", 3),
            ]),

        new CycleSeedData(
            Id:         new("20000000-0000-0000-0000-000000000004"),
            Code:       "CYCLE_LYCEEN",
            NameFr:     "Cycle lycéen",
            NameAr:     "التعليم الثانوي التأهيلي",
            BroadLevel: GradeLevel.Lyceen,
            SortOrder:  4,
            Grades:
            [
                // Tronc commun — tous les filières
                new(new("30000000-0000-0000-0000-000000000031"), "LYC_TC",          "Tronc commun",                      "الجذع المشترك",                        1 ),

                // 1ère Bac — filières
                new(new("30000000-0000-0000-0000-000000000032"), "LYC_1BAC_LETT",   "1ère Bac — Lettres et Sciences hum.",  "الأولى بكالوريا — آداب وعلوم إنسانية",  2),
                new(new("30000000-0000-0000-0000-000000000033"), "LYC_1BAC_SCI",    "1ère Bac — Sciences",                  "الأولى بكالوريا — العلوم",              3),
                new(new("30000000-0000-0000-0000-000000000034"), "LYC_1BAC_TECHNO", "1ère Bac — Technologie",               "الأولى بكالوريا — التكنولوجيا",        4),

                // 2ème Bac — filières
                new(new("30000000-0000-0000-0000-000000000035"), "LYC_2BAC_LETT_AR",  "2ème Bac — Lettres arabes",            "الثانية بكالوريا — الآداب والعلوم الإنسانية — اللغة العربية", 5),
                new(new("30000000-0000-0000-0000-000000000036"), "LYC_2BAC_LETT_FR",  "2ème Bac — Lettres et Sciences hum.",  "الثانية بكالوريا — الآداب والعلوم الإنسانية",                 6),
                new(new("30000000-0000-0000-0000-000000000037"), "LYC_2BAC_SCI_SM",   "2ème Bac — Sciences Maths",            "الثانية بكالوريا — العلوم الرياضية",                          7),
                new(new("30000000-0000-0000-0000-000000000038"), "LYC_2BAC_SCI_EXP",  "2ème Bac — Sciences expérimentales",   "الثانية بكالوريا — العلوم التجريبية",                         8),
                new(new("30000000-0000-0000-0000-000000000039"), "LYC_2BAC_SCI_TECH", "2ème Bac — Sciences et Technologies",  "الثانية بكالوريا — العلوم والتكنولوجيا",                      9),
                new(new("30000000-0000-0000-0000-000000000040"), "LYC_2BAC_ECO",      "2ème Bac — Sciences économiques",      "الثانية بكالوريا — العلوم الاقتصادية",                       10),
            ]),
    ];
}

// ── Internal seed data record types ─────────────────────────────────────────

file sealed record CycleSeedData(
    Guid Id,
    string Code,
    string NameFr,
    string NameAr,
    GradeLevel BroadLevel,
    int SortOrder,
    IReadOnlyList<GradeSeedData> Grades);

file sealed record GradeSeedData(
    Guid Id,
    string Code,
    string NameFr,
    string NameAr,
    int SortOrder);
