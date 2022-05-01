using ECommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Task<IEnumerable<ApiProduct>> Get()
        {
            var result = new[]
            {
                new ApiProduct
                {
                    Id = Guid.NewGuid(),
                    Description = "fake"
                }
            };

            return Task.FromResult<IEnumerable<ApiProduct>>(result);
        }

        [HttpPost]
        public Task Create()
        {
            return Task.CompletedTask;
        }
    }
}
