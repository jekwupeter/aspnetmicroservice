using Basket.API.Entities;

namespace Basket.API.Repositories
{
    public interface IBasket
    {
        Task<ShoppingCart> GetBasket(string userName);
        Task<ShoppingCart> UpdateBasket(ShoppingCart basket);
        Task RemoveBasket(string userName);
    }
}
