using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceTenantStoreInstanceConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stores");

            migrationBuilder.Sql("ALTER TABLE tenants ALTER COLUMN \"OwnerUserId\" TYPE uuid using (\"OwnerUserId\"::uuid);");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerUserId",
                table: "tenants",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "store_brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_instances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreBrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_instances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_instances_store_brands_StoreBrandId",
                        column: x => x.StoreBrandId,
                        principalTable: "store_brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_TenantId_Role",
                table: "tenant_users",
                columns: new[] { "TenantId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_store_brands_Name",
                table: "store_brands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_brands_NormalizedName",
                table: "store_brands",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_instances_StoreBrandId",
                table: "store_instances",
                column: "StoreBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_store_instances_TenantId",
                table: "store_instances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_store_instances_TenantId_DisplayName",
                table: "store_instances",
                columns: new[] { "TenantId", "DisplayName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "store_instances");

            migrationBuilder.DropTable(
                name: "store_brands");

            migrationBuilder.DropIndex(
                name: "IX_tenant_users_TenantId_Role",
                table: "tenant_users");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerUserId",
                table: "tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "stores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stores_TenantId",
                table: "stores",
                column: "TenantId");
        }
    }
}
