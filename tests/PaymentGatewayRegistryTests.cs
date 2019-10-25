using Epinova.NetsPaymentGateway;
using StructureMap;
using Xunit;
using Xunit.Abstractions;

namespace Epinova.NetsPaymentGatewayTests
{
    public class PaymentGatewayRegistryTests
    {
        private readonly Container _container;
        private readonly ITestOutputHelper _output;

        public PaymentGatewayRegistryTests(ITestOutputHelper output)
        {
            _output = output;
            _container = new Container(new TestableRegistry());
            _container.Configure(x => { x.AddRegistry(new PaymentGatewayRegistry()); });
        }


        [Fact]
        public void AssertConfigurationIsValid()
        {
            _output.WriteLine(_container.WhatDoIHave());
            _container.AssertConfigurationIsValid();
        }


        [Fact]
        public void Getting_IPaymentGatewayService_ReturnsPaymentGatewayService()
        {
            var instance = _container.GetInstance<IPaymentGatewayService>();

            Assert.IsType<PaymentGatewayService>(instance);
        }
    }
}
