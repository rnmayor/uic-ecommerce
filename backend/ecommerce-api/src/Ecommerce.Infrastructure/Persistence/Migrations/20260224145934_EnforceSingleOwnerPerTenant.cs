using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceSingleOwnerPerTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_store_brands_Name",
                table: "store_brands");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_TenantId",
                table: "tenant_users",
                column: "TenantId",
                unique: true,
                filter: "\"Role\" = 'Owner'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tenant_users_TenantId",
                table: "tenant_users");

            migrationBuilder.CreateIndex(
                name: "IX_store_brands_Name",
                table: "store_brands",
                column: "Name",
                unique: true);
        }
    }
}
