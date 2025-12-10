\set ON_ERROR_STOP 1
\set ECHO all
\encoding UTF8

-- CREATING SCHEMA
\ir 'schemas/subscription_management.sql'

-- CREATING EXTENSIONS
\ir 'extensions/guid_generator_extension.sql'

-- CREATING TABLES
\ir 'tables/quartz_scheduler.sql'
\ir 'tables/tariff_plans.sql'
\ir 'tables/subscriptions.sql'
\ir 'tables/saga_operation_statuses.sql'
\ir 'tables/version_info.sql'

-- DATA INSERTS
\ir 'data_tables/version_info.sql'