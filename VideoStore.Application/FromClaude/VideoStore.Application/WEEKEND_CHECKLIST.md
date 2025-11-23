# VideoStore Payment Integration - Weekend Implementation Checklist

## ✅ What's Been Done (Code Complete)

All code changes are complete and ready to use:

1. **Rental Model** - Added payment tracking fields (PaymentTransactionId, PaymentStatus, RentalAmount)
2. **PaymentStatus Enum** - Pending, Paid, Failed, Refunded
3. **Database Migration Script** - `add_payment_columns.sql`
4. **Repository Updates** - Added ConfirmPayment method
5. **Banking API Service** - HttpClient service to call Banking API
6. **Payment Confirmation Endpoint** - POST /api/rentals/{id}/confirm-payment
7. **Program.cs** - HttpClient registered with DI
8. **Configuration** - appsettings.json updated with Banking API URL

---

## 🎯 Your Weekend Tasks

### Saturday Morning (2-3 hours)

1. **Run Database Migration**
   ```bash
   psql -U postgres -d videostore -f add_payment_columns.sql
   ```

2. **Build and Test Compilation**
   ```bash
   cd VideoStore.Api
   dotnet build
   ```
   - Fix any compilation errors (shouldn't be any)
   - Make sure it builds successfully

3. **Start VideoStore API**
   ```bash
   dotnet run
   ```
   - Should start on http://localhost:5000
   - Test Swagger UI: http://localhost:5000/swagger

### Saturday Afternoon (2-3 hours)

4. **Manual Testing with Postman**
   - Create test customer
   - Create test video
   - Create rental with payment pending
   - Verify rental shows status "Pending"

5. **Banking API Setup** (if not already running)
   - Start your Banking API on port 5001
   - Create a test transaction
   - Note the transaction ID

### Sunday Morning (2-3 hours)

6. **Test Payment Confirmation Flow**
   - Use Postman to call confirm-payment endpoint
   - Verify it calls Banking API
   - Check rental status updates to "Paid"
   - Test error scenarios (invalid transaction, wrong amount, etc.)

7. **End-to-End Test**
   - Complete flow from rental creation to payment confirmation
   - Document any issues you find

### Sunday Afternoon (2-3 hours)

8. **ATM Integration** (if you have time)
   - Test with your actual ATM simulator
   - Process payment via ISO 8583
   - Use real transaction ID from ATM

9. **Documentation & Cleanup**
   - Document any changes you made
   - Update your GitHub repo
   - Write notes on what you learned

---

## 📋 Pre-Flight Checklist

Before you start testing, make sure you have:

- [ ] PostgreSQL running
- [ ] videostore database exists
- [ ] Database migration script executed
- [ ] VideoStore API builds successfully
- [ ] Banking API is available (or plan to mock it)
- [ ] Postman installed with test collection
- [ ] Test data ready (customer, video, transaction)

---

## 🔧 Quick Configuration Reference

**VideoStore API Port:** 5000
**Banking API Port:** 5001 (configurable in appsettings.json)

**Database Connection:**
```
Host=localhost;Port=5432;Database=videostore;Username=postgres;Password=test123
```

**New Rental Payload:**
```json
{
  "customerId": "GUID",
  "videoId": "GUID",
  "rentalAmount": 5.00,
  "paymentStatus": "Pending"
}
```

**Payment Confirmation Payload:**
```json
{
  "transactionId": "transaction-id-from-banking-api"
}
```

---

## 🚨 Common Issues & Solutions

### Issue: "Cannot connect to Banking API"
**Solution:** Check Banking API is running on port 5001, update appsettings.json if different

### Issue: "Table rentals doesn't have column payment_status"
**Solution:** Run the database migration script

### Issue: "Transaction not found"
**Solution:** Make sure you're using a valid transaction ID from your Banking API

### Issue: "Amount mismatch"
**Solution:** Rental amount must match transaction amount (within $0.01)

---

## 📊 Success Criteria

By end of weekend, you should have:

✅ Database updated with payment columns
✅ VideoStore API running with new endpoint
✅ Successfully created rental with Pending status
✅ Successfully confirmed payment with transaction ID
✅ Verified rental status changed to Paid
✅ Tested at least 3 error scenarios
✅ End-to-end flow working (either manual or with ATM)

---

## 🎉 Bonus Goals (if time permits)

- [ ] Update HTML frontend to show payment status
- [ ] Add payment confirmation UI to frontend
- [ ] Create Postman collection for automated testing
- [ ] Test with real ATM simulator
- [ ] Add logging for debugging
- [ ] Write integration tests

---

## 📞 Next Steps After Weekend

1. Add authentication between VideoStore and Banking API
2. Implement payment refund functionality
3. Add payment history tracking
4. Integrate with frontend UI
5. Deploy to test environment

---

Good luck with your implementation! 🚀

Remember: Start simple, test often, document as you go.
