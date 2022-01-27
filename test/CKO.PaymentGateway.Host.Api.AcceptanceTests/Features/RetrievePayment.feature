Feature: RetrievePayment

The retrieve payments feature.

@retrieve_payments
Scenario: Retrieving an existing payment for an allowed merchant should return the payment
	Given a merchant with name 'John Doe' and merchant id '8005d917-3c6b-4b48-adc8-0ebe0e6dbc94'
	And the authorization token 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJtZXJjaGFudElkIjoiODAwNWQ5MTctM2M2Yi00YjQ4LWFkYzgtMGViZTBlNmRiYzk0In0.0V4hnefnc9sDWcgcdlhqawRfsz-fn4I5SqE3jcvuJyo'
	When the retrieve payment is requested for id 'c2fdcf74-f4d9-4a21-a7a4-c39ec65857d1'
	Then the result should be a payment with id 'c2fdcf74-f4d9-4a21-a7a4-c39ec65857d1'.

@retrieve_payments
Scenario: Retrieving an existing payment from another merchant should return a 404 Not Found
	Given a merchant with name 'Adidas' and merchant id 'ed962527-eb35-44a9-9c09-b75af6c84ac2'
	And the authorization token 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiQWRpZGFzIiwibWVyY2hhbnRJZCI6ImVkOTYyNTI3LWViMzUtNDRhOS05YzA5LWI3NWFmNmM4NGFjMiJ9.dSGWjei4fg6jbhQ-Hysuok6F7967thIifSetVoy1NlY'
	When the retrieve payment is requested for id 'c2fdcf74-f4d9-4a21-a7a4-c39ec65857d1'
	Then the result should be payment not found.

@retrieve_payments
Scenario: Retrieving a payment with an invalid token should return a 401 Unauthorized
	Given a merchant with name 'John Doe' and merchant id '8005d917-3c6b-4b48-adc8-0ebe0e6dbc94'
	And the authorization token 'not so good token'
	When the retrieve payment is requested for id 'c2fdcf74-f4d9-4a21-a7a4-c39ec65857d1'
	Then the result an unauthorized response.
