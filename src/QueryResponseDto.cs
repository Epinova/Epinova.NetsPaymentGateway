using System.Xml.Serialization;

namespace Epinova.NetsPaymentGateway
{
    /// <remarks>DTO has to be public to enable XmlSerializer's deserialize support.</remarks>
    [XmlRoot("PaymentInfo")]
    public class QueryResponseDto : ResponseDtoBase
    {
        public QueryCardInformationDto CardInformation { get; set; }

        [XmlArray(IsNullable = false)]
        [XmlArrayItem("TransactionLogLine")]
        public QueryTransactionLogEntryDto[] History { get; set; }

        public QueryOrderInformationDto OrderInformation { get; set; }
        public QuerySummaryDto Summary { get; set; }
        public string TransactionId { get; set; }
    }
}
