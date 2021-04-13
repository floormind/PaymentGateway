using System;
using System.Collections.Generic;
using Braintree;
using PaymentGateway.Repository.Models.Classes;
using PaymentGateway.Repository.Models.Interfaces;
using PaymentGateway.Service.Helper.Interfaces;
using PaymentGateway.Service.Models.Classes;
using PaymentGateway.Service.Models.Interfaces;

namespace PaymentGateway.Service.Helper.Classes
{

    public class BraintreePaymentAdapter<T> : IPaymentGatewayAdapter<T> where T : Transaction
    {
        private readonly IBraintreePaymentGatewayConfiguration _braintreePaymentGatewayConfiguration;
        private readonly IBraintreeGateway _gateway;
        private readonly IList<PaymentHistory> _paymentHistories;
        private readonly IPaymentRecord _paymentRecord;

        public BraintreePaymentAdapter(IBraintreePaymentGatewayConfiguration braintreePaymentGatewayConfiguration,
            IList<PaymentHistory> paymentHistories, IPaymentRecord paymentRecord)
        {
            _braintreePaymentGatewayConfiguration = braintreePaymentGatewayConfiguration;
            _paymentHistories = paymentHistories;
            _paymentRecord = paymentRecord;
            _gateway = ConstructGateway();
        }

        private IBraintreeGateway ConstructGateway()
        {
            var constructedGateway = new BraintreeGateway()
            {
                MerchantId = _braintreePaymentGatewayConfiguration.MerchantId,
                PublicKey = _braintreePaymentGatewayConfiguration.PublicKey,
                PrivateKey = _braintreePaymentGatewayConfiguration.PrivateKey,
                Environment = Braintree.Environment.SANDBOX
            };
            
            return constructedGateway;
        }
        public string GenerateToken()
        {
            var token = _gateway.ClientToken.Generate();
            return token;
        }

        public IPaymentRecord ProcessPayment(PurchaseModel purchaseModel)
        {
            var transactionRequest = new TransactionRequest()
            {
                OrderId = Guid.NewGuid().ToString(),
                Amount = Convert.ToDecimal(purchaseModel.Basket.Total),
                CustomerId = purchaseModel.Shopper.CustomerGatewayId,
                Options = new TransactionOptionsRequest()
                {
                    SubmitForSettlement = true
                },
                // For the purpose of this challenge, i have added a credit card details here, 
                // ideally this will come with from the client within the payload
                // adding it here isnt ideal and safe as it should be encrypted
                // however this is just to show how the data will be passed to Braintree
                CreditCard = new TransactionCreditCardRequest()
                {
                    Number = "4111111111111111",
                    ExpirationDate = "06/2022",
                    CVV = "123"
                }
            };
            var result = _gateway.Transaction.Sale(transactionRequest);
            if (!result.IsSuccess()) return null;
            
            CreatePaymentRecordObject(purchaseModel, (T)result.Target);
            
            return _paymentRecord;
        }
        
        public void CreatePaymentRecordObject(PurchaseModel purchaseModel, T charge)
        {
            _paymentRecord.Id = Guid.NewGuid();
            _paymentRecord.CustomerGatewayId = purchaseModel.Shopper.CustomerGatewayId;
            _paymentRecord.Amount = Convert.ToDecimal(charge.Amount);
            _paymentRecord.DateCreated = charge.CreatedAt.Value;
            _paymentRecord.GatewayReferenceId = charge.Id;
            // A check can be done here against the shopper's ID to check if we the shopper is a logged in or is a guest
            // if the shopper is a gues we can store the first and last names
            // otherwise we can just store the shopper's ID instead. 
            // but for the purpose of this test, i will store all 3 information
            _paymentRecord.ShopperId = purchaseModel.Shopper.Id;
            _paymentRecord.CustomerGatewayId = purchaseModel.Shopper.CustomerGatewayId;
            _paymentRecord.ShopperFirstName = purchaseModel.Shopper.FirstName;
            _paymentRecord.ShopperLastName = purchaseModel.Shopper.LastName;
            _paymentRecord.ShopperEmail = purchaseModel.Shopper.Email;
            _paymentRecord.Status =
                "Completed"; // this may need to be mapped to a constant to keep the database entry for this consistent
            _paymentRecord.LastFour = charge.CreditCard.LastFour;
            _paymentRecord.CardBrand = charge.CreditCard.CardType.ToString();
            _paymentRecord.Currency = charge.CurrencyIsoCode;
        }

        public T GetPaymentHistory(string id)
        {
            var request = new TransactionSearchRequest().Id.Is(id);
            ResourceCollection<Transaction> collection = _gateway.Transaction.Search(request);
            return (T) collection.FirstItem;
        }
    }
}
