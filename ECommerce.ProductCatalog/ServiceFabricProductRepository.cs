﻿using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.ProductCatalog
{
    class ServiceFabricProductRepository : IProductRepository
    {
        private readonly IReliableStateManager _stateManager;

        public ServiceFabricProductRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }
        
        public async Task Add(Product product)
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

            using var tx = _stateManager.CreateTransaction();
            await products.AddOrUpdateAsync(tx, product.Id, product, (id, value) => product);
            await tx.CommitAsync();
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

            using var tx = _stateManager.CreateTransaction();

            var allProducts = await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

            using var enumerator = allProducts.GetAsyncEnumerator();

            var result = new List<Product>();
            while (await enumerator.MoveNextAsync(CancellationToken.None))
            {
                var current = enumerator.Current;
                result.Add(current.Value);
            }

            return result;
        }
    }
}
