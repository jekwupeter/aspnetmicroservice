using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasket
    {
        private readonly IDistributedCache _cache;
        public BasketRepository(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basketStr = await _cache.GetStringAsync(userName);
            if (string.IsNullOrWhiteSpace(basketStr)) return null;
            return JsonConvert.DeserializeObject<ShoppingCart>(basketStr);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _cache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasket(basket.UserName);
        }

        public async Task RemoveBasket(string userName)
        {
            await _cache.RemoveAsync(userName);
        }
    }
}
