using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class ItemModel
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string PhoneNumber { get; set; }
    }
}
