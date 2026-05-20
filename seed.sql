-- ============================================================
--  SchoolERP — Script de datos iniciales para pruebas
--  Contraseña del admin: Admin@1234
--  Ejecutar DESPUÉS de: dotnet ef database update
-- ============================================================

BEGIN;

-- ── IDs fijos (para referenciar fácilmente en Postman/Swagger) ──────────────
DO $$
DECLARE
    v_tenant_id   uuid := 'a0000000-0000-0000-0000-000000000001';
    v_school_id   uuid := 'b0000000-0000-0000-0000-000000000001';
    v_year_id     uuid := 'c0000000-0000-0000-0000-000000000001';
    v_admin_id    uuid := 'd0000000-0000-0000-0000-000000000001';
    v_school_admin_id uuid := 'd0000000-0000-0000-0000-000000000002';
    v_now         timestamptz := now();

    -- AcademicPeriod IDs
    v_period_1    uuid := 'f0000000-0000-0000-0000-000000000001';
    v_period_2    uuid := 'f0000000-0000-0000-0000-000000000002';
    v_period_3    uuid := 'f0000000-0000-0000-0000-000000000003';

    -- GradeLevel IDs
    v_gl_pre_1    uuid := 'e0000000-0000-0000-0000-000000000001';
    v_gl_pre_2    uuid := 'e0000000-0000-0000-0000-000000000002';
    v_gl_1ro      uuid := 'e0000000-0000-0000-0000-000000000003';
    v_gl_2do      uuid := 'e0000000-0000-0000-0000-000000000004';
    v_gl_3ro      uuid := 'e0000000-0000-0000-0000-000000000005';
    v_gl_4to      uuid := 'e0000000-0000-0000-0000-000000000006';
    v_gl_5to      uuid := 'e0000000-0000-0000-0000-000000000007';
    v_gl_6to      uuid := 'e0000000-0000-0000-0000-000000000008';
    v_gl_7mo      uuid := 'e0000000-0000-0000-0000-000000000009';
    v_gl_8vo      uuid := 'e0000000-0000-0000-0000-000000000010';
    v_gl_9no      uuid := 'e0000000-0000-0000-0000-000000000011';
    v_gl_10mo     uuid := 'e0000000-0000-0000-0000-000000000012';
    v_gl_11mo     uuid := 'e0000000-0000-0000-0000-000000000013';
    v_gl_12mo     uuid := 'e0000000-0000-0000-0000-000000000014';

BEGIN

-- ── 1. Tenant ────────────────────────────────────────────────────────────────
INSERT INTO tenants (
    "Id", "Name", "LegalName", "ContactEmail", "ContactPhone",
    "Country", "Plan", "IsActive", "OnboardingCompleted", "OnboardingStep",
    "ContractStart", "CreatedAt", "UpdatedAt"
) VALUES (
    v_tenant_id,
    'Colegio San Martín',
    'Colegio San Martín S.R.L.',
    'admin@sanmartin.edu.do',
    '809-555-0100',
    'DO', 'pro', true, true, 5,
    v_now, v_now, v_now
) ON CONFLICT ("Id") DO NOTHING;

-- ── 2. Escuela ───────────────────────────────────────────────────────────────
INSERT INTO schools (
    "Id", "TenantId", "Name", "LegalName", "CodeMinerd",
    "Province", "Municipality", "PhonePrimary", "Email",
    "LevelType", "IsActive", "CreatedAt", "UpdatedAt"
) VALUES (
    v_school_id,
    v_tenant_id,
    'Colegio San Martín',
    'Colegio San Martín S.R.L.',
    '10-001-0001',
    'Santo Domingo', 'Santo Domingo Este',
    '809-555-0100', 'info@sanmartin.edu.do',
    'Primaria', true, v_now, v_now
) ON CONFLICT ("Id") DO NOTHING;

-- ── 3. Usuarios ──────────────────────────────────────────────────────────────
-- PlatformAdmin (acceso total a /api/tenants y gestión global)
-- Password: Admin@1234
INSERT INTO users (
    "Id", "TenantId", "Email", "PasswordHash", "Role",
    "IsActive", "EmailVerified", "PhoneVerified",
    "FailedAttempts", "PreferredLanguage", "TwoFactorEnabled",
    "CreatedAt", "UpdatedAt"
) VALUES (
    v_admin_id,
    v_tenant_id,
    'admin@sanmartin.edu.do',
    '$2a$12$JwVKXVRzYpkJdjAFCswBqOGs0KZ3iPVrmO6JvGoPYpTe44/JG22oe',
    'PlatformAdmin',
    true, true, false, 0, 'es', false,
    v_now, v_now
) ON CONFLICT ("Id") DO NOTHING;

-- SchoolAdmin (acceso a gestión escolar del tenant)
-- Password: Admin@1234
INSERT INTO users (
    "Id", "TenantId", "Email", "PasswordHash", "Role",
    "IsActive", "EmailVerified", "PhoneVerified",
    "FailedAttempts", "PreferredLanguage", "TwoFactorEnabled",
    "CreatedAt", "UpdatedAt"
) VALUES (
    v_school_admin_id,
    v_tenant_id,
    'director@sanmartin.edu.do',
    '$2a$12$JwVKXVRzYpkJdjAFCswBqOGs0KZ3iPVrmO6JvGoPYpTe44/JG22oe',
    'SchoolAdmin',
    true, true, false, 0, 'es', false,
    v_now, v_now
) ON CONFLICT ("Id") DO NOTHING;

