using ECommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserActor.Interfaces;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<ApiBasket> GetAsync(string userId)
        {
            var actor = GetActor(userId);

            var products = await actor.GetBasket();

            return new ApiBasket
            {
                UserId = userId,
                Items = products.Select(
                    p => new ApiBasketItem
                    {
                        ProductId = p.Key.ToString(),
                        Quantinty = p.Value
                    }).ToArray()
            };
        }

        [HttpPost("userId")]
        public Task AddAsync(string userId, [FromBody] ApiBasketAddRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("userId")]
        public Task DeleteAsyc(string userId)
        {
            throw new NotImplementedException();
        }

        private IUserActor GetActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(new ActorId(userId), new Uri("fabric:/ECommnerce/UserActorService"));
        }
    }
}
