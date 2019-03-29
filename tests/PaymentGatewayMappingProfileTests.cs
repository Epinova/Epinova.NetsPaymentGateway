using AutoMapper;
using Epinova.NetsPaymentGateway;
using Xunit;

namespace Epinova.NetsPaymentGatewayTests
{
    public class PaymentGatewayMappingProfileTests
    {
        private readonly MapperConfiguration _config;
        private readonly IMapper _mapper;

        public PaymentGatewayMappingProfileTests()
        {
            _config = new MapperConfiguration(cfg => { cfg.AddProfile<PaymentGatewayMappingProfile>(); });
            _mapper = _config.CreateMapper();
        }

        [Fact]
        public void AllowNullCollections_IsFalse()
        {
            var profile = new PaymentGatewayMappingProfile();

            Assert.False(profile.AllowNullCollections);
        }

        [Fact]
        public void AutomapperConfiguration_IsValid()
        {
            _config.AssertConfigurationIsValid();
        }


        [Fact]
        public void Map_PaymentInformation_CapturedAmountIsMappedCorrectly()
        {
            int capturedAmount = Factory.GetInteger();
            var src = new QueryResponseDto { Summary = new QuerySummaryDto { AmountCaptured = capturedAmount } };

            var dest = _mapper.Map<PaymentInformation>(src);
            Assert.Equal(AmountHelper.Deflate(capturedAmount), dest.CapturedAmount);
        }

        [Fact]
        public void Map_PaymentInformation_CapturedAmountIsZeroForEmptyDto()
        {
            var src = new QueryResponseDto();

            var dest = _mapper.Map<PaymentInformation>(src);
            Assert.Equal(0, dest.CapturedAmount);
        }

        [Fact]
        public void Map_PaymentInformation_IsNullIfDtoIsNull()
        {
            var dest = _mapper.Map<PaymentInformation>(null);
            Assert.Null(dest);
        }

        [Fact]
        public void Map_RegisterResponseDto_CorrectTransactionId()
        {
            var src = new RegisterResponseDto() { TransactionId = Factory.GetString() };

            var dest = _mapper.Map<RegisterResponse>(src);

            Assert.Equal(src.TransactionId, dest.TransactionId);
        }
    }
}