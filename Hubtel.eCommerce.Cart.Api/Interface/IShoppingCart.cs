using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api
{
   public interface IShoppingCart
    {
        string AddItemToCart(ItemModel item);
        string AddItemToCartEF(ItemModel item);
        string DeleteCartItem(DeleteItemModel item);
        ItemModel GetCartItem(GetSingleItemModel item);
        Task<IEnumerable<ItemModel>> GetAllCartItem(ItemFilter filter);
        IEnumerable<ItemModel> GetItemsAfterAdd(string phone); 
    }
}
