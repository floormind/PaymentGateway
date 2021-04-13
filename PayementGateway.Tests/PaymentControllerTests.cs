using System;
using Braintree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentGateway.Repository.Interfaces;
using PaymentGateway.Repository.Models.Classes;
using PaymentGateway.Repository.Models.Interfaces;
using PaymentGateway.Service.Controllers;
using PaymentGateway.Service.Helper.Interfaces;
using PaymentGateway.Service.Models.Classes;
using Stripe;

namespace PayementGateway.Tests
{
    public class Tests
    {
        private PaymentController _paymentController;
        private Mock<IRepository> _repositoryMock;
        private Mock<IPaymentGatewayAdapter<Charge>> _paymentGatewayAdapterMock;
        private Mock<ILogger<PaymentController>> _loggerMock;
        
        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository>();
            // I will only be demonstrating testing the transaction with Stripe
            // However the same can be achieved by mocking the IPaymentGatewayAdapter with Transaction as the Generic type.
            _paymentGatewayAdapterMock = new Mock<IPaymentGatewayAdapter<Charge>>();
            _loggerMock = new Mock<ILogger<PaymentController>>();
            
            _paymentController = new PaymentController(_paymentGatewayAdapterMock.Object, _repositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public void Purchase_With_Invalid_Purchase_Model_Returns_BadRequest()
        {
            //arrange
            var paymentModel = It.IsAny<PurchaseModel>();
            _paymentGatewayAdapterMock
                .Setup(x => x.ProcessPayment(paymentModel))
                .Returns(It.IsAny<IPaymentRecord>());
            
            //act
            var sut = (BadRequestObjectResult)_paymentController.Purchase(paymentModel);

            //assert
            Assert.AreEqual(sut.StatusCode, 400);
        }
        
        [Test]
        public void Purchase_With_Valid_Purchase_Model_Returns_Ok()
        {
            //arrange
            var paymentModel = new PurchaseModel();
            
            _paymentGatewayAdapterMock
                .Setup(x => x.ProcessPayment(paymentModel))
                .Returns(new InternalPaymentRecord());
            
            _repositoryMock
                .Setup(x => x.CreateRecord(new InternalPaymentRecord()))
                .Returns(true);
            
            //act
            var sut = (OkObjectResult)_paymentController.Purchase(paymentModel);

            //assert
            Assert.AreEqual(sut.StatusCode, 200);
        }
        
        [Test]
        public void Purchase_With_PaymentAdapterProcessPayment_Returning_Null_Throws_Exception()
        {
            //arrange
            var paymentModel = new PurchaseModel();
            
            _paymentGatewayAdapterMock
                .Setup(x => x.ProcessPayment(paymentModel))
                .Throws<Exception>();
            
            _repositoryMock
                .Setup(x => x.CreateRecord(new InternalPaymentRecord()))
                .Returns(true);
            
            //act
            var sut = (ObjectResult)_paymentController.Purchase(paymentModel);

            //assert
            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(1));
            
            Assert.AreEqual(sut.StatusCode, 500);
        }
        
        [Test]
        public void Purchase_With_RepositoryCreateRecord_Returning_False_Logs_Warning()
        {
            var paymentModel = new PurchaseModel();
            
            _paymentGatewayAdapterMock
                .Setup(x => x.ProcessPayment(paymentModel))
                .Returns(new InternalPaymentRecord());
            
            _repositoryMock
                .Setup(x => x.CreateRecord(new InternalPaymentRecord()))
                .Returns(false);
            
            //act
            var sut = (OkObjectResult)_paymentController.Purchase(paymentModel);

            //assert
            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(1));
            
            Assert.AreEqual(sut.StatusCode, 200);
        }

        [Test]
        public void History_With_Invalid_Transaction_Id_Should_Return_BadRequest()
        {
            //arrange
            var paymentReference = It.IsAny<string>();
            _paymentGatewayAdapterMock
                .Setup(x => x.GetPaymentHistory(paymentReference))
                .Returns(It.IsAny<Charge>());
            
            //act
            var sut = (BadRequestObjectResult)_paymentController.History(paymentReference);

            //assert
            Assert.AreEqual(sut.StatusCode, 400);
        }
        
        [Test]
        public void History_With_Unfound_Transaction_Id_Should_Return_Notfound()
        {
            //arrange
            var paymentReference = "12345";
            _paymentGatewayAdapterMock
                .Setup(s => s.GetPaymentHistory(paymentReference))
                .Returns(It.IsAny<Charge>());
            
            //act
            var sut = (NotFoundObjectResult)_paymentController.History(paymentReference);

            //assert
            Assert.AreEqual(sut.StatusCode, 404);
        }
        
        [Test]
        public void History_With_Valid_Transaction_Id_Should_Return_Ok()
        {
            //arrange
            var paymentReference = "12345";
            _paymentGatewayAdapterMock
                .Setup(s => s.GetPaymentHistory(paymentReference))
                .Returns(new Charge());
            
            //act
            var sut = (OkObjectResult)_paymentController.History(paymentReference);

            //assert
            Assert.AreEqual(sut.StatusCode, 200);
        }
    }
}