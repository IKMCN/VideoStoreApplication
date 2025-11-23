-- Migration: Add payment tracking columns to rentals table
-- Run this against your videostore database

-- Add new columns
ALTER TABLE rentals 
ADD COLUMN payment_transaction_id VARCHAR(255),
ADD COLUMN payment_status VARCHAR(50) NOT NULL DEFAULT 'Pending',
ADD COLUMN rental_amount DECIMAL(10,2) NOT NULL DEFAULT 0.00;

-- Create index on payment_transaction_id for faster lookups
CREATE INDEX idx_rentals_payment_transaction_id 
ON rentals(payment_transaction_id);

-- Add check constraint to ensure valid payment statuses
ALTER TABLE rentals
ADD CONSTRAINT chk_payment_status 
CHECK (payment_status IN ('Pending', 'Paid', 'Failed', 'Refunded'));

-- Verify the changes
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = 'rentals'
ORDER BY ordinal_position;
