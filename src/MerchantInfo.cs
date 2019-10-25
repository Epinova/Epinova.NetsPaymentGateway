using System.Net;

namespace Epinova.NetsPaymentGateway
{
    public class MerchantInfo
    {
        public string[] DefaultPaymentMethods { get; set; }
        public bool IsTestEnvironment { get; set; }
        public string MerchantId { get; set; }
        public string Token { get; set; }
        public IPAddress[] ValidCallbackIPs { get; set; }
    }
}
