using Epinova.NetsPaymentGateway;
using Xunit;

namespace Epinova.NetsPaymentGatewayTests
{
    public class AmountHelperTests
    {
        [Theory]
        [InlineData(10000, 100)]
        [InlineData(1337, 13.37)]
        [InlineData(40, 0.4)]
        [InlineData(60, 0.6)]
        public void Deflate_Amount_MoveDecimalSeperatorTwoPlacesLeft(int amount, decimal expectedResult)
        {
            decimal result = AmountHelper.Deflate(amount);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(100, "10000")]
        [InlineData(13.37, "1337")]
        [InlineData(0.4, "40")]
        [InlineData(0.6, "60")]
        public void Inflate_Amount_MoveDecimalSeperatorTwoPlacesRight(decimal amount, string expectedResult)
        {
            string result = AmountHelper.Inflate(amount);
            Assert.Equal(expectedResult, result);
        }
    }
}