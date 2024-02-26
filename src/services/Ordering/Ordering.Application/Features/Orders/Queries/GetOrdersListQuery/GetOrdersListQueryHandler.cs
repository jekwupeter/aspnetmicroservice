using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersListQuery
{
    public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrdersDTO>>
    {
        private readonly IOrderRepository _repo;
        private readonly IMapper _map;

        public GetOrdersListQueryHandler(IOrderRepository repo, IMapper map)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public async Task<List<OrdersDTO>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
        {
            var orders = await _repo.GetOrdersByUserName(request.UserName);

            var dtos = _map.Map<List<OrdersDTO>>(orders);
            return dtos;
        }
    }
}
