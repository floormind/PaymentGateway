namespace PaymentGateway.Service.Models.Classes
{
    public class PaymentHistory
    {
        public string OrderId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Amount { get; set; }
        public string Merchant { get; set; }
        public string MaskedCreditCard { get; set; }
        public string CreatedDate { get; set; }
        
    }
}