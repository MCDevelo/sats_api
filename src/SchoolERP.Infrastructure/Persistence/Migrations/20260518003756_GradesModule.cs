using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GradesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicPeriods_AcademicYears_AcademicYearId",
                table: "AcademicPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_AcademicYears_schools_SchoolId",
                table: "AcademicYears");

            migrationBuilder.DropForeignKey(
                name: "FK_AcademicYears_tenants_TenantId",
                table: "AcademicYears");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollments_AcademicYears_AcademicYearId",
                table: "enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationPlans_AcademicPeriods_AcademicPeriodId",
                table: "EvaluationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationPlans_Subjects_SubjectId",
                table: "EvaluationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationPlans_tenants_TenantId",
                table: "EvaluationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_entries_EvaluationPlans_EvaluationPlanId",
                table: "grade_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_entries_Subjects_SubjectId",
                table: "grade_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_invoices_AcademicYears_AcademicYearId",
                table: "invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_sections_AcademicYears_AcademicYearId",
                table: "sections");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_GradeLevels_GradeLevelId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_schools_SchoolId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_tenants_TenantId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_AcademicYears_AcademicYearId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_Subjects_SubjectId",
                table: "TeacherAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subjects",
                table: "Subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluationPlans",
                table: "EvaluationPlans");

            migrationBuilder.DropIndex(
                name: "IX_EvaluationPlans_SubjectId",
                table: "EvaluationPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicYears",
                table: "AcademicYears");

            migrationBuilder.RenameTable(
                name: "Subjects",
                newName: "subjects");

            migrationBuilder.RenameTable(
                name: "EvaluationPlans",
                newName: "evaluation_plans");

            migrationBuilder.RenameTable(
                name: "AcademicYears",
                newName: "academic_years");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_TenantId",
                table: "subjects",
                newName: "IX_subjects_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_SchoolId",
                table: "subjects",
                newName: "IX_subjects_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_GradeLevelId",
                table: "subjects",
                newName: "IX_subjects_GradeLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationPlans_TenantId",
                table: "evaluation_plans",
                newName: "IX_evaluation_plans_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationPlans_AcademicPeriodId",
                table: "evaluation_plans",
                newName: "IX_evaluation_plans_AcademicPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_AcademicYears_TenantId",
                table: "academic_years",
                newName: "IX_academic_years_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_AcademicYears_SchoolId",
                table: "academic_years",
                newName: "IX_academic_years_SchoolId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "subjects",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "subjects",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "subjects",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "evaluation_plans",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "evaluation_plans",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "evaluation_plans",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "academic_years",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subjects",
                table: "subjects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_evaluation_plans",
                table: "evaluation_plans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_academic_years",
                table: "academic_years",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_subjects_TenantId_GradeLevelId_Code",
                table: "subjects",
                columns: new[] { "TenantId", "GradeLevelId", "Code" },
                unique: true,
                filter: "code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_plans_SubjectId_AcademicPeriodId",
                table: "evaluation_plans",
                columns: new[] { "SubjectId", "AcademicPeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_academic_years_TenantId_SchoolId_Name",
                table: "academic_years",
                columns: new[] { "TenantId", "SchoolId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_academic_years_schools_SchoolId",
                table: "academic_years",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_academic_years_tenants_TenantId",
                table: "academic_years",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicPeriods_academic_years_AcademicYearId",
                table: "AcademicPeriods",
                column: "AcademicYearId",
                principalTable: "academic_years",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_enrollments_academic_years_AcademicYearId",
                table: "enrollments",
                column: "AcademicYearId",
                principalTable: "academic_years",
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
                name: "FK_evaluation_plans_subjects_SubjectId",
                table: "evaluation_plans",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_evaluation_plans_tenants_TenantId",
                table: "evaluation_plans",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_entries_evaluation_plans_EvaluationPlanId",
                table: "grade_entries",
                column: "EvaluationPlanId",
                principalTable: "evaluation_plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_entries_subjects_SubjectId",
                table: "grade_entries",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_academic_years_AcademicYearId",
                table: "invoices",
                column: "AcademicYearId",
                principalTable: "academic_years",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sections_academic_years_AcademicYearId",
                table: "sections",
                column: "AcademicYearId",
                principalTable: "academic_years",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subjects_GradeLevels_GradeLevelId",
                table: "subjects",
                column: "GradeLevelId",
                principalTable: "GradeLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subjects_schools_SchoolId",
                table: "subjects",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_subjects_tenants_TenantId",
                table: "subjects",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_academic_years_AcademicYearId",
                table: "TeacherAssignments",
                column: "AcademicYearId",
                principalTable: "academic_years",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_subjects_SubjectId",
                table: "TeacherAssignments",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_academic_years_schools_SchoolId",
                table: "academic_years");

            migrationBuilder.DropForeignKey(
                name: "FK_academic_years_tenants_TenantId",
                table: "academic_years");

            migrationBuilder.DropForeignKey(
                name: "FK_AcademicPeriods_academic_years_AcademicYearId",
                table: "AcademicPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollments_academic_years_AcademicYearId",
                table: "enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_evaluation_plans_AcademicPeriods_AcademicPeriodId",
                table: "evaluation_plans");

            migrationBuilder.DropForeignKey(
                name: "FK_evaluation_plans_subjects_SubjectId",
                table: "evaluation_plans");

            migrationBuilder.DropForeignKey(
                name: "FK_evaluation_plans_tenants_TenantId",
                table: "evaluation_plans");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_entries_evaluation_plans_EvaluationPlanId",
                table: "grade_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_grade_entries_subjects_SubjectId",
                table: "grade_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_invoices_academic_years_AcademicYearId",
                table: "invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_sections_academic_years_AcademicYearId",
                table: "sections");

            migrationBuilder.DropForeignKey(
                name: "FK_subjects_GradeLevels_GradeLevelId",
                table: "subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_subjects_schools_SchoolId",
                table: "subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_subjects_tenants_TenantId",
                table: "subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_academic_years_AcademicYearId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_subjects_SubjectId",
                table: "TeacherAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subjects",
                table: "subjects");

            migrationBuilder.DropIndex(
                name: "IX_subjects_TenantId_GradeLevelId_Code",
                table: "subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_evaluation_plans",
                table: "evaluation_plans");

            migrationBuilder.DropIndex(
                name: "IX_evaluation_plans_SubjectId_AcademicPeriodId",
                table: "evaluation_plans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_academic_years",
                table: "academic_years");

            migrationBuilder.DropIndex(
                name: "IX_academic_years_TenantId_SchoolId_Name",
                table: "academic_years");

            migrationBuilder.RenameTable(
                name: "subjects",
                newName: "Subjects");

            migrationBuilder.RenameTable(
                name: "evaluation_plans",
                newName: "EvaluationPlans");

            migrationBuilder.RenameTable(
                name: "academic_years",
                newName: "AcademicYears");

            migrationBuilder.RenameIndex(
                name: "IX_subjects_TenantId",
                table: "Subjects",
                newName: "IX_Subjects_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_subjects_SchoolId",
                table: "Subjects",
                newName: "IX_Subjects_SchoolId");

            migrationBuilder.RenameIndex(
                name: "IX_subjects_GradeLevelId",
                table: "Subjects",
                newName: "IX_Subjects_GradeLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_evaluation_plans_TenantId",
                table: "EvaluationPlans",
                newName: "IX_EvaluationPlans_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_evaluation_plans_AcademicPeriodId",
                table: "EvaluationPlans",
                newName: "IX_EvaluationPlans_AcademicPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_academic_years_TenantId",
                table: "AcademicYears",
                newName: "IX_AcademicYears_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_academic_years_SchoolId",
                table: "AcademicYears",
                newName: "IX_AcademicYears_SchoolId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subjects",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Subjects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Subjects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "EvaluationPlans",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EvaluationPlans",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EvaluationPlans",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AcademicYears",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subjects",
                table: "Subjects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluationPlans",
                table: "EvaluationPlans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicYears",
                table: "AcademicYears",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationPlans_SubjectId",
                table: "EvaluationPlans",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicPeriods_AcademicYears_AcademicYearId",
                table: "AcademicPeriods",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicYears_schools_SchoolId",
                table: "AcademicYears",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicYears_tenants_TenantId",
                table: "AcademicYears",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_enrollments_AcademicYears_AcademicYearId",
                table: "enrollments",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationPlans_AcademicPeriods_AcademicPeriodId",
                table: "EvaluationPlans",
                column: "AcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationPlans_Subjects_SubjectId",
                table: "EvaluationPlans",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationPlans_tenants_TenantId",
                table: "EvaluationPlans",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_entries_EvaluationPlans_EvaluationPlanId",
                table: "grade_entries",
                column: "EvaluationPlanId",
                principalTable: "EvaluationPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grade_entries_Subjects_SubjectId",
                table: "grade_entries",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_AcademicYears_AcademicYearId",
                table: "invoices",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sections_AcademicYears_AcademicYearId",
                table: "sections",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_GradeLevels_GradeLevelId",
                table: "Subjects",
                column: "GradeLevelId",
                principalTable: "GradeLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_schools_SchoolId",
                table: "Subjects",
                column: "SchoolId",
                principalTable: "schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_tenants_TenantId",
                table: "Subjects",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_AcademicYears_AcademicYearId",
                table: "TeacherAssignments",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_Subjects_SubjectId",
                table: "TeacherAssignments",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
