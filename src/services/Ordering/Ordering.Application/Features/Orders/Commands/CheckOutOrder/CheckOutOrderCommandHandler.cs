using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckOutOrder
{
    public class CheckOutOrderCommandHandler : IRequestHandler<CheckOutOrderCommand, int>
    {
        private readonly IOrderRepository _repo;
        private readonly IMapper _map;
        private readonly IEmailService _email;
        private readonly ILogger<CheckOutOrderCommandHandler> _log;

        public CheckOutOrderCommandHandler(IOrderRepository repo, IMapper map, IEmailService email, ILogger<CheckOutOrderCommandHandler> log)
        {
            // create new order and send mail

            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<int> Handle(CheckOutOrderCommand request, CancellationToken cancellationToken)
        {
            // convert command into entity obj
            var orderEntity = _map.Map<Order>(request);

            var neworder = await _repo.AddAsync(orderEntity);

            await SendMail(neworder);

            _log.LogInformation($"Order {neworder.Id} is successfulyl created");

            return neworder.Id;
        }

        public async Task SendMail(Order order)
        {
            var email = new Email()
            {
                To = "cephaspeter123@gmail.com",
                Body = $"Order id {order.Id} was created",
                Subject = "Order Created"
            };

            try
            {
                await _email.SendEmail(email);
            }
            catch (Exception ex)
            {
                _log.LogError($"Order {order.Id} failed due to an error with the email service: {ex.Message}");
            }
        }
    }
}
