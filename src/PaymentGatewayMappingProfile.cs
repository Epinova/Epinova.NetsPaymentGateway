using AutoMapper;

namespace Epinova.NetsPaymentGateway
{
    internal class PaymentGatewayMappingProfile : Profile
    {
        public PaymentGatewayMappingProfile()
        {
            AllowNullCollections = false;

            CreateMap<RegisterResponseDto, RegisterResponse>()
                .ForMember(dest => dest.TerminalUrl, opt => opt.Ignore());

            CreateMap<QueryResponseDto, PaymentInformation>()
                .ForMember(dest => dest.AuthorizationId, opt => opt.MapFrom(src => src.Summary.AuthorizationId))
                .ForMember(dest => dest.CapturedAmount, opt => opt.MapFrom(src => AmountHelper.Deflate(src.Summary.AmountCaptured)))
                .ForMember(dest => dest.CreditedAmount, opt => opt.MapFrom(src => AmountHelper.Deflate(src.Summary.AmountCredited)))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.OrderInformation.Currency))
                .ForMember(dest => dest.FeeAmount, opt => opt.MapFrom(src => AmountHelper.Deflate(src.OrderInformation.Fee)))
                .ForMember(dest => dest.IsAuthorized, opt => opt.MapFrom(src => src.Summary.Authorized))
                .ForMember(dest => dest.IsCancelled, opt => opt.MapFrom(src => src.Summary.Annulled))
                .ForMember(dest => dest.OrderAmount, opt => opt.MapFrom(src => AmountHelper.Deflate(src.OrderInformation.Amount)))
                .ForMember(dest => dest.OrderDescription, opt => opt.MapFrom(src => src.OrderInformation.OrderDescription))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderInformation.OrderNumber))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.CardInformation.PaymentMethod))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.OrderInformation.Timestamp))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => AmountHelper.Deflate(src.OrderInformation.Total)));
        }
    }
}
