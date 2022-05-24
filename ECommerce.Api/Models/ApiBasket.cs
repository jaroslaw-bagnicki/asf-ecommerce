using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Models
{
    public class ApiBasket
    {

        public string UserId { get; set; }

        public ApiBasketItem[] Items { get; set; }
    }

    public class ApiBasketItem
    {
        public string ProductId { get; set; }

        public int Quantinty { get; set; }

    }
}
