using EventBus.Messages.Events;
using Ordering.Application.Features.Commands.CheckoutOrder;
using AutoMapper;

namespace Ordering.API.Mapper
{
    public class OrderingProfile : Profile
    {
        public OrderingProfile()
        {
            CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
