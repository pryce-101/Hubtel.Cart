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
                    return Ok(new
                    {
                        status = true,
                        code = 200,
                        message = response,
                        cartData = GetItemsAfterAdd(item.PhoneNumber)
                    });
                }
                else
                {
                    return BadRequest(new {
                        status=false,
                        code=400,
                        message = response                    
                    });
                }

            }
        }

        // Delete item
        [HttpDelete, Route("DeleteItem")]
        public IActionResult DeleteItem(DeleteItemModel item)
        {
            string response = "";
            if (String.IsNullOrEmpty(ValidateModel()))
            {
                 response = _shoppingCart.DeleteCartItem(item);
                if (response != String.Empty)
                {
                    return  Ok(new
                    {
                        status = true,
                        code = 200,
                        Message = response
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        status = false,
                        code = 400,
                        Message = response
                    });
                }

            }
            else
            {
             return   BadRequest(new
                {
                    status = false,
                    code=400,
                    Message= "Deletion Failed"
                });
            }

            //return response;
        }


        //GET all cart items (list)
        [HttpGet("filter"), Route("GetAllCartItems")]
        public async Task<IEnumerable<ItemModel>> GetAllCartItem([FromQuery]ItemFilter item)
        {
            IEnumerable<ItemModel> allCartItems = null;

            if (!String.IsNullOrEmpty(ValidateModel()))
            {
                BadRequest("Kindly provide requested fields");
            }
            else
            {
                allCartItems = await _shoppingCart.GetAllCartItem(item);
            }
            return allCartItems;

        }



        //GET single Cart Item
        [HttpGet, Route("GetSingleCartItem")]
        public IActionResult GetSingleCartItem([FromQuery] GetSingleItemModel item)
        {
            ItemModel cartItem = new ItemModel();

            if (String.IsNullOrEmpty(ValidateModel()))
            {
                cartItem = _shoppingCart.GetCartItem(item);
            }
            else
            {
                return BadRequest(new {
                    code = 400,
                    status = false,
                    response = ValidateModel()
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


        [HttpPost, Route("AddItemToCartEF")]
        public IActionResult AddItemToCartEF([FromBody] ItemModel item)
        {
            string response = "";
            if (!String.IsNullOrEmpty(ValidateModel()))
            {
                return BadRequest(ValidateModel());
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
                        code = 200,
                        message = response ,
                        cartData = GetItemsAfterAdd(item.PhoneNumber)
                    });
                }
                else
                {
                    return BadRequest(new {
                        status=false,
                        code=400,message=response
                    });
                }

            }
        }



        public string ValidateModel()
        {
            string message = string.Join(";", ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage)
                );
            return message;

        }

        //public string ValidateDeleteModel()
        //{
        //    string message = string.Join(";", ModelState.Values
        //        .SelectMany(x => x.Errors)
        //        .Select(x => x.ErrorMessage)
        //        );
        //    return message;

        //}
        //public string ValidateGetSingleItemModel()
        //{
        //    string message = string.Join(";", ModelState.Values
        //        .SelectMany(x => x.Errors)
        //        .Select(x => x.ErrorMessage)
        //        );
        //    return message;

        //}    


    }
              
    }
