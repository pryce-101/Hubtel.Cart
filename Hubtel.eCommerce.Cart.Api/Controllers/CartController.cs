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
        public CartController(IShoppingCart shopCart)
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
        [HttpPost, Route("AddItemToCart")]
        public IActionResult AddItemToCart([FromBody] ItemModel item)
        {
            string response = "";
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Kindly fill in all required fields"
                });
            }
            else
            {
                response = _shoppingCart.AddItemToCart(item);

                if (response != String.Empty)
                {
                    //return success and added items                  
                    return Ok(new
                    {
                        status = true,
                        message = response,
                        data = GetItemsAfterAdd(item.phonenumber)
                    });
                }
                else
                {
                    //return if addition failed
                    return BadRequest(new {
                        status=false,
                        message = "Adding Item to cart failed. Try Again."                    
                    });
                }

            }
        }

        // Delete item
        [HttpDelete, Route("DeleteItem")]
        public IActionResult DeleteItem(DeleteItemModel item)
        {
            string response = "";
            if (ModelState.IsValid)
            {
                 response = _shoppingCart.DeleteCartItem(item);
                if (response != String.Empty)
                {
                    return Ok(new
                    {
                        status = true,
                        message = response
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Deletion failed. Try Again"
                    });
                }
            }
            else
            {
                return BadRequest(new
                {
                    status = false,
                    Message= "Kindly provide required fields"
                });
            }
            
        }


        //GET all cart items (list)
        [HttpGet, Route("GetAllCartItems")]
        public async Task<IEnumerable<ItemModel>> GetAllCartItems([FromQuery]ItemFilter item)
        {
            IEnumerable<ItemModel> allCartItems = null;

            if (!ModelState.IsValid)
            {
                BadRequest(new {
                    success=false,
                    message= "Kindly provide requested fields"
                });
            }
            else
            {
                allCartItems = await _shoppingCart.GetAllCartItem(item);
               
            }

            return  allCartItems;
        }



        //GET single Cart Item
        [HttpGet, Route("GetSingleCartItem")]
        public IActionResult GetSingleCartItem([FromQuery] GetSingleItemModel item)
        {
            ItemModel cartItem = new ItemModel();

            if (ModelState.IsValid)
            {
                cartItem = _shoppingCart.GetCartItem(item);
            }
            else
            {
                return BadRequest(new {
                    status = false,
                    message = "Kindly provide all Required fields"
                });
            }

            if (cartItem == null)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Item not found in cart. Check item details",
                    data = cartItem
                });
            }
            else
            {
                return Ok(new
                {
                    status = true,
                    message = "Cart Item found Successfully.",
                    data = cartItem
                });
            }

           
        }

        [HttpPost]
        [Route("GetItemsAfterAdd")]
        public IEnumerable<ItemModel> GetItemsAfterAdd(string phoneNum) {
            IEnumerable<ItemModel> items = null;
            items = _shoppingCart.GetItemsAfterAdd(phoneNum);

            return items;
        }


        [HttpPost, Route("AddItemToCartEF")]
        public IActionResult AddItemToCartEF([FromBody] ItemModel item)
        {
            string response = "";
            if (!ModelState.IsValid)
            {
                return BadRequest(new {
                    status=false,
                    message= "Kindly fill in required fields",
                });
            }
            else
            {
                response = _shoppingCart.AddItemToCartEF(item);

                if (response != String.Empty)
                {
                    //return success and added items
                    return Ok(new
                    {
                        status = true,
                        message = response ,
                        data = GetItemsAfterAdd(item.phonenumber)
                    });
                }
                else
                {
                    return BadRequest(new {
                        status=false,
                        data=response
                    });
                }

            }
        }


    }
              
}
