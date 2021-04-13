using PaymentGateway.Service.Models.Interfaces;

namespace PaymentGateway.Service.Models.Classes
{
    public class StripePaymentGatewayConfiguration : IStripePaymentGatewayConfiguration
    {
        public string PublishableKey { get; set; }
        public string SecreteKey { get; set; }
    }
}