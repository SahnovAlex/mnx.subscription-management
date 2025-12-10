CREATE TABLE subscription_management.saga_operation_statuses
(
    id uuid NOT NULL,
    reason text,
    status text CHECK (status in ( 'LicenseExtending', 'LicenseExtended', 'Expired', 'Success', 'Fail' ) ) NOT NULL,

    CONSTRAINT pk_saga_operation_statuses PRIMARY KEY (id)
);

COMMENT ON TABLE subscription_management.saga_operation_statuses IS 'Операции продления подписки';
