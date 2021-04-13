using System;
using System.Collections.Generic;

namespace PaymentGateway.Service.Models.Classes
{
    public class Basket
    {
        public Guid Id { get; set; }
        public IList<BasketItem> BasketItem { get; set; }
        public string Total { get; set; }
    }
}