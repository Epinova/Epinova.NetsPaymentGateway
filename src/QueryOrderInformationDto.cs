using System;

namespace Epinova.NetsPaymentGateway
{
    public class QueryOrderInformationDto
    {
        public int Amount { get; set; }
        public string Currency { get; set; }
        public int Fee { get; set; }
        public string OrderDescription { get; set; }
        public string OrderNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public int Total { get; set; }
    }
}
