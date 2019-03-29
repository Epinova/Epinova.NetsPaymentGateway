using System.Xml.Serialization;
using Epinova.Infrastructure;

namespace Epinova.NetsPaymentGateway
{
    /// <remarks>DTO has to be public to enable XmlSerializer's deserialize support.</remarks>
    [XmlRoot("RegisterResponse")]
    public class RegisterResponseDto : ServiceResponseBase
    {
        public string TransactionId { get; set; }
    }
}