using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TeachersEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── New columns ────────────────────────────────────────────────────
            migrationBuilder.AddColumn<string>(
                name: "MinerdCode",
                table: "teachers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "teachers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcademicTitle",
                table: "teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingHoursPerWeek",
                table: "teachers",
                type: "integer",
                nullable: false,
                defaultValue: 40);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractEndDate",
                table: "teachers",
                type: "timestamp with time zone",
                nullable: true);

            // ── Adjust existing column lengths ─────────────────────────────────
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
                name: "LastName",
                table: "teachers",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "teachers",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "teachers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "TeacherCode",
                table: "teachers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Specialization",
                table: "teachers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            // ── Unique index: (TenantId, MinerdCode) WHERE minerd_code IS NOT NULL
            migrationBuilder.CreateIndex(
                name: "IX_teachers_TenantId_MinerdCode",
                table: "teachers",
                columns: new[] { "TenantId", "MinerdCode" },
                unique: true,
                filter: "minerd_code IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_teachers_TenantId_MinerdCode",
                table: "teachers");

            migrationBuilder.DropColumn(name: "MinerdCode",          table: "teachers");
            migrationBuilder.DropColumn(name: "Address",             table: "teachers");
            migrationBuilder.DropColumn(name: "AcademicTitle",       table: "teachers");
            migrationBuilder.DropColumn(name: "WorkingHoursPerWeek", table: "teachers");
            migrationBuilder.DropColumn(name: "ContractEndDate",     table: "teachers");
        }
    }
}
