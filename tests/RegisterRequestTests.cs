using Epinova.NetsPaymentGateway;
using Xunit;

namespace Epinova.NetsPaymentGatewayTests
{
    public class RegisterRequestTests
    {
        public static RegisterRequest GetValidRequest()
        {
            return new RegisterRequest
            {
                Amount = Factory.GetInteger(),
                CurrencyCode = "NOK",
                OrderDescription = Factory.GetString(),
                OrderNumber = Factory.GetString(),
                RedirectUrl = Factory.GetUri()
            };
        }

        [Fact]
        public void Ctor_PaymentMethods_IsNotNull()
        {
            var request = new RegisterRequest();
            Assert.NotNull(request.PaymentMethods);
        }


        [Fact]
        public void IsValid_AllPropertiesSet_ReturnsTrue()
        {
            RegisterRequest request = GetValidRequest();

            Assert.True(request.IsValid());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_AmountMissing_ReturnsFalse(decimal amount)
        {
            RegisterRequest request = GetValidRequest();
            request.Amount = amount;

            Assert.False(request.IsValid());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_CurrencyCodeMissing_ReturnsFalse(string currencyCode)
        {
            RegisterRequest request = GetValidRequest();
            request.CurrencyCode = currencyCode;

            Assert.False(request.IsValid());
        }

        [Fact]
        public void IsValid_NoPropertiesSet_ReturnsFalse()
        {
            var request = new RegisterRequest();

            Assert.False(request.IsValid());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_OrderDescriptionMissing_ReturnsFalse(string orderDescription)
        {
            RegisterRequest request = GetValidRequest();
            request.OrderDescription = orderDescription;

            Assert.False(request.IsValid());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_OrderNumberMissing_ReturnsFalse(string orderNumber)
        {
            RegisterRequest request = GetValidRequest();
            request.OrderNumber = orderNumber;

            Assert.False(request.IsValid());
        }

        [Fact]
        public void IsValid_RedirectUrlMissing_ReturnsFalse()
        {
            RegisterRequest request = GetValidRequest();
            request.RedirectUrl = null;

            Assert.False(request.IsValid());
        }
    }
}