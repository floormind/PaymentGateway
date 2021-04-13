using System;

namespace PaymentGateway.Service.Models.Classes
{
    public class Shopper
    {
        public Guid Id { get; set; }
        public string CustomerGatewayId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}