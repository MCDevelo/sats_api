using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Persistence;

public static class DataSeeder
{
    // ── Platform (sistema) ────────────────────────────────────────────────────
    private static readonly Guid PlatformTenantId = Guid.Parse("00000000-0000-0000-0000-000000000099");
    private static readonly Guid SuperAdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000004");

    // ── San Martín tenant ─────────────────────────────────────────────────────
    private static readonly Guid TenantId   = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SchoolId   = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000003");

    // BCrypt.EnhancedHashPassword("Admin@1234", 12)
    private const string AdminPasswordHash =
        "$2a$12$jrLL2BaC0hsdYTUwJsrQd.f3lRiBNbdYub5NA8YT.33xtPoWgWKRW";

    // BCrypt.EnhancedHashPassword("Pass@1234", 12)
    private const string DefaultPasswordHash =
        "$2a$12$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi";

    public static async Task SeedAsync(ApplicationDbContext db, ILogger logger)
    {
        // Always run (idempotent) — ensures the platform superadmin exists and
        // corrects the role of the school admin if it was previously wrong.
        await SeedPlatformAdminAsync(db, logger);

        // Skip school seed if a school tenant already exists.
        if (await db.Set<Tenant>().AnyAsync(t => t.Id != PlatformTenantId))
        {
            logger.LogInformation("Database already seeded, skipping base seed.");
            await SeedTestDataAsync(db, logger);
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
            "professional",
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

        // ── School Admin User (TenantAdmin — solo ve su colegio) ──────────────
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
            "TenantAdmin",
            true,
            true,
            false,
            0,
            false,
            "es",
            now,
            now);

        logger.LogInformation("Seed complete. School admin: admin@sanmartin.edu.do / Admin@1234");

        await SeedTestDataAsync(db, logger);
    }

