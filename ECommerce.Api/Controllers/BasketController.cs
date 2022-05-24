using ECommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpGet("{userId}")]
        public Task<ApiBasket> GetAsync(string userId)
        {
            throw new NotImplementedException();
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
    }
}
