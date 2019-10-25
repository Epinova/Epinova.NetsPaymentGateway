using System.Threading.Tasks;

namespace Epinova.NetsPaymentGateway
{
    public interface IPaymentGatewayService
    {
        /// <summary>
        /// The authorization (AUTH) must for most payment methods be done within 24 hours of the customer being
        /// redirected back from the payment window.
        /// </summary>
        Task<bool> AuthorizeAsync(MerchantInfo merchant, string transactionId);

        /// <summary>
        /// If the purchase is cancelled and you have only authorised the corresponding payment, you need to cancel
        /// the authorisation by using the cance (ANNUL) call. During the cancellation, the cash provision previously
        /// made for the customer's account is cancelled. If you have already performed the Capture call, the amount
        /// has already been captured from the customer, and Credit call need to be done instead.
        /// </summary>
        Task<bool> CancelAsync(MerchantInfo merchant, string transactionId);

        /// <summary>
        /// The CAPTURE operation needs to be done while the authorization is still valid, if applicable. Many Issuers
        /// are using a 21-day limit. Beyond this, the response depends on the Issuer and the specific account
        /// </summary>
        Task<bool> CaptureAsync(MerchantInfo merchant, string transactionId, decimal? amount = null);

        /// <summary>
        /// The CREDIT operation is available for 365 days
        /// </summary>
        Task<bool> CreditAsync(MerchantInfo merchant, string transactionId, decimal? amount = null);

        /// <summary>
        /// This query call can be used to request information related to the transaction from Netaxept. Netaxept then
        /// returns detailed information concerning the transaction, such as the status of the transaction and the
        /// payment method used. 
        /// </summary>
        Task<PaymentInformation> GetStatusAsync(MerchantInfo merchant, string transactionId);

        /// <summary>
        /// The purpose of the Register call is to send all the data needed to complete a transaction to Netaxept.
        /// During the registration, Netaxept will accept and store the transaction, and reply with a transaction ID
        /// that refers to the transaction in question.
        /// </summary>
        Task<RegisterResponse> RegisterAsync(MerchantInfo merchant, RegisterRequest request);
    }
}