    /// <summary>
    /// Crea el tenant plataforma y el usuario superadmin global.
    /// Se ejecuta siempre — es completamente idempotente.
    /// También corrige el rol del admin de San Martín si quedó como PlatformAdmin.
    /// </summary>
    private static async Task SeedPlatformAdminAsync(ApplicationDbContext db, ILogger logger)
    {
        var now = DateTime.UtcNow;

        // ── Tenant plataforma (contenedor del superadmin, no aparece en la lista) ──
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO tenants
                (""Id"", ""Name"", ""LegalName"", ""ContactEmail"", ""Plan"", ""IsActive"",
                 ""OnboardingCompleted"", ""OnboardingStep"", ""Country"", ""ContractStart"",
                 ""CreatedAt"", ""UpdatedAt"")
            VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11})
            ON CONFLICT (""Id"") DO NOTHING",
            PlatformTenantId,
            "SchoolERP Platform",
            "SchoolERP Platform S.R.L.",
            "superadmin@schoolerp.io",
            "platform",
            true,
            true,
            1,
            "DO",
            now, now, now);

        // ── Superadmin global ─────────────────────────────────────────────────
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO users
                (""Id"", ""TenantId"", ""Email"", ""PasswordHash"", ""Role"",
                 ""IsActive"", ""EmailVerified"", ""PhoneVerified"",
                 ""FailedAttempts"", ""TwoFactorEnabled"", ""PreferredLanguage"",
                 ""CreatedAt"", ""UpdatedAt"")
            VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})
            ON CONFLICT (""Id"") DO NOTHING",
            SuperAdminUserId,
            PlatformTenantId,
            "superadmin@schoolerp.io",
            AdminPasswordHash,
            "PlatformAdmin",
            true,
            true,
            false,
            0,
            false,
            "es",
            now, now);

        // ── Corregir rol del admin de San Martín si quedó como PlatformAdmin ──
        await db.Database.ExecuteSqlRawAsync(@"
            UPDATE users
            SET ""Role"" = 'TenantAdmin', ""UpdatedAt"" = {0}
            WHERE ""Email"" = 'admin@sanmartin.edu.do'
              AND ""Role"" = 'PlatformAdmin'",
            now);

        logger.LogInformation(
            "Platform admin ensured. Login: superadmin@schoolerp.io / Admin@1234");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Test data — fixed GUIDs so inserts are idempotent (ON CONFLICT DO NOTHING)
    // ─────────────────────────────────────────────────────────────────────────
    private static readonly Guid DirectorUserId   = Guid.Parse("00000000-0000-0000-0000-000000000010");
    private static readonly Guid SecretaryUserId  = Guid.Parse("00000000-0000-0000-0000-000000000011");
    private static readonly Guid Teacher1UserId   = Guid.Parse("00000000-0000-0000-0000-000000000012");
    private static readonly Guid Teacher2UserId   = Guid.Parse("00000000-0000-0000-0000-000000000013");
    private static readonly Guid AcademicYearId   = Guid.Parse("00000000-0000-0000-0000-000000000020");
    private static readonly Guid Grade1Id  = Guid.Parse("00000000-0000-0000-0000-000000000031");
    private static readonly Guid Grade2Id  = Guid.Parse("00000000-0000-0000-0000-000000000032");
    private static readonly Guid Grade3Id  = Guid.Parse("00000000-0000-0000-0000-000000000033");
    private static readonly Guid Grade4Id  = Guid.Parse("00000000-0000-0000-0000-000000000034");
    private static readonly Guid Grade5Id  = Guid.Parse("00000000-0000-0000-0000-000000000035");
    private static readonly Guid Grade6Id  = Guid.Parse("00000000-0000-0000-0000-000000000036");
    private static readonly Guid Teacher1Id      = Guid.Parse("00000000-0000-0000-0000-000000000041");
    private static readonly Guid Teacher2Id      = Guid.Parse("00000000-0000-0000-0000-000000000042");
    private static readonly Guid Section1AId     = Guid.Parse("00000000-0000-0000-0000-000000000051");
    private static readonly Guid Section2AId     = Guid.Parse("00000000-0000-0000-0000-000000000052");
    private static readonly Guid Section3AId     = Guid.Parse("00000000-0000-0000-0000-000000000053");
    private static readonly Guid Period1Id       = Guid.Parse("00000000-0000-0000-0000-000000000061");
    private static readonly Guid Period2Id       = Guid.Parse("00000000-0000-0000-0000-000000000062");
    private static readonly Guid Period3Id       = Guid.Parse("00000000-0000-0000-0000-000000000063");
    private static readonly Guid Subj1Español    = Guid.Parse("00000000-0000-0000-0000-000000000071");
    private static readonly Guid Subj2Matemática = Guid.Parse("00000000-0000-0000-0000-000000000072");
    private static readonly Guid Subj3Ciencias   = Guid.Parse("00000000-0000-0000-0000-000000000073");
    private static readonly Guid Subj4Sociales   = Guid.Parse("00000000-0000-0000-0000-000000000074");

    private static async Task SeedTestDataAsync(ApplicationDbContext db, ILogger logger)
    {
        if (await db.Set<Student>().AnyAsync())
        {
            logger.LogInformation("Test data already seeded, skipping.");
            return;
        }

        logger.LogInformation("Seeding test data (users, academic year, sections, students)...");

        static DateTime D(int y, int m, int d) => new(y, m, d, 0, 0, 0, DateTimeKind.Utc);
        var now = DateTime.UtcNow;

        // ── Extra users ───────────────────────────────────────────────────────
        var extraUsers = new[]
        {
            (DirectorUserId,  "director@sanmartin.edu.do",   "Director"),
            (SecretaryUserId, "secretaria@sanmartin.edu.do", "Secretary"),
            (Teacher1UserId,  "prof.garcia@sanmartin.edu.do","Teacher"),
            (Teacher2UserId,  "prof.marte@sanmartin.edu.do", "Teacher"),
        };

        foreach (var (id, email, role) in extraUsers)
        {
            await db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO users
                    (""Id"", ""TenantId"", ""Email"", ""PasswordHash"", ""Role"",
                     ""IsActive"", ""EmailVerified"", ""PhoneVerified"",
                     ""FailedAttempts"", ""TwoFactorEnabled"", ""PreferredLanguage"",
                     ""CreatedAt"", ""UpdatedAt"")
                VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})
                ON CONFLICT (""Id"") DO NOTHING",
                id, TenantId, email, DefaultPasswordHash, role,
                true, true, false, 0, false, "es", now, now);
        }

        // ── Academic year 2024-2025 ───────────────────────────────────────────
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO academic_years
                (""Id"", ""TenantId"", ""SchoolId"", ""Name"",
                 ""StartDate"", ""EndDate"", ""IsActive"", ""IsCurrent"",
                 ""CreatedAt"", ""UpdatedAt"")
            VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9})
            ON CONFLICT (""Id"") DO NOTHING",
            AcademicYearId, TenantId, SchoolId, "2024-2025",
            D(2024, 9, 1), D(2025, 6, 30), true, true, now, now);

        // ── Grade levels ──────────────────────────────────────────────────────
        var grades = new[]
        {
            (Grade1Id, "1ro Primaria", 1),
            (Grade2Id, "2do Primaria", 2),
            (Grade3Id, "3ro Primaria", 3),
            (Grade4Id, "4to Primaria", 4),
            (Grade5Id, "5to Primaria", 5),
            (Grade6Id, "6to Primaria", 6),
        };

        foreach (var (gId, gName, gOrder) in grades)
        {
            await db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO grade_levels
                    (""Id"", ""TenantId"", ""SchoolId"", ""Name"",
                     ""Order"", ""EducationLevel"", ""IsActive"",
                     ""CreatedAt"", ""UpdatedAt"")
                VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8})
                ON CONFLICT (""Id"") DO NOTHING",
                gId, TenantId, SchoolId, gName,
                gOrder, (int)EducationLevel.Primaria, true, now, now);
        }

        // ── Teachers ──────────────────────────────────────────────────────────
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO teachers
                (""Id"", ""TenantId"", ""SchoolId"", ""FirstName"", ""LastName"",
                 ""Email"", ""Phone"", ""Gender"", ""ContractType"",
                 ""HireDate"", ""WorkingHoursPerWeek"", ""IsActive"",
                 ""UserId"", ""CreatedAt"", ""UpdatedAt"")
            VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14})
            ON CONFLICT (""Id"") DO NOTHING",
            Teacher1Id, TenantId, SchoolId, "Carlos", "García",
            "prof.garcia@sanmartin.edu.do", "809-555-0201", (int)Gender.M,
            (int)ContractType.Permanent, D(2020, 1, 15),
            40, true, Teacher1UserId, now, now);

        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO teachers
                (""Id"", ""TenantId"", ""SchoolId"", ""FirstName"", ""LastName"",
                 ""Email"", ""Phone"", ""Gender"", ""ContractType"",
                 ""HireDate"", ""WorkingHoursPerWeek"", ""IsActive"",
                 ""UserId"", ""CreatedAt"", ""UpdatedAt"")
            VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14})
            ON CONFLICT (""Id"") DO NOTHING",
            Teacher2Id, TenantId, SchoolId, "Ana", "Marte",
            "prof.marte@sanmartin.edu.do", "809-555-0202", (int)Gender.F,
            (int)ContractType.Permanent, D(2019, 3, 1),
            40, true, Teacher2UserId, now, now);

        // ── Sections ──────────────────────────────────────────────────────────
        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO sections
                (""Id"", ""TenantId"", ""SchoolId"", ""GradeLevelId"", ""AcademicYearId"",
                 ""Name"", ""Shift"", ""Capacity"", ""Classroom"",
                 ""HomeTeacherId"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
            VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})
            ON CONFLICT (""Id"") DO NOTHING",
            Section1AId, TenantId, SchoolId, Grade1Id, AcademicYearId,
            "A", (int)Shift.Matutino, 35, "Aula 101", Teacher1Id, true, now, now);

        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO sections
                (""Id"", ""TenantId"", ""SchoolId"", ""GradeLevelId"", ""AcademicYearId"",
                 ""Name"", ""Shift"", ""Capacity"", ""Classroom"",
                 ""HomeTeacherId"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
            VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})
            ON CONFLICT (""Id"") DO NOTHING",
            Section2AId, TenantId, SchoolId, Grade2Id, AcademicYearId,
            "A", (int)Shift.Matutino, 35, "Aula 201", Teacher2Id, true, now, now);

        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO sections
                (""Id"", ""TenantId"", ""SchoolId"", ""GradeLevelId"", ""AcademicYearId"",
                 ""Name"", ""Shift"", ""Capacity"", ""Classroom"",
                 ""HomeTeacherId"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
            VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})
            ON CONFLICT (""Id"") DO NOTHING",
            Section3AId, TenantId, SchoolId, Grade3Id, AcademicYearId,
            "A", (int)Shift.Matutino, 35, "Aula 301", null, true, now, now);

        // ── Academic periods ─────────────────────────────────────────────────
        var periods = new[]
        {
            (Period1Id, "1er Trimestre", 1, D(2024,9,1),  D(2024,12,20)),
            (Period2Id, "2do Trimestre", 2, D(2025,1,13), D(2025,3,28)),
            (Period3Id, "3er Trimestre", 3, D(2025,4,7),  D(2025,6,27)),
        };
        foreach (var (pId, pName, pNum, pStart, pEnd) in periods)
        {
            await db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO academic_periods
                    (""Id"", ""AcademicYearId"", ""Name"", ""PeriodNumber"",
                     ""StartDate"", ""EndDate"", ""IsActive"", ""GradesPublished"",
                     ""CreatedAt"", ""UpdatedAt"")
                VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9})
                ON CONFLICT (""Id"") DO NOTHING",
                pId, AcademicYearId, pName, pNum, pStart, pEnd, true, false, now, now);
        }

        // ── Subjects (for Grade 1) ────────────────────────────────────────────
        var subjects = new[]
        {
            (Subj1Español,    Grade1Id, "Español",     "ESP", 8, true),
            (Subj2Matemática, Grade1Id, "Matemáticas", "MAT", 8, true),
            (Subj3Ciencias,   Grade1Id, "Ciencias",    "CIE", 6, true),
            (Subj4Sociales,   Grade1Id, "Sociales",    "SOC", 6, true),
        };
        foreach (var (sId, gId, sName, sCode, sHours, sReq) in subjects)
        {
            await db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO subjects
                    (""Id"", ""TenantId"", ""SchoolId"", ""GradeLevelId"",
                     ""Name"", ""Code"", ""WeeklyHours"", ""IsRequired"", ""IsActive"",
                     ""CreatedAt"", ""UpdatedAt"")
                VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10})
                ON CONFLICT (""Id"") DO NOTHING",
                sId, TenantId, SchoolId, gId, sName, sCode, sHours, sReq, true, now, now);
        }

                // ── Students + enrollments ─────────────────────────────────────────────
        var students = new[]
        {
            // (firstName, lastName, dob, gender, code, sectionId)
            ("María",     "Pérez",    D(2017,3,12), Gender.F, "EST-001", Section1AId),
            ("Juan",      "Rodríguez",D(2017,7,5),  Gender.M, "EST-002", Section1AId),
            ("Valentina", "Díaz",     D(2017,1,22), Gender.F, "EST-003", Section1AId),
            ("Carlos",    "Fernández",D(2017,9,30), Gender.M, "EST-004", Section1AId),
            ("Sofía",     "López",    D(2016,4,18), Gender.F, "EST-005", Section2AId),
            ("Miguel",    "Martínez", D(2016,11,9), Gender.M, "EST-006", Section2AId),
            ("Isabella",  "González", D(2016,6,3),  Gender.F, "EST-007", Section2AId),
            ("Andrés",    "Torres",   D(2016,2,27), Gender.M, "EST-008", Section2AId),
            ("Camila",    "Ramírez",  D(2015,8,14), Gender.F, "EST-009", Section3AId),
            ("Luis",      "Jiménez",  D(2015,5,20), Gender.M, "EST-010", Section3AId),
        };

        foreach (var (firstName, lastName, dob, gender, code, sectionId) in students)
        {
            var studentId = Guid.NewGuid();

            await db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO students
                    (""Id"", ""TenantId"", ""SchoolId"", ""FirstName"", ""LastName"",
                     ""DateOfBirth"", ""Gender"", ""StudentCode"",
                     ""Nationality"", ""IsActive"", ""HasSpecialNeeds"",
                     ""CreatedAt"", ""UpdatedAt"")
                VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})",
                studentId, TenantId, SchoolId, firstName, lastName,
                dob, (int)gender, code,
                "Dominicana", true, false, now, now);

            await db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO enrollments
                    (""Id"", ""TenantId"", ""StudentId"", ""SectionId"", ""AcademicYearId"",
                     ""Status"", ""EnrollmentDate"", ""CreatedAt"", ""UpdatedAt"")
                VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8})",
                Guid.NewGuid(), TenantId, studentId, sectionId, AcademicYearId,
                (int)EnrollmentStatus.Active,
                new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                now, now);
        }

        logger.LogInformation(
            "Test data seeded: 4 users, 1 academic year, 3 periods, 6 grade levels, 4 subjects (Grado 1), 3 sections, 2 teachers, 10 students.");
        logger.LogInformation("Extra logins (password = Pass@1234):");
        logger.LogInformation("  director@sanmartin.edu.do    (Director)");
        logger.LogInformation("  secretaria@sanmartin.edu.do  (Secretary)");
        logger.LogInformation("  prof.garcia@sanmartin.edu.do (Teacher)");
        logger.LogInformation("  prof.marte@sanmartin.edu.do  (Teacher)");
    }
}
