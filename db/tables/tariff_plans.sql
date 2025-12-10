CREATE TABLE subscription_management.tariff_plans
(
    id uuid NOT NULL DEFAULT uuid_generate_v4(),
    name text NOT NULL,
    description text,
    price decimal NOT NULL,
    payment_strategy text CHECK ( payment_strategy in ( 'Prepayment', 'Postpayment' ) ) NOT NULL,

    CONSTRAINT pk_tariff_plans PRIMARY KEY (id),

    CONSTRAINT unique_name UNIQUE (name)
);

CREATE INDEX ix_tariff_plans_name
    ON subscription_management.tariff_plans USING btree
    (name ASC NULLS LAST);

COMMENT ON TABLE subscription_management.tariff_plans IS 'Тарифный план';
