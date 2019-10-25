namespace Epinova.NetsPaymentGateway
{
    public class QuerySummaryDto
    {
        public int AmountCaptured { get; set; }
        public int AmountCredited { get; set; }
        public bool Annulled { get; set; }
        public string AuthorizationId { get; set; }
        public bool Authorized { get; set; }
    }
}
