using PaymentGateway.Repository.Models.Interfaces;

namespace PaymentGateway.Repository.Interfaces
{
    public interface IRepository
    {
        public bool CreateRecord(IPaymentRecord paymentRecord);
    }
}
