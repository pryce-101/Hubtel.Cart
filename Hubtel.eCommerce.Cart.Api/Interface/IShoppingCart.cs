using Hubtel.eCommerce.Cart.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api
{
   public interface IShoppingCart
    {
        string AddItemToCart(ItemModel item);
        string DeleteCartItem(int itemID, string phoneNumber);
        ItemModel GetCartItem(int itemID, string phoneNumber);
        IEnumerable<ItemModel> GetAllCartItem(ItemFilter filter);
        string GetSingleItemValidation(int itemID, string phoneNum);
    }
}
