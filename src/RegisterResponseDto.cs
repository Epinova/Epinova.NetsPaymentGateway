using System.Xml.Serialization;

namespace Epinova.NetsPaymentGateway
{
    /// <remarks>DTO has to be public to enable XmlSerializer's deserialize support.</remarks>
    [XmlRoot("RegisterResponse")]
    public class RegisterResponseDto : ResponseDtoBase
    {
        public string TransactionId { get; set; }
    }
}