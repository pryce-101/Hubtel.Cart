using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
     
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IShoppingCart _shoppingCart;
        public ValuesController( IShoppingCart shopCart)
        {
            _shoppingCart = shopCart;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Ready!", "Lets run some codes!" };
        }

        //Add item to cart
        [HttpPost,Route("AddItemToCart")]
        public ActionResult<string> AddItemToCart([FromBody] ItemModel item)
        {
            string response = _shoppingCart.AddItemToCart(item);

            if (response != String.Empty)
            {
                 return response;
            }
            else
            {
                return response;
            }
        }

        // Delete item
        [HttpDelete, Route("DeleteItem")]
        public ActionResult<string> DeleteItem(ItemModel item)
        {
            string response = _shoppingCart.DeleteCartItem(item.ItemID,item.PhoneNumber);

            if (response != String.Empty)
            {
                return response;
            }
            else
            {
                return response;
            }
        }

       
        //GET all cart items (list)
        [HttpGet("filter"), Route("GetAllCartItems")]
        public IEnumerable<ItemModel> GetAllCartItem([FromQuery]ItemFilter item)
        {
            IEnumerable<ItemModel> allCartItems;//
            allCartItems = _shoppingCart.GetAllCartItem(item);
            return allCartItems;
        }



        //GET single Cart Item
        [HttpGet, Route("GetSingleCartItem")]
        public IActionResult GetSingleCartItem(int itemID, string phoneNumber)
        {
            ItemModel cartItem = new ItemModel();

            if (String.IsNullOrEmpty(_shoppingCart.GetSingleItemValidation(itemID,phoneNumber)))
            {
               cartItem = _shoppingCart.GetCartItem(itemID, phoneNumber);
            }
            else
            {
               return BadRequest(new {
                    status = false,
                    response = _shoppingCart.GetSingleItemValidation(itemID, phoneNumber)
                });
            }
           
            return Ok(new
            {
                status = true,
                response = cartItem
            });
        }


    }
}
