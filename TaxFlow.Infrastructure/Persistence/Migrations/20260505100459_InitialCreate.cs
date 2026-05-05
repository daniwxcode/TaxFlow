using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "asset_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    liquidation_mode = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_asset_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "enum_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    label = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enum_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "legal_references",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    text_type = table.Column<int>(type: "integer", nullable: false),
                    reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    article = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    publication_date = table.Column<DateOnly>(type: "date", nullable: true),
                    effective_date = table.Column<DateOnly>(type: "date", nullable: true),
                    url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_legal_references", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tax_rules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    label = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    expression = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    asset_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tax_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_tax_rules_asset_types_asset_type_id",
                        column: x => x.asset_type_id,
                        principalTable: "asset_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attribute_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    label = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    data_type = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    enum_definition_id = table.Column<Guid>(type: "uuid", nullable: true),
                    regex_pattern = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    asset_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attribute_definitions", x => x.id);
                    table.ForeignKey(
                        name: "FK_attribute_definitions_asset_types_asset_type_id",
                        column: x => x.asset_type_id,
                        principalTable: "asset_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attribute_definitions_enum_definitions_enum_definition_id",
                        column: x => x.enum_definition_id,
                        principalTable: "enum_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "enum_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    label = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    enum_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enum_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_enum_items_enum_definitions_enum_definition_id",
                        column: x => x.enum_definition_id,
                        principalTable: "enum_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tax_obligation_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    fiscal_year = table.Column<int>(type: "integer", nullable: true),
                    tax_rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tax_obligation_schedules", x => x.id);
                    table.ForeignKey(
                        name: "FK_tax_obligation_schedules_tax_rules_tax_rule_id",
                        column: x => x.tax_rule_id,
                        principalTable: "tax_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "declaration_deadlines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    penalty_type = table.Column<int>(type: "integer", nullable: true),
                    penalty_trigger_event = table.Column<int>(type: "integer", nullable: true),
                    penalty_fixed_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_grace_period = table.Column<string>(type: "jsonb", nullable: true),
                    penalty_period = table.Column<string>(type: "jsonb", nullable: true),
                    penalty_annual_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_period_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_period_rate_increment = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_cap = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_minimum = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_capitalize = table.Column<bool>(type: "boolean", nullable: true),
                    declaration_type = table.Column<int>(type: "integer", nullable: false),
                    requires_documents = table.Column<bool>(type: "boolean", nullable: false),
                    form_reference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    label = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    localized_label = table.Column<string>(type: "jsonb", nullable: true),
                    due_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    grace_period = table.Column<string>(type: "jsonb", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    localized_description = table.Column<string>(type: "jsonb", nullable: true),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    periodicity = table.Column<int>(type: "integer", nullable: false),
                    regime = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    condition_expression = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    fiscal_year = table.Column<int>(type: "integer", nullable: true),
                    period = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_declaration_deadlines", x => x.id);
                    table.ForeignKey(
                        name: "FK_declaration_deadlines_tax_obligation_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "tax_obligation_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_deadlines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fraction = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    payment_type = table.Column<int>(type: "integer", nullable: false),
                    penalty_type = table.Column<int>(type: "integer", nullable: true),
                    penalty_trigger_event = table.Column<int>(type: "integer", nullable: true),
                    penalty_fixed_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_grace_period = table.Column<string>(type: "jsonb", nullable: true),
                    penalty_period = table.Column<string>(type: "jsonb", nullable: true),
                    penalty_annual_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_period_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_period_rate_increment = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_cap = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_minimum = table.Column<decimal>(type: "numeric", nullable: true),
                    penalty_capitalize = table.Column<bool>(type: "boolean", nullable: true),
                    linked_declaration_key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    allows_partial_payment = table.Column<bool>(type: "boolean", nullable: false),
                    minimum_payment = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    fixed_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    label = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    localized_label = table.Column<string>(type: "jsonb", nullable: true),
                    due_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    grace_period = table.Column<string>(type: "jsonb", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    localized_description = table.Column<string>(type: "jsonb", nullable: true),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    periodicity = table.Column<int>(type: "integer", nullable: false),
                    regime = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    condition_expression = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    fiscal_year = table.Column<int>(type: "integer", nullable: true),
                    period = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_deadlines", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_deadlines_tax_obligation_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "tax_obligation_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tax_obligation_schedule_legal_references",
                columns: table => new
                {
                    LegalReferencesId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaxObligationScheduleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tax_obligation_schedule_legal_references", x => new { x.LegalReferencesId, x.TaxObligationScheduleId });
                    table.ForeignKey(
                        name: "FK_tax_obligation_schedule_legal_references_legal_references_L~",
                        column: x => x.LegalReferencesId,
                        principalTable: "legal_references",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tax_obligation_schedule_legal_references_tax_obligation_sch~",
                        column: x => x.TaxObligationScheduleId,
                        principalTable: "tax_obligation_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "declaration_deadline_legal_references",
                columns: table => new
                {
                    DeclarationDeadlineId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalReferencesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_declaration_deadline_legal_references", x => new { x.DeclarationDeadlineId, x.LegalReferencesId });
                    table.ForeignKey(
                        name: "FK_declaration_deadline_legal_references_declaration_deadlines~",
                        column: x => x.DeclarationDeadlineId,
                        principalTable: "declaration_deadlines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_declaration_deadline_legal_references_legal_references_Lega~",
                        column: x => x.LegalReferencesId,
                        principalTable: "legal_references",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_deadline_legal_references",
                columns: table => new
                {
                    LegalReferencesId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentDeadlineId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_deadline_legal_references", x => new { x.LegalReferencesId, x.PaymentDeadlineId });
                    table.ForeignKey(
                        name: "FK_payment_deadline_legal_references_legal_references_LegalRef~",
                        column: x => x.LegalReferencesId,
                        principalTable: "legal_references",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payment_deadline_legal_references_payment_deadlines_Payment~",
                        column: x => x.PaymentDeadlineId,
                        principalTable: "payment_deadlines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asset_types_name",
                table: "asset_types",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_attribute_definitions_enum_definition_id",
                table: "attribute_definitions",
                column: "enum_definition_id");

            migrationBuilder.CreateIndex(
                name: "ux_attribute_definitions_asset_key",
                table: "attribute_definitions",
                columns: new[] { "asset_type_id", "key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_declaration_deadline_legal_references_LegalReferencesId",
                table: "declaration_deadline_legal_references",
                column: "LegalReferencesId");

            migrationBuilder.CreateIndex(
                name: "IX_declaration_deadlines_schedule_id",
                table: "declaration_deadlines",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "ux_enum_definitions_key",
                table: "enum_definitions",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_enum_items_definition_code",
                table: "enum_items",
                columns: new[] { "enum_definition_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_deadline_legal_references_PaymentDeadlineId",
                table: "payment_deadline_legal_references",
                column: "PaymentDeadlineId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_deadlines_schedule_id",
                table: "payment_deadlines",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_tax_obligation_schedule_legal_references_TaxObligationSched~",
                table: "tax_obligation_schedule_legal_references",
                column: "TaxObligationScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_tax_obligation_schedules_tax_rule_id",
                table: "tax_obligation_schedules",
                column: "tax_rule_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_tax_rules_asset_key",
                table: "tax_rules",
                columns: new[] { "asset_type_id", "key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attribute_definitions");

            migrationBuilder.DropTable(
                name: "declaration_deadline_legal_references");

            migrationBuilder.DropTable(
                name: "enum_items");

            migrationBuilder.DropTable(
                name: "payment_deadline_legal_references");

            migrationBuilder.DropTable(
                name: "tax_obligation_schedule_legal_references");

            migrationBuilder.DropTable(
                name: "declaration_deadlines");

            migrationBuilder.DropTable(
                name: "enum_definitions");

            migrationBuilder.DropTable(
                name: "payment_deadlines");

            migrationBuilder.DropTable(
                name: "legal_references");

            migrationBuilder.DropTable(
                name: "tax_obligation_schedules");

            migrationBuilder.DropTable(
                name: "tax_rules");

            migrationBuilder.DropTable(
                name: "asset_types");
        }
    }
}
