# VideoStore-Banking Payment Integration

## Implementation Guide

### Overview
This integration allows the VideoStore to confirm payments made through the Banking API (via ATM/card reader). The flow is:
1. Store clerk creates rental in VideoStore (status: Pending)
2. Customer pays at ATM/card reader (transaction recorded in Banking API)
3. Store clerk enters transaction ID in VideoStore
4. VideoStore verifies transaction with Banking API
5. Rental marked as Paid

---

## Setup Steps

### Step 1: Run Database Migration

Run the SQL script against your `videostore` PostgreSQL database:

```bash
psql -U postgres -d videostore -f add_payment_columns.sql
```

Or manually execute the SQL:
```sql
ALTER TABLE rentals 
ADD COLUMN payment_transaction_id VARCHAR(255),
ADD COLUMN payment_status VARCHAR(50) NOT NULL DEFAULT 'Pending',
ADD COLUMN rental_amount DECIMAL(10,2) NOT NULL DEFAULT 0.00;

CREATE INDEX idx_rentals_payment_transaction_id 
ON rentals(payment_transaction_id);

ALTER TABLE rentals
ADD CONSTRAINT chk_payment_status 
CHECK (payment_status IN ('Pending', 'Paid', 'Failed', 'Refunded'));
```

### Step 2: Update appsettings.json

Make sure your `appsettings.json` has the Banking API URL:

```json
{
  "BankingApi": {
    "BaseUrl": "http://localhost:5001"
  }
}
```

Adjust the port if your Banking API runs on a different port.

### Step 3: Build and Run

```bash
cd VideoStore.Api
dotnet build
dotnet run
```

