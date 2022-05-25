using System;
using System.Collections.Generic;

namespace ECommerce.Api.Models
{
    public class ApiCheckoutSummary
    {
        public ApiCheckoutProduct[] Products { get; set; }

        public double TotalPrice { get; set; }

        public DateTime Date { get; set; }
    }
}
