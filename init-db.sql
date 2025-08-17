-- Initialize CDR database
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_stat_statements";

-- Set timezone
SET timezone = 'UTC';

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_cdr_records_call_date_caller_id ON cdr_records(call_date, caller_id);
CREATE INDEX IF NOT EXISTS idx_cdr_records_recipient ON cdr_records(recipient);
CREATE INDEX IF NOT EXISTS idx_cdr_records_reference ON cdr_records(reference);

-- Grant necessary permissions
GRANT ALL PRIVILEGES ON DATABASE cdr TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO postgres;
