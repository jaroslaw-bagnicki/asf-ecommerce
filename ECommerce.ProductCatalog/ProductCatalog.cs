using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ECommerce.ProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalog : StatefulService, IProductCatalogService
    {
        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        public ServiceFabricProductRepository _productRepository { get; private set; }

        public Task AddProductAsync(Product product)
        {
            return _productRepository.Add(product);
        }

        public async Task<Product[]> GetAllProductAsync()
        {
            return (await _productRepository.GetAll()).ToArray();
        }

        public Task<Product> GetProductAsync(Guid productId)
        {
            return _productRepository.Get(productId);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] {
                new ServiceReplicaListener(ctx => new FabricTransportServiceRemotingListener(ctx, this)),
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _productRepository = new ServiceFabricProductRepository(StateManager);

            var task1 = _productRepository.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = "Dell Monitor",
                Description = "Computer Conitor",
                Price = 500,
                Availability = 100,
            });

            var task2 = _productRepository.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = "Surface Book",
                Description = "Microsoft's Latest Laptop, i7 CPU, 3GB RAM",
                Price = 2200,
                Availability = 15
            });

            var task3 = _productRepository.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = "Arc Touch Mouse",
                Description = "Computer Mouse, bluetooth, reqiures 2 AAA batteries",
                Price = 60,
                Availability = 30,
            });

            await Task.WhenAll(task1, task2, task3);

            var all = await _productRepository.GetAll();
        }
    }
}
