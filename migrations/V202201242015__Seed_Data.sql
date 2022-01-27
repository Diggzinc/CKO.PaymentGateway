INSERT INTO payments (
	id,
	merchant_id,
	description,
	partial_card_number,
	card_number_length,
	card_expiry_date_month,
	card_expiry_date_year,
	card_holder,
	charge_amount,
	charge_currency
)
VALUES(
	'606077e8-8f06-4f79-a6a1-12e48f3e33a1',
	'8005d917-3c6b-4b48-adc8-0ebe0e6dbc94',
	'AMZN PURCHASE',
	'1234',
	16,
	12,
	22,
	'John Doe',
	10.20,
	'EUR'),
	('967190ba-3877-4a4a-8163-11614f55080b',
	'8005d917-3c6b-4b48-adc8-0ebe0e6dbc94',
	'AMZN PURCHASE',
	'5678',
	16,
	12,
	22,
	'John Doe',
	100.99,
	'USD');


INSERT INTO payment_operation_records(
	id,
	payment_id,
	timestamp,
	operation,
	meta_data
)
VALUES(
	gen_random_uuid(),
	'606077e8-8f06-4f79-a6a1-12e48f3e33a1',
	'2022-01-24T20:55:21+00:00',
	'Issued',
	'{"transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"}'::jsonb),
	(gen_random_uuid(),
	'606077e8-8f06-4f79-a6a1-12e48f3e33a1',
	'2022-01-24T20:55:22+00:00',
	'Verifying',
	'{"transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"}'::jsonb),
	(gen_random_uuid(),
	'606077e8-8f06-4f79-a6a1-12e48f3e33a1',
	'2022-01-24T20:55:23+00:00',
	'Verified',
	'{"transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"}'::jsonb),
	(gen_random_uuid(),
	'606077e8-8f06-4f79-a6a1-12e48f3e33a1',
	'2022-01-24T20:55:24+00:00',
	'Authorizing',
	'{"transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"}'::jsonb),
	(gen_random_uuid(),
	'606077e8-8f06-4f79-a6a1-12e48f3e33a1',
	'2022-01-24T20:55:25+00:00',
	'Authorized',
	'{"transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"}'::jsonb),
	(gen_random_uuid(),
	'606077e8-8f06-4f79-a6a1-12e48f3e33a1',
	'2022-01-24T20:55:26+00:00',
	'Processing',
	'{"transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"}'::jsonb),
	(gen_random_uuid(),
	'606077e8-8f06-4f79-a6a1-12e48f3e33a1',
	'2022-01-24T20:55:27+00:00',
	'Processed',
	'{"transactionId": "020caaa5-fbf1-4923-b2f1-957440de9edd"}'::jsonb);

INSERT INTO payment_operation_records(
	id,
	payment_id,
	timestamp,
	operation,
	meta_data
)
VALUES(
	gen_random_uuid(),
	'967190ba-3877-4a4a-8163-11614f55080b',
	'2022-01-24T21:56:21+00:00',
	'Issued',
	'{"transactionId": "7e39a091-93da-4e74-acae-f53f38f412c3"}'::jsonb),
	(gen_random_uuid(),
	'967190ba-3877-4a4a-8163-11614f55080b',
	'2022-01-24T21:56:22+00:00',
	'Verifying',
	'{"transactionId": "7e39a091-93da-4e74-acae-f53f38f412c3"}'::jsonb),
	(gen_random_uuid(),
	'967190ba-3877-4a4a-8163-11614f55080b',
	'2022-01-24T21:56:23+00:00',
	'Verified',
	'{"transactionId": "7e39a091-93da-4e74-acae-f53f38f412c3"}'::jsonb),
	(gen_random_uuid(),
	'967190ba-3877-4a4a-8163-11614f55080b',
	'2022-01-24T21:56:24+00:00',
	'Authorizing',
	'{"transactionId": "7e39a091-93da-4e74-acae-f53f38f412c3"}'::jsonb),
	(gen_random_uuid(),
	'967190ba-3877-4a4a-8163-11614f55080b',
	'2022-01-24T21:56:26+00:00',
	'Failed',
	'{"transactionId": "7e39a091-93da-4e74-acae-f53f38f412c3", "reason": "INSUFFICIENT_FUNDS"}'::jsonb);
