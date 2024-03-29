﻿using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext db): base(db)
        {}
        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            return await _db.Orders
                .Where(o => o.UserName == userName)
                .ToListAsync();
        }
    }
}
