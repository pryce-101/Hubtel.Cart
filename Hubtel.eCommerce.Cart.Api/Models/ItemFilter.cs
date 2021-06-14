using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class ItemFilter:GetItemModel
    {
        //public DateTime Time { get; set; }
        [Required( ErrorMessage = "Filter option is Required")]
        public string filter { get; set; }

    }
}
