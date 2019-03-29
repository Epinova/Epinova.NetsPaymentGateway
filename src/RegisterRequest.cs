using System;
using System.Collections.Generic;

namespace Epinova.NetsPaymentGateway
{
    public class RegisterRequest
    {
        public RegisterRequest()
        {
            PaymentMethods = new List<string>();
        }

        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string OrderDescription { get; set; }
        public string OrderNumber { get; set; }
        public List<string> PaymentMethods { get; set; }
        public Uri RedirectUrl { get; set; }

        public bool IsValid()
        {
            return Amount > 0
                   && !String.IsNullOrWhiteSpace(CurrencyCode)
                   && !String.IsNullOrWhiteSpace(OrderDescription)
                   && !String.IsNullOrWhiteSpace(OrderNumber)
                   && !String.IsNullOrWhiteSpace(Convert.ToString(RedirectUrl));
        }
    }
}