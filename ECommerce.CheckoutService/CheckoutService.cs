using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.CheckoutService.Model;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using UserActor.Interfaces;

namespace ECommerce.CheckoutService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CheckoutService : StatefulService, ICheckoutService
    {
        static string CheckoutHistoryKey = "history";
        
        private readonly IProductCatalogService _productCatalogService;

        public CheckoutService(StatefulServiceContext context)
            : base(context)
        {
            var proxyFactory = new ServiceProxyFactory(_ => new FabricTransportServiceRemotingClientFactory());
            _productCatalogService = proxyFactory.CreateServiceProxy<IProductCatalogService>(
                new Uri("fabric:/ECommerce/ECommerce.ProductCatalog"),
                new ServicePartitionKey(0));
        }

        public async Task<CheckoutSummary> CheckoutAsync(string userId)
        {
            var userActor = GetUserActor(userId);
            var basket = await userActor.GetBasket();

            if (basket.Length == 0)
            {
                throw new InvalidOperationException("Basket doesn't contains any products!");
            }

            var summary = new CheckoutSummary();

            foreach(var basketItem in basket)
            {
                var product = await _productCatalogService.GetProductAsync(basketItem.ProductId);
                summary.Products.Add(new CheckoutProduct
                {
                    Product = product,
                    Quantity = basketItem.Quantity,
                    Price = product.Price
                });

            }

            summary.TotalPrice = summary.Products
                .Aggregate<CheckoutProduct, double>(0, (total, product) => total + product.Price * product.Quantity);
            summary.Date = DateTime.UtcNow;

            return summary;
        }

        public async Task<CheckoutSummary[]> GetOrderHistoryAsync(string userId)
        {
            var result = new List<CheckoutSummary>();
            
            var history = await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>(CheckoutHistoryKey);

            using var tx = StateManager.CreateTransaction();
            var historyEnumerable = await history.CreateEnumerableAsync(tx, EnumerationMode.Ordered);
            using var historyEnumerator = historyEnumerable.GetAsyncEnumerator();

            while (await historyEnumerator.MoveNextAsync(default))
            {
                result.Add(historyEnumerator.Current.Value);
            }

            return result.ToArray();
        }

        private IUserActor GetUserActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(new ActorId(userId), new Uri("fabric:/ECommerce/UserActorService"));
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] {
                new ServiceReplicaListener(ctx => new FabricTransportServiceRemotingListener(ctx, this)),
            };
        }
    }
}
