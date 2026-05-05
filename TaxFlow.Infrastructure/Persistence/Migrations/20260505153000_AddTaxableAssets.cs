using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TaxFlow.Infrastructure.Persistence;

#nullable disable

namespace TaxFlow.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(TaxFlowDbContext))]
    [Migration("20260505153000_AddTaxableAssets")]
    public partial class AddTaxableAssets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "taxable_assets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_deleted_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_recovered = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_recovered_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taxable_assets", x => x.id);
                    table.ForeignKey(
                        name: "FK_taxable_assets_asset_types_asset_type_id",
                        column: x => x.asset_type_id,
                        principalTable: "asset_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "extended_attributes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    value = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    data_type = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_deleted_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    last_recovered = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_recovered_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_extended_attributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_extended_attributes_taxable_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "taxable_assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_taxable_assets_asset_type_id",
                table: "taxable_assets",
                column: "asset_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_taxable_assets_external_id",
                table: "taxable_assets",
                column: "external_id");

            migrationBuilder.CreateIndex(
                name: "ix_extended_attributes_asset_id",
                table: "extended_attributes",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_extended_attributes_key",
                table: "extended_attributes",
                column: "key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "extended_attributes");

            migrationBuilder.DropTable(
                name: "taxable_assets");
        }
    }
}
