using System;

namespace PaymentGateway.Repository.Models.Interfaces
{
    public interface IPaymentRecord
    {
        public Guid Id { get; set; }
        public string CustomerGatewayId { get; set; }
        public string GatewayReferenceId { get; set; } //Gateways payment ID, we can use this to search for the payment on Stripe or Braintree.
        public DateTime DateCreated { get; set; }
        public String PaymentGateway { get; set; } //Whether Stripe or Braintree or any other like Paypal
        public Decimal Amount { get; set; }
        public Guid ShopperId { get; set; }
        public string ShopperFirstName { get; set; }
        public string ShopperLastName { get; set; }
        public string ShopperEmail { get; set; }
        public string Status { get; set; }
        public string CardBrand { get; set; }
        public string LastFour { get; set; }
        public string Currency { get; set; }
    }
}