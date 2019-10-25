using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Epinova.Infrastructure;
using Epinova.Infrastructure.Logging;
using EPiServer.Logging;

namespace Epinova.NetsPaymentGateway
{
    public class PaymentGatewayService : RestServiceBase, IPaymentGatewayService
    {
        internal static HttpClient Client = new HttpClient();
        private readonly ILogger _log;
        private readonly IMapper _mapper;

        public PaymentGatewayService(ILogger log, IMapper mapper) : base(log)
        {
            _log = log;
            _mapper = mapper;
        }

        public async Task<bool> AuthorizeAsync(MerchantInfo merchant, string transactionId)
        {
            bool isRetry = false;
            while (true)
            {
                _log.Debug($"hitting AUTH {transactionId}{(isRetry ? " second time" : "")}");
                bool authResult;

                try
                {
                    authResult = await ProcessAsync(merchant, "AUTH", transactionId);
                }
                catch (Exception ex)
                {
                    _log.Error($"Auth call on {transactionId} threw up", ex);
                    authResult = false;
                }

                if (authResult)
                    return true;

                _log.Information($"Unable to authorize{(isRetry ? " on retry" : "")}. Trying to check status to see if it is already authorized.");
                PaymentInformation transaction = await GetStatusAsync(merchant, transactionId);
                if (transaction.IsAuthorized)
                    return true;

                if (isRetry)
                    return false;

                isRetry = true;
            }
        }

        public async Task<bool> CancelAsync(MerchantInfo merchant, string transactionId)
        {
            PaymentInformation status = await GetStatusAsync(merchant, transactionId);
            if (status.IsCancelled)
            {
                _log.Warning(new { message = "Transaction already cancelled", transactionId });
                return true;
            }

            return await ProcessAsync(merchant, "ANNUL", transactionId);
        }

        public async Task<bool> CaptureAsync(MerchantInfo merchant, string transactionId, decimal? amount = null)
        {
            return await ProcessAsync(merchant, "CAPTURE", transactionId, amount);
        }

        public async Task<bool> CreditAsync(MerchantInfo merchant, string transactionId, decimal? amount = null)
        {
            return await ProcessAsync(merchant, "CREDIT", transactionId, amount);
        }

        public async Task<PaymentInformation> GetStatusAsync(MerchantInfo merchant, string transactionId)
        {
            if (String.IsNullOrWhiteSpace(transactionId))
            {
                _log.Warning(new { message = "Invalid query request", transactionId });
                return null;
            }

            _log.Debug($"hitting status check for {transactionId}");

            var parameters = new Dictionary<string, string>
            {
                { "merchantId", merchant.MerchantId },
                { "token", merchant.Token },
                { "transactionId", transactionId },
            };

            string baseAddress = GetBaseAddress(merchant);
            string url = $"{baseAddress}Netaxept/Query.aspx?{BuildQueryString(parameters)}";

            HttpResponseMessage responseMessage = await CallAsync(() => Client.GetAsync(url));

            if (responseMessage == null)
            {
                _log.Error(new { message = "Status query failed. Service response was NULL", transactionId });
                return null;
            }

            QueryResponseDto dto = await ParseXmlAsync<QueryResponseDto>(responseMessage);

            if (dto.HasError)
            {
                _log.Error(new { message = "Status query failed.", transactionId, dto.ErrorMessage });
                return null;
            }

            return _mapper.Map<PaymentInformation>(dto);
        }


        public async Task<RegisterResponse> RegisterAsync(MerchantInfo merchant, RegisterRequest request)
        {
            if (request == null || !request.IsValid())
            {
                _log.Warning(new { message = "Invalid request", request });
                return null;
            }

            var parameters = new Dictionary<string, string>
            {
                { "merchantId", merchant.MerchantId },
                { "token", merchant.Token },
                { "orderNumber", request.OrderNumber },
                { "amount", AmountHelper.Inflate(request.Amount) },
                { "CurrencyCode", request.CurrencyCode },
                { "redirectUrl", Convert.ToString(request.RedirectUrl) },
                { "orderDescription", request.OrderDescription }
            };

            string[] paymentMethods = request.PaymentMethods.Any() ? request.PaymentMethods.ToArray() : merchant.DefaultPaymentMethods;

            if (paymentMethods.Any())
            {
                var paymentMethodsJson = new StringBuilder();
                paymentMethodsJson.Append("[{");
                paymentMethodsJson.Append(String.Join("},{", paymentMethods.Select(p => $"\"PaymentMethod\":\"{p}\"")));
                paymentMethodsJson.Append("}]");

                parameters.Add("paymentMethodActionList", paymentMethodsJson.ToString());
            }

            string baseAddress = GetBaseAddress(merchant);
            string url = $"{baseAddress}Netaxept/Register.aspx?{BuildQueryString(parameters)}";

            HttpResponseMessage responseMessage = await CallAsync(() => Client.GetAsync(url));

            if (responseMessage == null)
            {
                _log.Error(new { message = "Register failed. Service response was NULL", request });
                return null;
            }

            RegisterResponseDto dto = await ParseXmlAsync<RegisterResponseDto>(responseMessage);

            if (dto.HasError)
            {
                _log.Error(new { message = "Register failed.", request, dto.ErrorMessage });
                return null;
            }

            var response = _mapper.Map<RegisterResponse>(dto);
            response.TerminalUrl = new Uri($"{baseAddress}Terminal/default.aspx?merchantId={merchant.MerchantId}&transactionId={dto.TransactionId}");

            return response;
        }

        private static string GetBaseAddress(MerchantInfo merchant)
        {
            return $"https://{(merchant.IsTestEnvironment ? "test." : null)}epayment.nets.eu/";
        }

        private async Task<bool> ProcessAsync(MerchantInfo merchant, string operation, string transactionId, decimal? amount = null)
        {
            if (String.IsNullOrWhiteSpace(transactionId) || (amount.HasValue && amount < 0))
            {
                _log.Warning(new { message = "Invalid process request", operation, transactionId, amount });
                return false;
            }

            var parameters = new Dictionary<string, string>
            {
                { "merchantId", merchant.MerchantId },
                { "token", merchant.Token },
                { "transactionId", transactionId },
                { "operation", operation }
            };
            if (amount.HasValue)
                parameters.Add("transactionAmount", AmountHelper.Inflate(amount.Value));

            string url = $"{GetBaseAddress(merchant)}Netaxept/Process.aspx?{BuildQueryString(parameters)}";

            HttpResponseMessage responseMessage = await CallAsync(() => Client.GetAsync(url));

            if (responseMessage == null)
            {
                _log.Error(new { message = $"Operation '{operation}' failed. Service response was NULL", transactionId, amount });
                return false;
            }

            ProcessResponseDto dto = await ParseXmlAsync<ProcessResponseDto>(responseMessage);

            if (dto.HasError)
            {
                _log.Error(new { message = $"Operation '{operation}' failed.", transactionId, amount, dto.ErrorMessage });
                return false;
            }

            bool isOk = dto.ResponseCode == "OK";

            if (!isOk)
                _log.Warning(new { message = $"Operation '{operation}' failed", transactionId, amount, response = dto });

            _log.Information(new { message = $"Operation '{operation}' succeeded", amount, response = dto });
            return isOk;
        }
    }
}
