using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StudentsEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── New columns on students ────────────────────────────────────────
            migrationBuilder.AddColumn<string>(
                name: "SecondLastName",
                table: "students",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nse",
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
                name: "Municipality",
                table: "students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "students",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

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

            // ── Widen existing columns to match updated config ─────────────────
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
                name: "LastName",
                table: "students",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

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
                oldDefaultValue: "DO");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "students",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "StudentCode",
                table: "students",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Allergies",
                table: "students",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "MedicalNotes",
                table: "students",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            // ── New unique index: (TenantId, Nse) WHERE nse IS NOT NULL ────────
            migrationBuilder.CreateIndex(
                name: "IX_students_TenantId_Nse",
                table: "students",
                columns: new[] { "TenantId", "Nse" },
                unique: true,
                filter: "nse IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_students_TenantId_Nse",
                table: "students");

            migrationBuilder.DropColumn(name: "SecondLastName",        table: "students");
            migrationBuilder.DropColumn(name: "Nse",                   table: "students");
            migrationBuilder.DropColumn(name: "Province",              table: "students");
            migrationBuilder.DropColumn(name: "Municipality",          table: "students");
            migrationBuilder.DropColumn(name: "Phone",                 table: "students");
            migrationBuilder.DropColumn(name: "HasSpecialNeeds",       table: "students");
            migrationBuilder.DropColumn(name: "HealthInsurance",       table: "students");
            migrationBuilder.DropColumn(name: "HealthInsuranceNumber", table: "students");
        }
    }
}
