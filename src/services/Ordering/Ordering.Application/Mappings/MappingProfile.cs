using AutoMapper;
using Ordering.Application.Features.Orders.Commands.CheckOutOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrdersListQuery;
using Ordering.Domain.Entities;

namespace Ordering.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrdersDTO>().ReverseMap();
            CreateMap<Order, CheckOutOrderCommand>().ReverseMap();
            CreateMap<Order, UpdateOrderCommand>().ReverseMap();
        }
    }
}
