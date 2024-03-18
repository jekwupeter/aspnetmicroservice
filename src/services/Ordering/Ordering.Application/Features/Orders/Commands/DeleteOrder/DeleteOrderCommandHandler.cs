using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Application.Features.Orders.Commands.CheckOutOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _repo;
        private readonly IMapper _map;
        private readonly ILogger<CheckOutOrderCommandHandler> _log;

        public DeleteOrderCommandHandler(IMapper map, IOrderRepository repo, ILogger<CheckOutOrderCommandHandler> log)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await _repo.GetByIdAsync(request.Id);

            if (orderToDelete == null)
            {
                throw new NotFoundException(nameof(Order), request.Id);
            }

            // map changes to fetch data
            _map.Map(request, orderToDelete, typeof(DeleteOrderCommand), typeof(Order));

            await _repo.DeleteAsync(orderToDelete);

            _log.LogInformation("Order with id - {s} deleted succesfully", request.Id);

        }
    }
}
