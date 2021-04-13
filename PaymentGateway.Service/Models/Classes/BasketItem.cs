using System;

namespace PaymentGateway.Service.Models.Classes
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
    }
}