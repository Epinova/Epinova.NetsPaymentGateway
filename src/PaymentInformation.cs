using System;

namespace Epinova.NetsPaymentGateway
{
    public class PaymentInformation
    {
        public string AuthorizationId { get; set; }
        public decimal CapturedAmount { get; set; }
        public decimal CreditedAmount { get; set; }
        public string Currency { get; set; }
        public decimal FeeAmount { get; set; }
        public bool IsAuthorized { get; set; }
        public bool IsCancelled { get; set; }
        public decimal OrderAmount { get; set; }
        public string OrderDescription { get; set; }
        public string OrderNumber { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionId { get; set; }

        //TODO: MN-299 - må utvides med info om feilet transaksjon. --tarjei
    }
}