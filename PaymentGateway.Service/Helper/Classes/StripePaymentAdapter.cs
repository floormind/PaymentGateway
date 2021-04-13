using System;
using System.Collections.Generic;
using PaymentGateway.Repository.Models.Interfaces;
using PaymentGateway.Service.Helper.Interfaces;
using PaymentGateway.Service.Models.Classes;
using PaymentGateway.Service.Models.Interfaces;
using Stripe;
using Stripe.Issuing;

namespace PaymentGateway.Service.Helper.Classes
{
    public class StripePayementAdapter<T> : IPaymentGatewayAdapter<T> where T : Stripe.Charge
    {
        private readonly IStripePaymentGatewayConfiguration _stripePaymentGatewayConfiguration;
        private readonly IList<PaymentHistory> _paymentHistories;
        private readonly IPaymentRecord _paymentRecord;

        public StripePayementAdapter(IStripePaymentGatewayConfiguration stripePaymentGatewayConfiguration,
            IList<PaymentHistory> paymentHistories, IPaymentRecord paymentRecord)
        {
            _stripePaymentGatewayConfiguration = stripePaymentGatewayConfiguration;
            _paymentHistories = paymentHistories;
            _paymentRecord = paymentRecord;
        }

        public string GenerateToken()
        {
            //TestCard
            //This token is ideally supposed to come from client,
            //however being that there is no client at the moment, i am generating a token on the server side.
            StripeConfiguration.ApiKey = _stripePaymentGatewayConfiguration.SecreteKey;

            var options = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = "4242424242424242",
                    ExpMonth = 4,
                    ExpYear = 2022,
                    Cvc = "314",
                }
            };

            var service = new TokenService();
            var token = service.Create(options);
            return token.Id;
        }

        // If customer and source are used together
        // customer needs to be set to the customer id
        // source should be set to the card registered against the customer
        // if Source is the only value used and Customer isnt used,
        // then Source needs to the set to a token, which is created by GenerateToken() above.
        public IPaymentRecord ProcessPayment(PurchaseModel purchaseModel)
        {
            StripeConfiguration.ApiKey = _stripePaymentGatewayConfiguration.SecreteKey;
            var chargeOption = new ChargeCreateOptions()
            {
                Amount = (long) Convert.ToDouble(purchaseModel.Basket.Total) * 100,
                Currency = "gbp",
                Source = "card_1Ifmo9KPiE0X1r2qKIL5qGDO",
                Customer = purchaseModel.Shopper.CustomerGatewayId,
                ReceiptEmail = purchaseModel.Shopper.Email,
                Metadata = new Dictionary<string, string>()
                {
                    {"OrderId", Guid.NewGuid().ToString()}
                }
            };

            var result = new ChargeService().Create(chargeOption);
            if (result.Status != "succeeded") return null;
            
            CreatePaymentRecordObject(purchaseModel, (T)result);

            return _paymentRecord;
        }

        public void CreatePaymentRecordObject(PurchaseModel purchaseModel, T charge)
        {
            _paymentRecord.Id = Guid.NewGuid();
            _paymentRecord.Amount = Convert.ToDecimal(charge.Amount / 100);
            _paymentRecord.DateCreated = charge.Created;
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
            _paymentRecord.LastFour = charge.PaymentMethodDetails.Card.Last4;
            _paymentRecord.CardBrand = charge.PaymentMethodDetails.Card.Brand;
            _paymentRecord.Currency = charge.Currency;
        }

        public T GetPaymentHistory(string id)
        {
            //TestCard
            //This token is ideally supposed to come from client,
            //however being that there is no client at the moment, i am generating a token on the server side.
            StripeConfiguration.ApiKey = _stripePaymentGatewayConfiguration.SecreteKey;
            
            var chargeService = new ChargeService();
            var charge = chargeService.Get(id);
            return (T)charge;
        }
    }
}