using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Application.Features.Orders.Commands.CheckOutOrder;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {

        private readonly IOrderRepository _repo;
        private readonly IMapper _map;
        private readonly ILogger<CheckOutOrderCommandHandler> _log;

        public UpdateOrderCommandHandler(IMapper map, IOrderRepository repo, ILogger<CheckOutOrderCommandHandler> log)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _repo.GetByIdAsync(request.Id);
            
            if (orderToUpdate == null) 
            {
                throw new NotFoundException(nameof(Order), request.Id);

            }
            
            // map changes to fetch data
            _map.Map(request, orderToUpdate, typeof(UpdateOrderCommand), typeof(Order));

            await _repo.UpdateAsync(orderToUpdate);

            _log.LogInformation("Order with id - {s} updated succesfully", request.Id);

        }
    }
}