-- ── 4. Año académico 2024-2025 ───────────────────────────────────────────────
INSERT INTO academic_years (
    "Id", "TenantId", "SchoolId", "Name",
    "StartDate", "EndDate", "IsActive", "IsCurrent",
    "CreatedAt", "UpdatedAt"
) VALUES (
    v_year_id,
    v_tenant_id,
    v_school_id,
    '2024-2025',
    '2024-09-02 00:00:00+00',
    '2025-06-30 00:00:00+00',
    true, true,
    v_now, v_now
) ON CONFLICT ("Id") DO NOTHING;

-- ── 5. Períodos académicos (3 trimestres) ────────────────────────────────────
INSERT INTO academic_periods (
    "Id", "TenantId", "AcademicYearId", "Name", "Order",
    "StartDate", "EndDate", "IsActive", "GradesPublished",
    "CreatedAt", "UpdatedAt"
) VALUES
    (v_period_1, v_tenant_id, v_year_id, '1er Trimestre', 1,
     '2024-09-02 00:00:00+00', '2024-11-30 00:00:00+00',
     true, false, v_now, v_now),
    (v_period_2, v_tenant_id, v_year_id, '2do Trimestre', 2,
     '2024-12-02 00:00:00+00', '2025-03-15 00:00:00+00',
     true, false, v_now, v_now),
    (v_period_3, v_tenant_id, v_year_id, '3er Trimestre', 3,
     '2025-03-17 00:00:00+00', '2025-06-30 00:00:00+00',
     true, false, v_now, v_now)
ON CONFLICT ("Id") DO NOTHING;

-- ── 6. Niveles de grado (sistema RD: Inicial + Primaria + Secundaria) ─────────
INSERT INTO grade_levels (
    "Id", "TenantId", "SchoolId", "Name", "Order", "EducationLevel", "IsActive", "CreatedAt", "UpdatedAt"
) VALUES
    -- Inicial
    (v_gl_pre_1, v_tenant_id, v_school_id, 'Pre-Kinder',      1,  'Inicial',    true, v_now, v_now),
    (v_gl_pre_2, v_tenant_id, v_school_id, 'Kinder',          2,  'Inicial',    true, v_now, v_now),
    -- Primaria (1ro - 6to)
    (v_gl_1ro,   v_tenant_id, v_school_id, '1ro Primaria',    3,  'Primaria',   true, v_now, v_now),
    (v_gl_2do,   v_tenant_id, v_school_id, '2do Primaria',    4,  'Primaria',   true, v_now, v_now),
    (v_gl_3ro,   v_tenant_id, v_school_id, '3ro Primaria',    5,  'Primaria',   true, v_now, v_now),
    (v_gl_4to,   v_tenant_id, v_school_id, '4to Primaria',    6,  'Primaria',   true, v_now, v_now),
    (v_gl_5to,   v_tenant_id, v_school_id, '5to Primaria',    7,  'Primaria',   true, v_now, v_now),
    (v_gl_6to,   v_tenant_id, v_school_id, '6to Primaria',    8,  'Primaria',   true, v_now, v_now),
    -- Secundaria (7mo - 12mo / Bachillerato)
    (v_gl_7mo,   v_tenant_id, v_school_id, '7mo Secundaria',  9,  'Secundaria', true, v_now, v_now),
    (v_gl_8vo,   v_tenant_id, v_school_id, '8vo Secundaria',  10, 'Secundaria', true, v_now, v_now),
    (v_gl_9no,   v_tenant_id, v_school_id, '9no Secundaria',  11, 'Secundaria', true, v_now, v_now),
    (v_gl_10mo,  v_tenant_id, v_school_id, '1ro Bachillerato', 12, 'Secundaria', true, v_now, v_now),
    (v_gl_11mo,  v_tenant_id, v_school_id, '2do Bachillerato', 13, 'Secundaria', true, v_now, v_now),
    (v_gl_12mo,  v_tenant_id, v_school_id, '3ro Bachillerato', 14, 'Secundaria', true, v_now, v_now)
ON CONFLICT ("Id") DO NOTHING;

END $$;

COMMIT;

-- ============================================================
--  IDs para copiar en Postman / Swagger
-- ============================================================
--  Tenant:         a0000000-0000-0000-0000-000000000001
--  School:         b0000000-0000-0000-0000-000000000001
--  AcademicYear:   c0000000-0000-0000-0000-000000000001
--  Period 1er Tri: f0000000-0000-0000-0000-000000000001
--  Period 2do Tri: f0000000-0000-0000-0000-000000000002
--  Period 3er Tri: f0000000-0000-0000-0000-000000000003
--
--  Login PlatformAdmin:
--    email:    admin@sanmartin.edu.do
--    password: Admin@1234
--
--  Login SchoolAdmin:
--    email:    director@sanmartin.edu.do
--    password: Admin@1234
-- ============================================================
