using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamePlanValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE tenants SET \"Plan\" = 'starter'      WHERE \"Plan\" = 'basic';");
            migrationBuilder.Sql("UPDATE tenants SET \"Plan\" = 'professional' WHERE \"Plan\" = 'standard';");
            migrationBuilder.Sql("UPDATE tenants SET \"Plan\" = 'enterprise'   WHERE \"Plan\" = 'premium';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE tenants SET \"Plan\" = 'basic'    WHERE \"Plan\" = 'starter';");
            migrationBuilder.Sql("UPDATE tenants SET \"Plan\" = 'standard' WHERE \"Plan\" = 'professional';");
            migrationBuilder.Sql("UPDATE tenants SET \"Plan\" = 'premium'  WHERE \"Plan\" = 'enterprise';");
        }
    }
}
