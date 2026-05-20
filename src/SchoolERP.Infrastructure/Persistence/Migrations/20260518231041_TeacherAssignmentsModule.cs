using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TeacherAssignmentsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedule_slots_TeacherAssignments_TeacherAssignmentId",
                table: "schedule_slots");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_academic_years_AcademicYearId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_sections_SectionId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_subjects_SubjectId",
                table: "TeacherAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAssignments_teachers_TeacherId",
                table: "TeacherAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherAssignments",
                table: "TeacherAssignments");

            migrationBuilder.RenameTable(
                name: "TeacherAssignments",
                newName: "teacher_assignments");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherAssignments_TeacherId",
                table: "teacher_assignments",
                newName: "IX_teacher_assignments_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherAssignments_SubjectId",
                table: "teacher_assignments",
                newName: "IX_teacher_assignments_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherAssignments_SectionId",
                table: "teacher_assignments",
                newName: "IX_teacher_assignments_SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherAssignments_AcademicYearId",
                table: "teacher_assignments",
                newName: "IX_teacher_assignments_AcademicYearId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_teacher_assignments",
                table: "teacher_assignments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_teacher_assignments_SectionId_SubjectId_AcademicYearId",
                table: "teacher_assignments",
                columns: new[] { "SectionId", "SubjectId", "AcademicYearId" },
                unique: true,
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "IX_teacher_assignments_TeacherId_SectionId_SubjectId_AcademicY~",
                table: "teacher_assignments",
                columns: new[] { "TeacherId", "SectionId", "SubjectId", "AcademicYearId" },
                unique: true,
                filter: "is_active = true");

            migrationBuilder.AddForeignKey(
                name: "FK_schedule_slots_teacher_assignments_TeacherAssignmentId",
                table: "schedule_slots",
                column: "TeacherAssignmentId",
                principalTable: "teacher_assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teacher_assignments_academic_years_AcademicYearId",
                table: "teacher_assignments",
                column: "AcademicYearId",
                principalTable: "academic_years",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_teacher_assignments_sections_SectionId",
                table: "teacher_assignments",
                column: "SectionId",
                principalTable: "sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_teacher_assignments_subjects_SubjectId",
                table: "teacher_assignments",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_teacher_assignments_teachers_TeacherId",
                table: "teacher_assignments",
                column: "TeacherId",
                principalTable: "teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedule_slots_teacher_assignments_TeacherAssignmentId",
                table: "schedule_slots");

            migrationBuilder.DropForeignKey(
                name: "FK_teacher_assignments_academic_years_AcademicYearId",
                table: "teacher_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_teacher_assignments_sections_SectionId",
                table: "teacher_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_teacher_assignments_subjects_SubjectId",
                table: "teacher_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_teacher_assignments_teachers_TeacherId",
                table: "teacher_assignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_teacher_assignments",
                table: "teacher_assignments");

            migrationBuilder.DropIndex(
                name: "IX_teacher_assignments_SectionId_SubjectId_AcademicYearId",
                table: "teacher_assignments");

            migrationBuilder.DropIndex(
                name: "IX_teacher_assignments_TeacherId_SectionId_SubjectId_AcademicY~",
                table: "teacher_assignments");

            migrationBuilder.RenameTable(
                name: "teacher_assignments",
                newName: "TeacherAssignments");

            migrationBuilder.RenameIndex(
                name: "IX_teacher_assignments_TeacherId",
                table: "TeacherAssignments",
                newName: "IX_TeacherAssignments_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_teacher_assignments_SubjectId",
                table: "TeacherAssignments",
                newName: "IX_TeacherAssignments_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_teacher_assignments_SectionId",
                table: "TeacherAssignments",
                newName: "IX_TeacherAssignments_SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_teacher_assignments_AcademicYearId",
                table: "TeacherAssignments",
                newName: "IX_TeacherAssignments_AcademicYearId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherAssignments",
                table: "TeacherAssignments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_schedule_slots_TeacherAssignments_TeacherAssignmentId",
                table: "schedule_slots",
                column: "TeacherAssignmentId",
                principalTable: "TeacherAssignments",
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
                name: "FK_TeacherAssignments_sections_SectionId",
                table: "TeacherAssignments",
                column: "SectionId",
                principalTable: "sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_subjects_SubjectId",
                table: "TeacherAssignments",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAssignments_teachers_TeacherId",
                table: "TeacherAssignments",
                column: "TeacherId",
                principalTable: "teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
