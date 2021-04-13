using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Repository.Interfaces;
using PaymentGateway.Repository.Models.Interfaces;
using PaymentGateway.Service.Helper.Interfaces;
using PaymentGateway.Service.Models.Classes;
using Stripe;

namespace PaymentGateway.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        //Two payment gateways have been implemented, 
        //One for Stripe, and the other for Braintree.
        //Uncomment one and replace the generic type for testing.
        
        //Stripe
        private readonly IPaymentGatewayAdapter<Charge> _paymentGatewayAdapter;
        
        //Braintree
        //private readonly IPaymentGatewayAdapter<Transaction> _paymentGatewayAdapter; 
        
        private readonly IRepository _repository;

        private ILogger<PaymentController> _logger;

        public PaymentController(IPaymentGatewayAdapter<Charge> paymentGatewayAdapter, IRepository repository,
            ILogger<PaymentController> logger)
        {
            _paymentGatewayAdapter = paymentGatewayAdapter;
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("get-token")]
        public IActionResult GetToken()
        {
            var token = _paymentGatewayAdapter.GenerateToken();
            return Ok(token);
        }

        [HttpPost]
        public IActionResult Purchase(PurchaseModel model)
        {
            try
            {
                if (null == model) return BadRequest("Supply payment details");
                var result = _paymentGatewayAdapter.ProcessPayment(model);
                if (null == result) throw new Exception("There was an issue processing your payment, please try again.");

                //store payment for our records.
                var recordCreated = _repository.CreateRecord(result);
                if(!recordCreated)
                    _logger.LogWarning("Record not created for the purchase", model);
                    
                return Ok($"Payment Successful. Payment reference is => {result.GatewayReferenceId}"); // response may need to be specific when introducing a client. The reference can be returned and the client will then decide how to render the message
            }
            catch (Exception e)
            {
                _logger.LogError("Error charging payment", e.InnerException);
                return Problem("There seems to be a problem with the payment, we are looking into this for you");
            }
        }

        [HttpGet("payment-history/{id}")]
        public IActionResult History(string id)
        {
            try
            {
                if (null == id) return BadRequest("Id is not valid");
                var result = _paymentGatewayAdapter.GetPaymentHistory(id);

                if (result == null) return NotFound("There are no payment history for this account");

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting payment history", e.InnerException);
                return Problem("There seems to be a problem fetching payment history, please try again later");
            }
        }
        
    }
}
