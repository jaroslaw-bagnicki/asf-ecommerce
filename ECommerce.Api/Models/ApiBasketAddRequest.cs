using System;

namespace ECommerce.Api.Models
{
    public class ApiBasketAddRequest
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}