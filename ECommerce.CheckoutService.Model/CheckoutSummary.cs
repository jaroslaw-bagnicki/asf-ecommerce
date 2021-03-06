using System;
using System.Collections.Generic;

namespace ECommerce.CheckoutService.Model
{
    public class CheckoutSummary
    {
        public List<CheckoutProduct> Products { get; set; } = new List<CheckoutProduct>();

        public double TotalPrice { get; set; }

        public DateTime Date { get; set; }
    }
}
