using System;

namespace ECommerce.Api.Models
{
    public class ApiCheckoutProduct
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }
    }
}
