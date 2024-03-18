using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> log)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.AddRange(GetPreConfiguredOrder());
                await orderContext.SaveChangesAsync();
                log.LogInformation("Seed db associated with context{ctx}", typeof(OrderContext).Name);
            }

            
        }
        private static IEnumerable<Order> GetPreConfiguredOrder()
        {
            return new List<Order>
                {
                    new Order () {
                    UserName = "adm",
                    FirstName = "Cephas",
                    LastName = "Peter",
                    EmailAddress = "test@gmail.com",
                    AddressLine = "Lagos",
                    Country = "Nigeria",
                    TotalPrice = 40,
                    CCV = ""
                    }
                };
        }
    }
}
