using AutoMapper;
using StructureMap;

namespace Epinova.NetsPaymentGateway
{
    public class PaymentGatewayRegistry : Registry
    {
        public PaymentGatewayRegistry()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile(new PaymentGatewayMappingProfile()); });
            mapperConfiguration.CompileMappings();

            For<IPaymentGatewayService>().Use<PaymentGatewayService>().Ctor<IMapper>().Is(mapperConfiguration.CreateMapper());
        }
    }
}