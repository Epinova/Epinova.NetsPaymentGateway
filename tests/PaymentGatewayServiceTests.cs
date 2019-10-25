using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Epinova.NetsPaymentGateway;
using EPiServer.Logging;
using Moq;
using Xunit;

namespace Epinova.NetsPaymentGatewayTests
{
    [Collection("Static http client mock")]
    public class PaymentGatewayServiceTests
    {
        private readonly Mock<ILogger> _logMock;
        private readonly MerchantInfo _merchantInfo;
        private readonly TestableHttpMessageHandler _messageHandler;
        private readonly PaymentGatewayService _service;

        public PaymentGatewayServiceTests()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile(new PaymentGatewayMappingProfile()); });
            _messageHandler = new TestableHttpMessageHandler();
            _logMock = new Mock<ILogger>();
            PaymentGatewayService.Client = new HttpClient(_messageHandler);
            _service = new PaymentGatewayService(_logMock.Object, mapperConfiguration.CreateMapper());
            _merchantInfo = new MerchantInfo
            {
                IsTestEnvironment = true,
                MerchantId = Factory.GetInteger().ToString(),
                Token = Factory.GetString(),
                DefaultPaymentMethods = new[] { "Visa", "MasterCard" }
            };
        }

        [Fact]
        public async Task GetStatus_ParseResultFails_ReturnsNull()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"<xml>Some random unparasable xml</xml>")
            });
            PaymentInformation result = await _service.GetStatusAsync(_merchantInfo, Factory.GetString());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetStatus_ServiceReturnsNull_LogsError()
        {
            _messageHandler.SendAsyncReturns(null);
            await _service.GetStatusAsync(_merchantInfo, Factory.GetString());

            _logMock.VerifyLog<object>(Level.Error, Times.Once());
        }

        [Fact]
        public async Task GetStatus_ServiceReturnsNull_ReturnsNull()
        {
            _messageHandler.SendAsyncReturns(null);
            PaymentInformation result = await _service.GetStatusAsync(_merchantInfo, Factory.GetString());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetStatus_ServiceReturnsValidXml_ReturnsCorrectOrderNumber()
        {
            string transactionId = Factory.GetString();
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GetValidQueryResultXml(transactionId))
            });
            PaymentInformation result = await _service.GetStatusAsync(_merchantInfo, transactionId);

            Assert.Equal("106", result.OrderNumber);
        }

        [Fact]
        public async Task GetStatus_ServiceReturnsValidXml_ReturnsCorrectPaymentMethod()
        {
            string transactionId = Factory.GetString();
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GetValidQueryResultXml(transactionId))
            });
            PaymentInformation result = await _service.GetStatusAsync(_merchantInfo, transactionId);

            Assert.Equal("Visa", result.PaymentMethod);
        }

        [Fact]
        public async Task GetStatus_ServiceReturnsValidXml_ReturnsCorrectTimestamp()
        {
            string transactionId = Factory.GetString();
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GetValidQueryResultXml(transactionId))
            });
            PaymentInformation result = await _service.GetStatusAsync(_merchantInfo, transactionId);

            Assert.Equal(DateTime.Parse("2017-07-26T13:44:39.2570000", CultureInfo.InvariantCulture), result.Timestamp);
        }

        [Fact]
        public async Task GetStatus_ServiceReturnsValidXml_ReturnsCorrectTotalAmount()
        {
            string transactionId = Factory.GetString();
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GetValidQueryResultXml(transactionId))
            });
            PaymentInformation result = await _service.GetStatusAsync(_merchantInfo, transactionId);

            Assert.Equal(1699, result.TotalAmount);
        }

        [Fact]
        public async Task GetStatus_ServiceReturnsValidXml_ReturnsCorrectTransactionId()
        {
            string transactionId = Factory.GetString();
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GetValidQueryResultXml(transactionId))
            });
            PaymentInformation result = await _service.GetStatusAsync(_merchantInfo, transactionId);

            Assert.Equal(transactionId, result.TransactionId);
        }

        [Fact]
        public async Task Register_ParseResultFails_ReturnsNull()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"<xml>Some random unparasable xml</xml>")
            });
            RegisterResponse result = await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            Assert.Null(result);
        }

        [Fact]
        public async void Register_ReturnedHttpStatusCodeIsNot200_LogWarning()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            _logMock.VerifyLog<string>(Level.Warning, Times.Once());
        }

        [Fact]
        public async void Register_ReturnedHttpStatusCodeIsNot200_ReturnNull()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            RegisterResponse result = await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            Assert.Null(result);
        }

        [Fact]
        public async Task Register_ServiceCallThrows_LogErrorMessage()
        {
            _messageHandler.SendAsyncThrows(new Exception());
            await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            _logMock.VerifyLog<string>(Level.Error, Times.Once());
        }

        [Fact]
        public async Task Register_ServiceCallThrows_ReturnsNull()
        {
            string errorMessage = Factory.GetString();
            _messageHandler.SendAsyncThrows(new Exception(errorMessage));
            RegisterResponse result = await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            Assert.Null(result);
        }

        [Fact]
        public async Task Register_ServiceReturnsException_LogErrorMessage()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    $"<Exception xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Error xsi:type=\"AuthenticationException\"><Message>Authentication failed (Test) MerchantId: {_merchantInfo.MerchantId}</Message></Error></Exception>")
            });
            await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            _logMock.VerifyLog(Level.Error, "Deserializing xml failed.", Times.Once());
        }

        [Fact]
        public async Task Register_ServiceReturnsException_ReturnsNull()
        {
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    $"<Exception xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Error xsi:type=\"AuthenticationException\"><Message>Authentication failed (Test) MerchantId: {_merchantInfo.MerchantId}</Message></Error></Exception>")
            });
            RegisterResponse result = await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            Assert.Null(result);
        }

        [Fact]
        public async Task Register_ServiceReturnsNull_LogsError()
        {
            _messageHandler.SendAsyncReturns(null);
            await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            _logMock.VerifyLog<object>(Level.Error, Times.Once());
        }

        [Fact]
        public async Task Register_ServiceReturnsNull_ReturnsNull()
        {
            _messageHandler.SendAsyncReturns(null);
            RegisterResponse result = await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            Assert.Null(result);
        }

        [Fact]
        public async Task Register_ServiceReturnsValidXml_ReturnsCorrectTerminalUrl()
        {
            string transactionId = Factory.GetString();
            string expectedTerminalUrl = $"https://test.epayment.nets.eu/Terminal/default.aspx?merchantId={_merchantInfo.MerchantId}&transactionId={transactionId}";
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    $"<RegisterResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><TransactionId>{transactionId}</TransactionId></RegisterResponse>")
            });
            RegisterResponse result = await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            Assert.Equal(expectedTerminalUrl, result.TerminalUrl.ToString());
        }

        [Fact]
        public async Task Register_ServiceReturnsValidXml_ReturnsCorrectTransactionId()
        {
            string transactionId = Factory.GetString();
            _messageHandler.SendAsyncReturns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    $"<RegisterResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><TransactionId>{transactionId}</TransactionId></RegisterResponse>")
            });
            RegisterResponse result = await _service.RegisterAsync(_merchantInfo, RegisterRequestTests.GetValidRequest());

            Assert.Equal(transactionId, result.TransactionId);
        }


        private static string GetValidQueryResultXml(string transactionId)
        {
            return $@"
<PaymentInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema\"">
    <MerchantId>12002556</MerchantId>
    <QueryFinished>2017-07-26T13:46:58.4145387+02:00</QueryFinished>
    <TransactionId>{transactionId}</TransactionId>
    <AuthenticationInformation />
    <AvtaleGiroInformation />
    <CardInformation>
        <ExpiryDate>1901</ExpiryDate>
        <Issuer>Visa</Issuer>
        <IssuerCountry>NO</IssuerCountry>
        <MaskedPAN>492500******0004</MaskedPAN>
        <PaymentMethod>Visa</PaymentMethod>
        <IssuerId>3</IssuerId>
    </CardInformation>
    <CustomerInformation>
        <Address1 />
        <Address2 />
        <CompanyName />
        <CompanyRegistrationNumber />
        <CustomerNumber />
        <Country />
        <Email />
        <FirstName />
        <IP>79.135.17.87</IP>
        <LastName />
        <PhoneNumber />
        <Postcode />
        <SocialSecurityNumber />
        <Town />
    </CustomerInformation>
    <ErrorLog />
    <History>
        <TransactionLogLine>
            <DateTime>2017-07-26T13:44:39.257</DateTime>
            <Description />
            <Operation>Register</Operation>
            <TransactionReconRef />
        </TransactionLogLine>
    </History>
    <OrderInformation>
        <Amount>169900</Amount>
        <Currency>NOK</Currency>
        <Fee>0</Fee>
        <OrderDescription>&lt;br /&gt; Bellina Ruffle Kjole Zephyr (36, Svart)&lt;br /&gt;\r\n1699,00&lt;br /&gt;\r\n</OrderDescription>
        <OrderNumber>106</OrderNumber>
        <Timestamp>2017-07-26T13:44:39.257</Timestamp>
        <Total>169900</Total>
        <RoundingAmount>0</RoundingAmount>
    </OrderInformation>
    <SecurityInformation>
        <CustomerIPCountry>NO</CustomerIPCountry>
        <IPCountryMatchesIssuingCountry>true</IPCountryMatchesIssuingCountry>
    </SecurityInformation>
    <Summary>
        <AmountCaptured>0</AmountCaptured>
        <AmountCredited>0</AmountCredited>
        <Annuled>false</Annuled>
        <Annulled>false</Annulled>
        <Authorized>false</Authorized>
    </Summary>
    <TerminalInformation>
        <Browser>Firefox-Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:54.0) Gecko/20100101 Firefox/54.0</Browser>
        <CustomerEntered>2017-07-26T13:44:40.867</CustomerEntered>
        <CustomerRedirected>2017-07-26T13:45:17.977</CustomerRedirected>
    </TerminalInformation>
</PaymentInfo>";
        }
    }
}
