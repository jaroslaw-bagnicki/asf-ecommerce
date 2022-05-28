using System;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Api.Models;
using ECommerce.CheckoutService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private static readonly Random _random = new Random(DateTime.UtcNow.Second);

        public CheckoutController()
        {
        }


        [Route("{userId}")]
        public async Task<ApiCheckoutSummary> CheckoutAsync(string userId)
        {
            var summary = await GetCheckoutService().CheckoutAsync(userId);

            return MapToApiCheckoutSummary(summary);
        }

        [Route("history/{userId}")]
        public async Task<ApiCheckoutSummary[]> GetHistoryAsyc(string userId)
        {
            var history = await GetCheckoutService().GetOrderHistoryAsync(userId);

            return history.Select(MapToApiCheckoutSummary).ToArray();
        }

        private ApiCheckoutSummary MapToApiCheckoutSummary(CheckoutSummary summary)
        {
            return new ApiCheckoutSummary
            {
                Products = summary.Products.Select(p => new ApiCheckoutProduct
                {
                    ProductId = p.Product.Id,
                    ProductName = p.Product.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                }).ToArray(),
                TotalPrice = summary.TotalPrice,
                Date = summary.Date
            };
        }

        private ICheckoutService GetCheckoutService()
        {
            long key = LongRandom();
            
            var proxyFactory = new ServiceProxyFactory(_ => new FabricTransportServiceRemotingClientFactory());
            var service = proxyFactory.CreateServiceProxy<ICheckoutService>(
                new Uri("fabric:/ECommerce/ECommerce.CheckoutService"),
                new ServicePartitionKey(key));

            return service;
        }

        private long LongRandom()
        {
            byte[] buf = new byte[8];
            _random.NextBytes(buf);
            return BitConverter.ToInt64(buf);
        }
    }
}
