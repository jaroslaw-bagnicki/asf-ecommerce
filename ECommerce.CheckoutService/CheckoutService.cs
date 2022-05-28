using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.CheckoutService.Model;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
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

            var result = new CheckoutSummary();

            foreach(var basketItem in basket)
            {
                var product = await _productCatalogService.GetProductAsync(basketItem.ProductId);
                result.Products.Add(new CheckoutProduct
                {
                    Product = product,
                    Quantity = basketItem.Quantity,
                    Price = product.Price * basketItem.Quantity
                });

            }

            result.Date = DateTime.UtcNow;

            return result;
        }

        public Task<CheckoutSummary[]> GetOrderHistoryAsync(string userId)
        {
            throw new NotImplementedException();
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