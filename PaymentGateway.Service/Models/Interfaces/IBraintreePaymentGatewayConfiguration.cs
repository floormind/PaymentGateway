namespace PaymentGateway.Service.Models.Interfaces
{
    public interface IBraintreePaymentGatewayConfiguration
    { 
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}