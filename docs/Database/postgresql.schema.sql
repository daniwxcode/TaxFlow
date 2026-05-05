-- TaxFlow - Schéma initial PostgreSQL

CREATE TABLE IF NOT EXISTS asset_types (
    id uuid PRIMARY KEY,
    name varchar(200) NOT NULL,
    description varchar(1000),
    liquidation_mode int NOT NULL,

    created timestamptz NOT NULL,
    created_by uuid NOT NULL,
    last_modified timestamptz NOT NULL,
    last_modified_by uuid,

    deleted timestamptz,
    deleted_by uuid,
    last_deleted_on timestamptz,
    last_deleted_by uuid,
    last_recovered timestamptz,
    last_recovered_by uuid
);

CREATE INDEX IF NOT EXISTS ix_asset_types_name ON asset_types (name);

CREATE TABLE IF NOT EXISTS enum_definitions (
    id uuid PRIMARY KEY,
    key varchar(150) NOT NULL,
    label varchar(250) NOT NULL,

    created timestamptz NOT NULL,
    created_by uuid NOT NULL,
    last_modified timestamptz NOT NULL,
    last_modified_by uuid
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_enum_definitions_key ON enum_definitions (key);

CREATE TABLE IF NOT EXISTS enum_items (
    id uuid PRIMARY KEY,
    enum_definition_id uuid NOT NULL REFERENCES enum_definitions(id) ON DELETE CASCADE,
    code varchar(50) NOT NULL,
    label varchar(250) NOT NULL,
    "order" int NOT NULL,

    created timestamptz NOT NULL,
    created_by uuid NOT NULL,
    last_modified timestamptz NOT NULL,
    last_modified_by uuid
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_enum_items_definition_code ON enum_items (enum_definition_id, code);

CREATE TABLE IF NOT EXISTS attribute_definitions (
    id uuid PRIMARY KEY,
    asset_type_id uuid NOT NULL REFERENCES asset_types(id) ON DELETE CASCADE,
    enum_definition_id uuid REFERENCES enum_definitions(id) ON DELETE RESTRICT,
    key varchar(150) NOT NULL,
    label varchar(250) NOT NULL,
    data_type int NOT NULL,
    is_required boolean NOT NULL,
    regex_pattern varchar(2000),

    created timestamptz NOT NULL,
    created_by uuid NOT NULL,
    last_modified timestamptz NOT NULL,
    last_modified_by uuid
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_attribute_definitions_asset_key ON attribute_definitions (asset_type_id, key);
CREATE INDEX IF NOT EXISTS ix_attribute_definitions_enum_definition_id ON attribute_definitions (enum_definition_id);

CREATE TABLE IF NOT EXISTS tax_rules (
    id uuid PRIMARY KEY,
    asset_type_id uuid NOT NULL REFERENCES asset_types(id) ON DELETE CASCADE,
    key varchar(150) NOT NULL,
    label varchar(250) NOT NULL,
    expression varchar(4000) NOT NULL,
    description varchar(2000),
    enabled boolean NOT NULL,

    valid_from timestamptz NOT NULL,
    valid_to timestamptz,

    created timestamptz NOT NULL,
    created_by uuid NOT NULL,
    last_modified timestamptz NOT NULL,
    last_modified_by uuid
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_tax_rules_asset_key ON tax_rules (asset_type_id, key);

CREATE TABLE IF NOT EXISTS tax_obligation_schedules (
    id uuid PRIMARY KEY,
    tax_rule_id uuid NOT NULL REFERENCES tax_rules(id) ON DELETE CASCADE,
    name varchar(250),
    description varchar(2000),
    fiscal_year int,

    created timestamptz NOT NULL,
    created_by uuid NOT NULL,
    last_modified timestamptz NOT NULL,
    last_modified_by uuid
);

CREATE TABLE IF NOT EXISTS declaration_deadlines (
    id uuid PRIMARY KEY,
    schedule_id uuid NOT NULL REFERENCES tax_obligation_schedules(id) ON DELETE CASCADE,
    key varchar(150) NOT NULL,
    label varchar(250) NOT NULL,
    localized_label jsonb,
    due_date timestamptz NOT NULL,
    description varchar(2000),
    localized_description jsonb,
    enabled boolean NOT NULL,
    periodicity int NOT NULL,
    regime int NOT NULL,
    "order" int NOT NULL,
    condition_expression varchar(2000),
    fiscal_year int,
    period int,

    declaration_type int NOT NULL,
    requires_documents boolean NOT NULL,
    form_reference varchar(500),

    grace_period jsonb NOT NULL,

    penalty_type int,
    penalty_trigger_event int,
    penalty_fixed_amount numeric(18,2),
    penalty_annual_rate numeric(18,6),
    penalty_period_rate numeric(18,6),
    penalty_period_rate_increment numeric(18,6),
    penalty_cap numeric(18,2),
    penalty_minimum numeric(18,2),
    penalty_capitalize boolean,
    penalty_grace_period jsonb,
    penalty_period jsonb,

    created timestamptz NOT NULL,
    created_by uuid NOT NULL,
    last_modified timestamptz NOT NULL,
    last_modified_by uuid
);

CREATE TABLE IF NOT EXISTS payment_deadlines (
    id uuid PRIMARY KEY,
    schedule_id uuid NOT NULL REFERENCES tax_obligation_schedules(id) ON DELETE CASCADE,
    key varchar(150) NOT NULL,
    label varchar(250) NOT NULL,
    localized_label jsonb,
    due_date timestamptz NOT NULL,
    description varchar(2000),
    localized_description jsonb,
    enabled boolean NOT NULL,
    periodicity int NOT NULL,
    regime int NOT NULL,
    "order" int NOT NULL,
    condition_expression varchar(2000),
    fiscal_year int,
    period int,

    fraction numeric(9,6) NOT NULL,
    payment_type int NOT NULL,
    linked_declaration_key varchar(150),
    allows_partial_payment boolean NOT NULL,
    minimum_payment numeric(18,2),
    fixed_amount numeric(18,2),

    grace_period jsonb NOT NULL,

    penalty_type int,
    penalty_trigger_event int,
    penalty_fixed_amount numeric(18,2),
    penalty_annual_rate numeric(18,6),
    penalty_period_rate numeric(18,6),
    penalty_period_rate_increment numeric(18,6),
    penalty_cap numeric(18,2),
    penalty_minimum numeric(18,2),
    penalty_capitalize boolean,
    penalty_grace_period jsonb,
    penalty_period jsonb,

    created timestamptz NOT NULL,
    created_by uuid NOT NULL,
    last_modified timestamptz NOT NULL,
    last_modified_by uuid
);

CREATE TABLE IF NOT EXISTS legal_references (
    id uuid PRIMARY KEY,
    text_type int NOT NULL,
    reference varchar(200) NOT NULL,
    title varchar(500) NOT NULL,
    article varchar(200),
    publication_date date,
    effective_date date,
    url varchar(1000),
    notes varchar(2000),

    created timestamptz NOT NULL,
    created_by uuid NOT NULL,
    last_modified timestamptz NOT NULL,
    last_modified_by uuid
);

CREATE TABLE IF NOT EXISTS tax_obligation_schedule_legal_references (
    tax_obligation_schedules_id uuid NOT NULL REFERENCES tax_obligation_schedules(id) ON DELETE CASCADE,
    legal_references_id uuid NOT NULL REFERENCES legal_references(id) ON DELETE CASCADE,
    PRIMARY KEY (tax_obligation_schedules_id, legal_references_id)
);

CREATE TABLE IF NOT EXISTS declaration_deadline_legal_references (
    declaration_deadlines_id uuid NOT NULL REFERENCES declaration_deadlines(id) ON DELETE CASCADE,
    legal_references_id uuid NOT NULL REFERENCES legal_references(id) ON DELETE CASCADE,
    PRIMARY KEY (declaration_deadlines_id, legal_references_id)
);

CREATE TABLE IF NOT EXISTS payment_deadline_legal_references (
    payment_deadlines_id uuid NOT NULL REFERENCES payment_deadlines(id) ON DELETE CASCADE,
    legal_references_id uuid NOT NULL REFERENCES legal_references(id) ON DELETE CASCADE,
    PRIMARY KEY (payment_deadlines_id, legal_references_id)
);
