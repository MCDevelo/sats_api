using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionAndTeacherConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attendance_records_Sections_SectionId",
                table: "attendance_records");

            migrationBuilder.DropForeignKey(
                name: "FK_attendance_records_Teachers_TeacherId",
                table: "attendance_records");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollments_Sections_SectionId",
                table: "enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_entries_Teachers_TeacherId",
                table: "grade_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_AcademicYears_AcademicYearId",
                table: "Sections");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_GradeLevels_GradeLevelId",
                table: "Sections");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Teachers_HomeTeacherId",
                table: "Sections");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_schools_SchoolId",
                table: "Sections");

            migrationBuilder.DropForeignKey(
                name: "FK_Sections_tenants_TenantId",
                table: "Sections");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_Sections_SectionId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_Teachers_TeacherId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_schools_SchoolId",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_tenants_TenantId",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_users_UserId",
                table: "Teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sections",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_GradeLevelId",
                table: "Sections");

            migrationBuilder.RenameTable(
                name: "Teachers",
                newName: "teachers");

            migrationBuilder.RenameTable(
                name: "Sections",
                newName: "sections");

            migrationBuilder.RenameIndex(
                name: "IX_Teachers_UserId",
                table: "teachers",
                newName: "IX_teachers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Teachers_TenantId",
                table: "teachers",
                newName: "IX_teachers_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Teachers_SchoolId",
                table: "teachers",
                newName: "IX_teachers_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_TenantId",
                table: "sections",
                newName: "IX_sections_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_SchoolId",
                table: "sections",
                newName: "IX_sections_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_HomeTeacherId",
                table: "sections",
                newName: "IX_sections_HomeTeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_Sections_AcademicYearId",
                table: "sections",
                newName: "IX_sections_AcademicYearId");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherCode",
                table: "teachers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Specialization",
                table: "teachers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Qualifications",
                table: "teachers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "teachers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "teachers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "teachers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "teachers",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "teachers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContractType",
                table: "teachers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Shift",
                table: "sections",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "sections",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Classroom",
                table: "sections",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_teachers",
                table: "teachers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sections",
                table: "sections",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_teachers_TenantId_Email",
                table: "teachers",
                columns: new[] { "TenantId", "Email" },
                unique: true,
                filter: "email IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_teachers_TenantId_NationalId",
                table: "teachers",
                columns: new[] { "TenantId", "NationalId" },
                unique: true,
                filter: "national_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_sections_GradeLevelId_AcademicYearId_Name_Shift",
                table: "sections",
                columns: new[] { "GradeLevelId", "AcademicYearId", "Name", "Shift" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_records_sections_SectionId",
                table: "attendance_records",
                column: "SectionId",
                principalTable: "sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_records_teachers_TeacherId",
                table: "attendance_records",
                column: "TeacherId",
                principalTable: "teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_enrollments_sections_SectionId",
                table: "enrollments",
                column: "SectionId",
                principalTable: "sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_entries_teachers_TeacherId",
                table: "grade_entries",
                column: "TeacherId",
                principalTable: "teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sections_AcademicYears_AcademicYearId",
                table: "sections",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sections_GradeLevels_GradeLevelId",
                table: "sections",
                column: "GradeLevelId",
                principalTable: "GradeLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sections_schools_SchoolId",
                table: "sections",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sections_teachers_HomeTeacherId",
                table: "sections",
                column: "HomeTeacherId",
                principalTable: "teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_sections_tenants_TenantId",
                table: "sections",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_sections_SectionId",
                table: "TeacherAssignments",
                column: "SectionId",
                principalTable: "sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_teachers_TeacherId",
                table: "TeacherAssignments",
                column: "TeacherId",
                principalTable: "teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teachers_schools_SchoolId",
                table: "teachers",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_teachers_tenants_TenantId",
                table: "teachers",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_teachers_users_UserId",
                table: "teachers",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attendance_records_sections_SectionId",
                table: "attendance_records");

            migrationBuilder.DropForeignKey(
                name: "FK_attendance_records_teachers_TeacherId",
                table: "attendance_records");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollments_sections_SectionId",
                table: "enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_entries_teachers_TeacherId",
                table: "grade_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_sections_AcademicYears_AcademicYearId",
                table: "sections");

            migrationBuilder.DropForeignKey(
                name: "FK_sections_GradeLevels_GradeLevelId",
                table: "sections");

            migrationBuilder.DropForeignKey(
                name: "FK_sections_schools_SchoolId",
                table: "sections");

            migrationBuilder.DropForeignKey(
                name: "FK_sections_teachers_HomeTeacherId",
                table: "sections");

            migrationBuilder.DropForeignKey(
                name: "FK_sections_tenants_TenantId",
                table: "sections");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_sections_SectionId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_teachers_TeacherId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_teachers_schools_SchoolId",
                table: "teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_teachers_tenants_TenantId",
                table: "teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_teachers_users_UserId",
                table: "teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_teachers",
                table: "teachers");

            migrationBuilder.DropIndex(
                name: "IX_teachers_TenantId_Email",
                table: "teachers");

            migrationBuilder.DropIndex(
                name: "IX_teachers_TenantId_NationalId",
                table: "teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sections",
                table: "sections");

            migrationBuilder.DropIndex(
                name: "IX_sections_GradeLevelId_AcademicYearId_Name_Shift",
                table: "sections");

            migrationBuilder.RenameTable(
                name: "teachers",
                newName: "Teachers");

            migrationBuilder.RenameTable(
                name: "sections",
                newName: "Sections");

            migrationBuilder.RenameIndex(
                name: "IX_teachers_UserId",
                table: "Teachers",
                newName: "IX_Teachers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_teachers_TenantId",
                table: "Teachers",
                newName: "IX_Teachers_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_teachers_SchoolId",
                table: "Teachers",
                newName: "IX_Teachers_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_sections_TenantId",
                table: "Sections",
                newName: "IX_Sections_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_sections_SchoolId",
                table: "Sections",
                newName: "IX_Sections_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_sections_HomeTeacherId",
                table: "Sections",
                newName: "IX_Sections_HomeTeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_sections_AcademicYearId",
                table: "Sections",
                newName: "IX_Sections_AcademicYearId");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherCode",
                table: "Teachers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Specialization",
                table: "Teachers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Qualifications",
                table: "Teachers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "Teachers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Teachers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "Teachers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Teachers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teachers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContractType",
                table: "Teachers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "Shift",
                table: "Sections",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Classroom",
                table: "Sections",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sections",
                table: "Sections",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_GradeLevelId",
                table: "Sections",
                column: "GradeLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_records_Sections_SectionId",
                table: "attendance_records",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_records_Teachers_TeacherId",
                table: "attendance_records",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_enrollments_Sections_SectionId",
                table: "enrollments",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_entries_Teachers_TeacherId",
                table: "grade_entries",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_AcademicYears_AcademicYearId",
                table: "Sections",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_GradeLevels_GradeLevelId",
                table: "Sections",
                column: "GradeLevelId",
                principalTable: "GradeLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Teachers_HomeTeacherId",
                table: "Sections",
                column: "HomeTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_schools_SchoolId",
                table: "Sections",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_tenants_TenantId",
                table: "Sections",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_Sections_SectionId",
                table: "TeacherAssignments",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_Teachers_TeacherId",
                table: "TeacherAssignments",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_schools_SchoolId",
                table: "Teachers",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_tenants_TenantId",
                table: "Teachers",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_users_UserId",
                table: "Teachers",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
