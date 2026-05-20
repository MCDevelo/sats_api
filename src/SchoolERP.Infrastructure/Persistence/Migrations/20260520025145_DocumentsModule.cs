using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DocumentsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guardians_tenants_TenantId",
                table: "Guardians");

            migrationBuilder.DropForeignKey(
                name: "FK_Guardians_users_UserId",
                table: "Guardians");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentGuardians_Guardians_GuardianId",
                table: "StudentGuardians");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentGuardians_students_StudentId",
                table: "StudentGuardians");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guardians",
                table: "Guardians");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentGuardians",
                table: "StudentGuardians");

            migrationBuilder.DropColumn(
                name: "PhoneWork",
                table: "Guardians");

            migrationBuilder.RenameTable(
                name: "Guardians",
                newName: "guardians");

            migrationBuilder.RenameTable(
                name: "StudentGuardians",
                newName: "student_guardians");

            migrationBuilder.RenameIndex(
                name: "IX_Guardians_UserId",
                table: "guardians",
                newName: "IX_guardians_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Guardians_TenantId",
                table: "guardians",
                newName: "IX_guardians_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentGuardians_StudentId",
                table: "student_guardians",
                newName: "IX_student_guardians_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentGuardians_GuardianId",
                table: "student_guardians",
                newName: "IX_student_guardians_GuardianId");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherCode",
                table: "teachers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Specialization",
                table: "teachers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "teachers",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "teachers",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "teachers",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "teachers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "teachers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcademicTitle",
                table: "teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractEndDate",
                table: "teachers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MinerdCode",
                table: "teachers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingHoursPerWeek",
                table: "teachers",
                type: "integer",
                nullable: false,
                defaultValue: 40);

            migrationBuilder.AlterColumn<string>(
                name: "StudentCode",
                table: "students",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "students",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true,
                defaultValue: "Dominicana",
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5,
                oldNullable: true,
                oldDefaultValue: "DO");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "students",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicalNotes",
                table: "students",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "students",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "students",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BloodType",
                table: "students",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Allergies",
                table: "students",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasSpecialNeeds",
                table: "students",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HealthInsurance",
                table: "students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthInsuranceNumber",
                table: "students",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Municipality",
                table: "students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nse",
                table: "students",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "students",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondLastName",
                table: "students",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "guardians",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Occupation",
                table: "guardians",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "guardians",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "guardians",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "guardians",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "guardians",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "guardians",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "guardians",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinancialResponsible",
                table: "guardians",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneSecondary",
                table: "guardians",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatsApp",
                table: "guardians",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Workplace",
                table: "guardians",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Relationship",
                table: "student_guardians",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "CustodyNotes",
                table: "student_guardians",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasCustodyOrder",
                table: "student_guardians",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmergencyContact",
                table: "student_guardians",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "student_guardians",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_guardians",
                table: "guardians",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_guardians",
                table: "student_guardians",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "announcements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Audience = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AudienceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Priority = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_announcements_schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_announcements_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_announcements_users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "document_requirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    AcceptedFileTypes = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "pdf,jpg,jpeg,png"),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_requirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_document_requirements_schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ParentMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_messages_messages_ParentMessageId",
                        column: x => x.ParentMessageId,
                        principalTable: "messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messages_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messages_users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirementId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_student_documents_document_requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "document_requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_documents_students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_teachers_TenantId_MinerdCode",
                table: "teachers",
                columns: new[] { "TenantId", "MinerdCode" },
                unique: true,
                filter: "\"MinerdCode\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_students_TenantId_Nse",
                table: "students",
                columns: new[] { "TenantId", "Nse" },
                unique: true,
                filter: "\"Nse\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_guardians_TenantId_NationalId",
                table: "guardians",
                columns: new[] { "TenantId", "NationalId" },
                unique: true,
                filter: "\"NationalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_student_guardians_StudentId_GuardianId",
                table: "student_guardians",
                columns: new[] { "StudentId", "GuardianId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_announcements_AuthorId",
                table: "announcements",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_SchoolId_Audience",
                table: "announcements",
                columns: new[] { "SchoolId", "Audience" });

            migrationBuilder.CreateIndex(
                name: "IX_announcements_TenantId_SchoolId_IsPublished",
                table: "announcements",
                columns: new[] { "TenantId", "SchoolId", "IsPublished" });

            migrationBuilder.CreateIndex(
                name: "IX_document_requirements_SchoolId",
                table: "document_requirements",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_document_requirements_TenantId",
                table: "document_requirements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_document_requirements_TenantId_SchoolId",
                table: "document_requirements",
                columns: new[] { "TenantId", "SchoolId" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_ParentMessageId",
                table: "messages",
                column: "ParentMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_RecipientId",
                table: "messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_SenderId",
                table: "messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_TenantId_RecipientId_IsRead",
                table: "messages",
                columns: new[] { "TenantId", "RecipientId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_TenantId_SenderId",
                table: "messages",
                columns: new[] { "TenantId", "SenderId" });

            migrationBuilder.CreateIndex(
                name: "IX_student_documents_RequirementId",
                table: "student_documents",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_student_documents_StudentId",
                table: "student_documents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_student_documents_StudentId_RequirementId",
                table: "student_documents",
                columns: new[] { "StudentId", "RequirementId" });

            migrationBuilder.CreateIndex(
                name: "IX_student_documents_TenantId",
                table: "student_documents",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_guardians_tenants_TenantId",
                table: "guardians",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_guardians_users_UserId",
                table: "guardians",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_student_guardians_guardians_GuardianId",
                table: "student_guardians",
                column: "GuardianId",
                principalTable: "guardians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_student_guardians_students_StudentId",
                table: "student_guardians",
                column: "StudentId",
                principalTable: "students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_guardians_tenants_TenantId",
                table: "guardians");

            migrationBuilder.DropForeignKey(
                name: "FK_guardians_users_UserId",
                table: "guardians");

            migrationBuilder.DropForeignKey(
                name: "FK_student_guardians_guardians_GuardianId",
                table: "student_guardians");

            migrationBuilder.DropForeignKey(
                name: "FK_student_guardians_students_StudentId",
                table: "student_guardians");

            migrationBuilder.DropTable(
                name: "announcements");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "student_documents");

            migrationBuilder.DropTable(
                name: "document_requirements");

            migrationBuilder.DropIndex(
                name: "IX_teachers_TenantId_MinerdCode",
                table: "teachers");

            migrationBuilder.DropIndex(
                name: "IX_students_TenantId_Nse",
                table: "students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_guardians",
                table: "guardians");

            migrationBuilder.DropIndex(
                name: "IX_guardians_TenantId_NationalId",
                table: "guardians");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student_guardians",
                table: "student_guardians");

            migrationBuilder.DropIndex(
                name: "IX_student_guardians_StudentId_GuardianId",
                table: "student_guardians");

            migrationBuilder.DropColumn(
                name: "AcademicTitle",
                table: "teachers");

            migrationBuilder.DropColumn(
                name: "ContractEndDate",
                table: "teachers");

            migrationBuilder.DropColumn(
                name: "MinerdCode",
                table: "teachers");

            migrationBuilder.DropColumn(
                name: "WorkingHoursPerWeek",
                table: "teachers");

            migrationBuilder.DropColumn(
                name: "HasSpecialNeeds",
                table: "students");

            migrationBuilder.DropColumn(
                name: "HealthInsurance",
                table: "students");

            migrationBuilder.DropColumn(
                name: "HealthInsuranceNumber",
                table: "students");

            migrationBuilder.DropColumn(
                name: "Municipality",
                table: "students");

            migrationBuilder.DropColumn(
                name: "Nse",
                table: "students");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "students");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "students");

            migrationBuilder.DropColumn(
                name: "SecondLastName",
                table: "students");

            migrationBuilder.DropColumn(
                name: "IsFinancialResponsible",
                table: "guardians");

            migrationBuilder.DropColumn(
                name: "PhoneSecondary",
                table: "guardians");

            migrationBuilder.DropColumn(
                name: "WhatsApp",
                table: "guardians");

            migrationBuilder.DropColumn(
                name: "Workplace",
                table: "guardians");

            migrationBuilder.DropColumn(
                name: "CustodyNotes",
                table: "student_guardians");

            migrationBuilder.DropColumn(
                name: "HasCustodyOrder",
                table: "student_guardians");

            migrationBuilder.DropColumn(
                name: "IsEmergencyContact",
                table: "student_guardians");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "student_guardians");

            migrationBuilder.RenameTable(
                name: "guardians",
                newName: "Guardians");

            migrationBuilder.RenameTable(
                name: "student_guardians",
                newName: "StudentGuardians");

            migrationBuilder.RenameIndex(
                name: "IX_guardians_UserId",
                table: "Guardians",
                newName: "IX_Guardians_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_guardians_TenantId",
                table: "Guardians",
                newName: "IX_Guardians_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_student_guardians_StudentId",
                table: "StudentGuardians",
                newName: "IX_StudentGuardians_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_student_guardians_GuardianId",
                table: "StudentGuardians",
                newName: "IX_StudentGuardians_GuardianId");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherCode",
                table: "teachers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Specialization",
                table: "teachers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "teachers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "teachers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "teachers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentCode",
                table: "students",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "students",
                type: "character varying(5)",
                maxLength: 5,
                nullable: true,
                defaultValue: "DO",
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60,
                oldNullable: true,
                oldDefaultValue: "Dominicana");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "students",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicalNotes",
                table: "students",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "BloodType",
                table: "students",
                type: "character varying(5)",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Allergies",
                table: "students",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Guardians",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Occupation",
                table: "Guardians",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "Guardians",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Guardians",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Guardians",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Guardians",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Guardians",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Guardians",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneWork",
                table: "Guardians",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Relationship",
                table: "StudentGuardians",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guardians",
                table: "Guardians",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentGuardians",
                table: "StudentGuardians",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Guardians_tenants_TenantId",
                table: "Guardians",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guardians_users_UserId",
                table: "Guardians",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGuardians_Guardians_GuardianId",
                table: "StudentGuardians",
                column: "GuardianId",
                principalTable: "Guardians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGuardians_students_StudentId",
                table: "StudentGuardians",
                column: "StudentId",
                principalTable: "students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
