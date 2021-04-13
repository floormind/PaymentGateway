using System;
using System.Collections.Generic;
using PaymentGateway.Repository.Models.Interfaces;
using PaymentGateway.Service.Models.Classes;

namespace PaymentGateway.Service.Helper.Interfaces
{
    public interface IPaymentGatewayAdapter<T>
    {
        public string GenerateToken();
        public IPaymentRecord ProcessPayment(PurchaseModel purchaseModel);
        public void CreatePaymentRecordObject(PurchaseModel purchaseModel, T charge);
        public T GetPaymentHistory(string id);
    }
}
