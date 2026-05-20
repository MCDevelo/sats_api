using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Persistence;

public static class DataSeeder
{
    private static readonly Guid TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SchoolId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000003");

    // BCrypt.EnhancedHashPassword("Admin@1234", 12)
    private const string AdminPasswordHash =
        "$2a$12$jrLL2BaC0hsdYTUwJsrQd.f3lRiBNbdYub5NA8YT.33xtPoWgWKRW";

    public static async Task SeedAsync(ApplicationDbContext db, ILogger logger)
    {
        if (await db.Set<SchoolERP.Domain.Entities.Tenant>().AnyAsync())
        {
            logger.LogInformation("Database already seeded, skipping.");
            return;
        }

        logger.LogInformation("Seeding development data...");

        var now = DateTime.UtcNow;

        // ── Tenant ────────────────────────────────────────────────────────────
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO tenants
                (""Id"", ""Name"", ""LegalName"", ""ContactEmail"", ""Plan"", ""IsActive"",
                 ""OnboardingCompleted"", ""OnboardingStep"", ""Country"", ""ContractStart"",
                 ""CreatedAt"", ""UpdatedAt"")
            VALUES
                ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
            TenantId,
            "Colegio San Martín",
            "Colegio San Martín S.R.L.",
            "admin@sanmartin.edu.do",
            "premium",
            true,
            true,
            1,
            "DO",
            now,
            now,
            now);

        // ── School ────────────────────────────────────────────────────────────
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO schools
                (""Id"", ""TenantId"", ""Name"", ""LegalName"", ""Email"",
                 ""PhonePrimary"", ""Province"", ""Municipality"",
                 ""LevelType"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
            VALUES
                ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
            SchoolId,
            TenantId,
            "Colegio San Martín",
            "Colegio San Martín S.R.L.",
            "info@sanmartin.edu.do",
            "809-555-0100",
            "Santo Domingo",
            "Santo Domingo de Guzmán",
            "Primaria",
            true,
            now,
            now);

        // ── Admin User ────────────────────────────────────────────────────────
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO users
                (""Id"", ""TenantId"", ""Email"", ""PasswordHash"", ""Role"",
                 ""IsActive"", ""EmailVerified"", ""PhoneVerified"",
                 ""FailedAttempts"", ""TwoFactorEnabled"", ""PreferredLanguage"",
                 ""CreatedAt"", ""UpdatedAt"")
            VALUES
                ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12})",
            AdminUserId,
            TenantId,
            "admin@sanmartin.edu.do",
            AdminPasswordHash,
            "PlatformAdmin",
            true,
            true,
            false,
            0,
            false,
            "es",
            now,
            now);

        logger.LogInformation("Seed complete. Login: admin@sanmartin.edu.do / Admin@1234");
    }
}
