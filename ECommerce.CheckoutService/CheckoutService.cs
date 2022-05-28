using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using ECommerce.CheckoutService.Model;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ECommerce.CheckoutService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CheckoutService : StatefulService, ICheckoutService
    {
        public CheckoutService(StatefulServiceContext context)
            : base(context)
        { }

        public Task<CheckoutSummary> CheckoutAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<CheckoutSummary[]> GetOrderHistoryAsync(string userId)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] {
                new ServiceReplicaListener(ctx => new FabricTransportServiceRemotingListener(ctx, this)),
            };
        }
    }
}