The API should start on `http://localhost:5000` (or https://localhost:5001)

---

## Testing the Integration

### Prerequisites
1. VideoStore API running (port 5000)
2. Banking API running (port 5001) - you'll need this for the verification to work
3. PostgreSQL database with updated schema
4. Postman or similar API testing tool

### Test Scenario: Complete Rental with Payment

#### Step 1: Create a Customer (if needed)
```http
POST http://localhost:5000/api/customers
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "555-0123"
}
```

Response will include the customer ID (GUID).

#### Step 2: Create a Video (if needed)
```http
POST http://localhost:5000/api/videos
Content-Type: application/json

{
  "title": "The Matrix",
  "genre": "Sci-Fi",
  "releaseYear": 1999,
  "copiesAvailable": 5
}
```

Response will include the video ID (GUID).

#### Step 3: Create a Rental
```http
POST http://localhost:5000/api/rentals
Content-Type: application/json

{
  "customerId": "YOUR_CUSTOMER_ID",
  "videoId": "YOUR_VIDEO_ID",
  "rentalAmount": 5.00,
  "paymentStatus": "Pending"
}
```

Response:
```json
{
  "id": "RENTAL_ID_GUID",
  "customerId": "...",
  "videoId": "...",
  "rentedAt": "2024-11-22T...",
  "dueDate": "2024-11-29T...",
  "returnedAt": null,
  "lateFee": 0,
  "paymentTransactionId": null,
  "paymentStatus": "Pending",
  "rentalAmount": 5.00
}
```

Save the `id` (rental ID) - you'll need it for payment confirmation.

#### Step 4: Process Payment at ATM
This step happens in your Banking API via the ATM simulator:
1. Customer uses ATM/card reader
2. Payment processed through ISO 8583 server
3. Transaction recorded in Banking API
4. Transaction ID is displayed to customer/clerk

For testing, you can create a transaction directly in your Banking API:
```http
POST http://localhost:5001/api/transactions
Content-Type: application/json
Authorization: Bearer YOUR_JWT_TOKEN

{
  "accountId": "customer-account-id",
  "type": "Debit",
  "amount": 5.00,
  "description": "VideoStore Rental Payment"
}
```

Save the transaction ID from the response.

#### Step 5: Confirm Payment in VideoStore
```http
POST http://localhost:5000/api/rentals/{RENTAL_ID}/confirm-payment
Content-Type: application/json

{
  "transactionId": "TRANSACTION_ID_FROM_BANKING_API"
}
```

Success Response (200 OK):
```json
{
  "message": "Payment confirmed successfully",
  "rentalId": "...",
  "transactionId": "..."
}
```

#### Step 6: Verify Rental is Paid
```http
GET http://localhost:5000/api/rentals/{RENTAL_ID}
```

Response should show:
```json
{
  "id": "...",
  "paymentTransactionId": "TRANSACTION_ID",
  "paymentStatus": "Paid",
  "rentalAmount": 5.00
}
```

---

## Error Scenarios to Test

### 1. Transaction Not Found
```http
POST http://localhost:5000/api/rentals/{RENTAL_ID}/confirm-payment
{
  "transactionId": "invalid-transaction-id"
}
```
Expected: 400 Bad Request - "Transaction not found in Banking system"

### 2. Amount Mismatch
Create rental with amount 10.00, but transaction has amount 5.00
Expected: 400 Bad Request - "Transaction amount does not match rental amount"

### 3. Already Paid
Try to confirm payment twice on the same rental
Expected: 400 Bad Request - "Rental has already been paid"

### 4. Rental Not Found
```http
POST http://localhost:5000/api/rentals/00000000-0000-0000-0000-000000000000/confirm-payment
{
  "transactionId": "some-transaction-id"
}
```
Expected: 404 Not Found - "Rental not found"

---

## API Endpoints Added

### POST /api/rentals/{id}/confirm-payment
Confirms payment for a rental by verifying the transaction with the Banking API.

**Request Body:**
```json
{
  "transactionId": "string"
}
```

**Responses:**
- 200 OK: Payment confirmed successfully
- 400 Bad Request: Already paid, transaction not found, or amount mismatch
- 404 Not Found: Rental not found

---

## Integration Architecture

```
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│             │         │             │         │             │
│  VideoStore │ ◄─────► │  Banking    │ ◄─────► │  ATM/Card   │
│  Frontend   │         │  API        │         │  Reader     │
│             │         │             │         │             │
└─────────────┘         └─────────────┘         └─────────────┘
       │                       │                       │
       │ 1. Create Rental      │                       │
       │ (Status: Pending)     │                       │
       ├──────────────────────►│                       │
       │                       │ 2. Process Payment    │
       │                       │◄──────────────────────┤
       │                       │ 3. Return Trans ID    │
       │                       ├──────────────────────►│
       │ 4. Confirm Payment    │                       │
       │ (with Trans ID)       │                       │
       ├──────────────────────►│                       │
       │ 5. Verify Trans       │                       │
       │◄──────────────────────┤                       │
       │ 6. Update to Paid     │                       │
       │◄──────────────────────┤                       │
```

---

## Next Steps

1. **Run the database migration** ✅
2. **Test the flow manually** with Postman
3. **Update your frontend** to include payment confirmation UI
4. **Add authentication** between VideoStore and Banking API (optional but recommended)
5. **Add automated tests** for the payment flow
6. **Deploy** both APIs together

---

## Troubleshooting

### "Banking API returned 404"
- Check that your Banking API is running on the configured port
- Verify the transaction ID exists in the Banking API
- Check the `appsettings.json` BankingApi:BaseUrl is correct

### "Failed to confirm payment"
- Rental might already be marked as Paid
- Check database directly: `SELECT * FROM rentals WHERE id = 'rental_id';`

### "Transaction amount does not match"
- The Banking API transaction amount must match the rental amount
- Check both values in the error response

---

## Files Modified/Created

### Modified:
- `VideoStore.Application/Models/Rental.cs` - Added payment fields
- `VideoStore.Application/Repositories/IRentalRepository.cs` - Added ConfirmPayment method
- `VideoStore.Application/Repositories/RentalRepository.cs` - Implemented ConfirmPayment
- `VideoStore.Api/Controllers/RentalsController.cs` - Added confirm-payment endpoint
- `VideoStore.Api/Program.cs` - Registered HttpClient and BankingApiService
- `VideoStore.Api/appsettings.json` - Added Banking API configuration

### Created:
- `VideoStore.Application/Models/PaymentDtos.cs` - DTOs for Banking API
- `VideoStore.Application/Services/IBankingApiService.cs` - Service interface
- `VideoStore.Application/Services/BankingApiService.cs` - Service implementation
- `add_payment_columns.sql` - Database migration script

---

## Success Criteria

✅ Database schema updated with payment columns
✅ Can create rental with Pending status
✅ Can confirm payment with valid transaction ID
✅ Payment confirmation calls Banking API
✅ Rental status updates to Paid
✅ Error handling for invalid scenarios

---

Happy Testing! 🎉
