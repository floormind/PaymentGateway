namespace PaymentGateway.Service.Models.Classes
{
    public class PurchaseModel
    {
        public Shopper Shopper { get; set; }
        public Basket Basket { get; set; }
        public string ClientToken { get; set; }
    }
}