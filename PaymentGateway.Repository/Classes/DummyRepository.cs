using PaymentGateway.Repository.Interfaces;
using PaymentGateway.Repository.Models.Interfaces;

namespace PaymentGateway.Repository.Classes
{
    public class DummyRepository : IRepository
    {
        // A repoisitory that creates a record on our database
        // this is good as we do not have a record for the payment such as card information
        // however we can cross check against Stripe/Braintree at any point in time with the references on our database/
        public bool CreateRecord(IPaymentRecord paymentRecord)
        {
            return true;
        }
    }
} 