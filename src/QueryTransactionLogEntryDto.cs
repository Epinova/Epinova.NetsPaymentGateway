using System;

namespace Epinova.NetsPaymentGateway
{
    public class QueryTransactionLogEntryDto
    {
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
        public string Operation { get; set; }
        public string TransactionReconRef { get; set; }
    }
}