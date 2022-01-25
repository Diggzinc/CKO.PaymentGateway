 CREATE TABLE payments (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    merchant_id uuid NOT NULL,
    description text NOT NULL,
    partial_card_number text NOT NULL,
    card_number_length integer NOT NULL CHECK (card_number_length BETWEEN 14 AND 19),
    card_expiry_date_month integer NOT NULL CHECK (card_expiry_date_month BETWEEN 1 AND 12),
    card_expiry_date_year integer NOT NULL CHECK (card_expiry_date_year BETWEEN 0 AND 99),
    card_holder text NOT NULL,
    charge_amount decimal NOT NULL CHECK (charge_amount > 0),
    charge_currency text NOT NULL
);

 CREATE TABLE payment_operation_records (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    payment_id uuid,
    timestamp timestamptz NOT NULL,
    operation text NOT NULL,
    meta_data jsonb NOT NULL DEFAULT '{}'::jsonb,
	CONSTRAINT fk_payment
	      FOREIGN KEY(payment_id)
		  REFERENCES payments(id)
);

CREATE INDEX idx_merchant_id ON payments (merchant_id);
CREATE INDEX idx_id_merchant_id ON payments (id,merchant_id);
