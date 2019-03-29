using System.Threading.Tasks;

namespace Epinova.NetsPaymentGateway
{
    public interface IPaymentGatewayService
    {
        Task<bool> AuthorizeAsync(MerchantInfo merchant, string transactionId);
        Task<bool> CancelAsync(MerchantInfo merchant, string transactionId);
        Task<bool> CaptureAsync(MerchantInfo merchant, string transactionId, decimal? amount = null);
        Task<bool> CreditAsync(MerchantInfo merchant, string transactionId, decimal? amount = null);
        Task<PaymentInformation> GetStatusAsync(MerchantInfo merchant, string transactionId);
        Task<RegisterResponse> RegisterAsync(MerchantInfo merchant, RegisterRequest request);
    }
}