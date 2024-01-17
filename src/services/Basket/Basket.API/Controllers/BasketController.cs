using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasket _repo;
        private readonly ILogger _log;
        private readonly DiscountGrpcService _discountGrpc;

        public BasketController(IBasket repo, ILogger<BasketController> log, DiscountGrpcService discountGrpc)
        {
            _log = log;
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _discountGrpc = discountGrpc ?? throw new ArgumentNullException(nameof(discountGrpc));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(IEnumerable<ShoppingCart>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetProduct(string userName)
        {
            var basket = await _repo.GetBasket(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<ShoppingCart>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            foreach (var item in basket.Items)
            {
                var coupon = await _discountGrpc.GetDiscount(item.ProductName);

                item.Price -= coupon.Amount;
            }

            var response = await _repo.UpdateBasket(basket);

            return Ok(response);
        }

        [HttpDelete("{userName}", Name = "Deletess")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await _repo.RemoveBasket(userName);

            return Ok();
        }

    }
}
