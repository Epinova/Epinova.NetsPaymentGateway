using System;
using System.Xml.Serialization;
using Epinova.Infrastructure;

namespace Epinova.NetsPaymentGateway
{
    /// <remarks>DTO has to be public to enable XmlSerializer's deserialize support.</remarks>
    [XmlRoot("ProcessResponse")]
    public class ProcessResponseDto : ServiceResponseBase
    {
        public string AuthorizationId { get; set; }
        public DateTimeOffset ExecutionTime { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseSource { get; set; }
        public string ResponseText { get; set; }
        public string TransactionId { get; set; }
    }
}