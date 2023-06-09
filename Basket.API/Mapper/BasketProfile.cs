using Basket.API.Entities;
using EventBus.Messages.Events;
using AutoMapper;

namespace Basket.API.Mapper
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
        }

    }
}
