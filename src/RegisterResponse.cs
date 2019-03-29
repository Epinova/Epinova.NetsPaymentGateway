using System;

namespace Epinova.NetsPaymentGateway
{
    public class RegisterResponse
    {
        public Uri TerminalUrl { get; set; }
        public string TransactionId { get; set; }
    }
}