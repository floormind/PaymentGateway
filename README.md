# PaymentGateway

1) To Process a payment call the endpoint **http://localhost:5000/payment** with the below data. It will return Payment Successful with a transaction reference which can be used to get the transaction.
2) I created a customer manually on Stripe & Braintree, so if the Stripe gateway is being used use the customergatewayId which is "cus_JINFZ0SrrdVncv"
3) If the Braintree Gateway is being used, you have the change the customergatwayid to 869768490

**http://localhost:5000/payment** 
{
    "shopper": {
        "id": "97fae51d-d210-4d7e-ac04-f8b04800eb95",
        "customergatewayid": "cus_JINFZ0SrrdVncv",
        "firstname": "Ife",
        "lastname": "Ayelabola",
        "email": "ife_labolz@hotmail.com"
    },
    "basket": {
        "id": "d717cec8-9c47-4028-a5f9-a829ba39d960",
        "basketItem": [
            {
                "id": "cd8b6023-0168-42cc-a00c-5d31cd835635",
                "name": "PS5",
                "price": "500"
            }
        ],
        "total": "500"
    },
    "clienttoken" : ""
}

4) To fetch a transaction, call the endpoint with a transaction id, one which was returned from the endpoint above 
http://localhost:5000/payment/payment-history/{transaction-id}
