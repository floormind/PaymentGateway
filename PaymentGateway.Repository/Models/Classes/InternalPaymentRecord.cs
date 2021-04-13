using System;
using PaymentGateway.Repository.Models.Interfaces;

namespace PaymentGateway.Repository.Models.Classes
{
    public class InternalPaymentRecord : IPaymentRecord
    {
        public Guid Id { get; set; }
        public string CustomerGatewayId { get; set; }
        public string GatewayReferenceId { get; set; }
        public DateTime DateCreated { get; set; }
        public string PaymentGateway { get; set; }
        public decimal Amount { get; set; }
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