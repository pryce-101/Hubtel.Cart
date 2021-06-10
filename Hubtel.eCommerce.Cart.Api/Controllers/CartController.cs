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
    public class CartController : ControllerBase
    {
        private readonly IShoppingCart _shoppingCart;
        public CartController( IShoppingCart shopCart)
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
            string response = "";
            if (!String.IsNullOrEmpty(ValidateModel()))
            {
                return BadRequest(ValidateModel());                
            }
            else
            {
                response = _shoppingCart.AddItemToCart(item);

                if (response != String.Empty)
                {
                   //return success and added items
                  return  Ok(new
                    {
                        status = true,
                        code=200,
                        message = response,
                        cartData = GetItemsAfterAdd(item.PhoneNumber)
                    });
                }                                    
                else
                {                  
                    return BadRequest(response);
                }

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
            IEnumerable<ItemModel> allCartItems = null;

            if (!String.IsNullOrEmpty(ValidateModel()))
            {
                BadRequest();
            }
            else
            {
                allCartItems = _shoppingCart.GetAllCartItem(item);
            }
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
                    code = 400,
                    status = false,
                    response = _shoppingCart.GetSingleItemValidation(itemID, phoneNumber)
                });
            }
           
            return Ok(new
            {
                code = 200,
                status = true,
                response = cartItem
            });
        }

        [HttpPost]
        [Route("GetItemsAfterAdd")]
        public IEnumerable<ItemModel> GetItemsAfterAdd(string phoneNum) {
            IEnumerable<ItemModel> items = null;
            items = _shoppingCart.GetItemsAfterAdd(phoneNum);

            return items;
        }




        public string ValidateModel()
        {
            string message = string.Join(";", ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)           
                );
            return message;

        }


    }
}
