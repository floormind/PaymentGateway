namespace PaymentGateway.Service.Models.Interfaces
{
    public interface IStripePaymentGatewayConfiguration
    {
        public string PublishableKey { get; set; }
        public string SecreteKey { get; set; }
    }
}