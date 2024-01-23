using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersListQuery
{
    public class GetOrdersListQuery : IRequest<List<OrdersDTO>>
    {
        public string UserName { get; set; }

        public GetOrdersListQuery(string userName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }
}
