using PaymentGateway.Service.Models.Interfaces;

namespace PaymentGateway.Service.Models.Classes
{
    public class BraintreeBraintreePaymentGatewayConfiguration : IBraintreePaymentGatewayConfiguration
    {
        public string MerchantId  { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}