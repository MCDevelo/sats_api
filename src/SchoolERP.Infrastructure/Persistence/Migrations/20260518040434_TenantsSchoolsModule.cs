using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TenantsSchoolsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicPeriods_academic_years_AcademicYearId",
                table: "AcademicPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_attendance_records_AcademicPeriods_AcademicPeriodId",
                table: "attendance_records");

            migrationBuilder.DropForeignKey(
                name: "FK_evaluation_plans_AcademicPeriods_AcademicPeriodId",
                table: "evaluation_plans");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_entries_AcademicPeriods_AcademicPeriodId",
                table: "grade_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_GradeLevels_schools_SchoolId",
                table: "GradeLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_GradeLevels_tenants_TenantId",
                table: "GradeLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_sections_GradeLevels_GradeLevelId",
                table: "sections");

            migrationBuilder.DropForeignKey(
                name: "FK_subjects_GradeLevels_GradeLevelId",
                table: "subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GradeLevels",
                table: "GradeLevels");

            migrationBuilder.DropIndex(
                name: "IX_GradeLevels_SchoolId",
                table: "GradeLevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicPeriods",
                table: "AcademicPeriods");

            migrationBuilder.RenameTable(
                name: "GradeLevels",
                newName: "grade_levels");

            migrationBuilder.RenameTable(
                name: "AcademicPeriods",
                newName: "academic_periods");

            migrationBuilder.RenameIndex(
                name: "IX_GradeLevels_TenantId",
                table: "grade_levels",
                newName: "IX_grade_levels_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_AcademicPeriods_AcademicYearId",
                table: "academic_periods",
                newName: "IX_academic_periods_AcademicYearId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "grade_levels",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EducationLevel",
                table: "grade_levels",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "academic_periods",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_grade_levels",
                table: "grade_levels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_academic_periods",
                table: "academic_periods",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_grade_levels_SchoolId_Name",
                table: "grade_levels",
                columns: new[] { "SchoolId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grade_levels_SchoolId_Order",
                table: "grade_levels",
                columns: new[] { "SchoolId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_academic_periods_AcademicYearId_PeriodNumber",
                table: "academic_periods",
                columns: new[] { "AcademicYearId", "PeriodNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_academic_periods_academic_years_AcademicYearId",
                table: "academic_periods",
                column: "AcademicYearId",
                principalTable: "academic_years",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_records_academic_periods_AcademicPeriodId",
                table: "attendance_records",
                column: "AcademicPeriodId",
                principalTable: "academic_periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_evaluation_plans_academic_periods_AcademicPeriodId",
                table: "evaluation_plans",
                column: "AcademicPeriodId",
                principalTable: "academic_periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_entries_academic_periods_AcademicPeriodId",
                table: "grade_entries",
                column: "AcademicPeriodId",
                principalTable: "academic_periods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_levels_schools_SchoolId",
                table: "grade_levels",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_levels_tenants_TenantId",
                table: "grade_levels",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sections_grade_levels_GradeLevelId",
                table: "sections",
                column: "GradeLevelId",
                principalTable: "grade_levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subjects_grade_levels_GradeLevelId",
                table: "subjects",
                column: "GradeLevelId",
                principalTable: "grade_levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_academic_periods_academic_years_AcademicYearId",
                table: "academic_periods");

            migrationBuilder.DropForeignKey(
                name: "FK_attendance_records_academic_periods_AcademicPeriodId",
                table: "attendance_records");

            migrationBuilder.DropForeignKey(
                name: "FK_evaluation_plans_academic_periods_AcademicPeriodId",
                table: "evaluation_plans");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_entries_academic_periods_AcademicPeriodId",
                table: "grade_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_levels_schools_SchoolId",
                table: "grade_levels");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_levels_tenants_TenantId",
                table: "grade_levels");

            migrationBuilder.DropForeignKey(
                name: "FK_sections_grade_levels_GradeLevelId",
                table: "sections");

            migrationBuilder.DropForeignKey(
                name: "FK_subjects_grade_levels_GradeLevelId",
                table: "subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_grade_levels",
                table: "grade_levels");

            migrationBuilder.DropIndex(
                name: "IX_grade_levels_SchoolId_Name",
                table: "grade_levels");

            migrationBuilder.DropIndex(
                name: "IX_grade_levels_SchoolId_Order",
                table: "grade_levels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_academic_periods",
                table: "academic_periods");

            migrationBuilder.DropIndex(
                name: "IX_academic_periods_AcademicYearId_PeriodNumber",
                table: "academic_periods");

            migrationBuilder.RenameTable(
                name: "grade_levels",
                newName: "GradeLevels");

            migrationBuilder.RenameTable(
                name: "academic_periods",
                newName: "AcademicPeriods");

            migrationBuilder.RenameIndex(
                name: "IX_grade_levels_TenantId",
                table: "GradeLevels",
                newName: "IX_GradeLevels_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_academic_periods_AcademicYearId",
                table: "AcademicPeriods",
                newName: "IX_AcademicPeriods_AcademicYearId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GradeLevels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "EducationLevel",
                table: "GradeLevels",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AcademicPeriods",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GradeLevels",
                table: "GradeLevels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicPeriods",
                table: "AcademicPeriods",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GradeLevels_SchoolId",
                table: "GradeLevels",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicPeriods_academic_years_AcademicYearId",
                table: "AcademicPeriods",
                column: "AcademicYearId",
                principalTable: "academic_years",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_records_AcademicPeriods_AcademicPeriodId",
                table: "attendance_records",
                column: "AcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_evaluation_plans_AcademicPeriods_AcademicPeriodId",
                table: "evaluation_plans",
                column: "AcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_entries_AcademicPeriods_AcademicPeriodId",
                table: "grade_entries",
                column: "AcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GradeLevels_schools_SchoolId",
                table: "GradeLevels",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GradeLevels_tenants_TenantId",
                table: "GradeLevels",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sections_GradeLevels_GradeLevelId",
                table: "sections",
                column: "GradeLevelId",
                principalTable: "GradeLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subjects_GradeLevels_GradeLevelId",
                table: "subjects",
                column: "GradeLevelId",
                principalTable: "GradeLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
