CREATE TABLE subscription_management.subscriptions
(
    id uuid NOT NULL DEFAULT uuid_generate_v4(),
    user_id uuid NOT NULL,
    start_date_time timestamp with time zone NOT NULL,
    end_date_time timestamp with time zone NOT NULL,
    validity_period bigint NOT NULL,
    auto_extend boolean NOT NULL,
    tariff_plan_id uuid NOT NULL,
    status text CHECK (status in ( 'Active', 'Inactive' ) ) NOT NULL,

    CONSTRAINT pk_subscriptions PRIMARY KEY (id),

    CONSTRAINT fk_subscriptions_tariff_plan_tariff_plan_id FOREIGN KEY (tariff_plan_id)
        REFERENCES subscription_management.tariff_plans (id) MATCH SIMPLE
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

COMMENT ON TABLE subscription_management.subscriptions IS 'Подписка';
