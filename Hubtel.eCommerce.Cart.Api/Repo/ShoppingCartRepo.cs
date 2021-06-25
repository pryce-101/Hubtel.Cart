using Dapper;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace Hubtel.eCommerce.Cart.Api.Repo
{

    public class ShoppingCartRepo: IShoppingCart
    {
        private readonly string connectionString;
        private readonly string SuccessMessage = "Item Added to Cart Successfully!";
        private readonly string DeleteSuccessMessage = "Item Successfully Deleted from Cart. Thank You.";
      

        private ShoppingCartDBContext _dbContext { get; set; }
        public ShoppingCartRepo(IConfiguration configuration, ShoppingCartDBContext context)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbContext = context;

        }       

        //Add item to cart
        public string AddItemToCart(ItemModel item)
        {
            string operationStatus= "";
           
            try
            {
                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();
                        bool rowsAffected0 = false;
                        NpgsqlCommand fst_Command = new NpgsqlCommand("SELECT * from cartitems WHERE ItemID = @ItemID AND PhoneNumber = @PhoneNumber", conn);
                        fst_Command.Parameters.AddWithValue("@ItemID", item.itemid);
                        fst_Command.Parameters.AddWithValue("@PhoneNumber", item.phonenumber);
                        NpgsqlDataReader reader = fst_Command.ExecuteReader();
                        while (reader.Read())
                        {
                            rowsAffected0 = reader.HasRows;
                        }
                         reader.Close();
                        
                        if (rowsAffected0)
                        {
                            NpgsqlCommand update_Command = new NpgsqlCommand("UPDATE cartitems SET Quantity = Quantity + @Quantity WHERE ItemID = @ItemID AND PhoneNumber = @PhoneNumber ", conn);
                            update_Command.Parameters.AddWithValue("@ItemID", item.itemid);
                            update_Command.Parameters.AddWithValue("@Quantity", item.quantity);
                            update_Command.Parameters.AddWithValue("@PhoneNumber", item.phonenumber);
                            var rowsAffected1 = update_Command.ExecuteNonQuery();
                            operationStatus = SuccessMessage;
                        }
                        else
                        {

                            NpgsqlCommand ngpsqlCommand = new NpgsqlCommand("INSERT INTO cartitems (ItemID,ItemName,Quantity,UnitPrice,PhoneNumber) VALUES (@ItemID,@ItemName,@Quantity,@UnitPrice,@PhoneNumber)", conn);
                            ngpsqlCommand.Parameters.AddWithValue("@ItemID", item.itemid);
                            ngpsqlCommand.Parameters.AddWithValue("@ItemName", item.itemname);
                            ngpsqlCommand.Parameters.AddWithValue("@Quantity", item.quantity);
                            ngpsqlCommand.Parameters.AddWithValue("@UnitPrice", item.unitprice);
                            ngpsqlCommand.Parameters.AddWithValue("@PhoneNumber", item.phonenumber.Trim());
                            var rowsAffected = ngpsqlCommand.ExecuteNonQuery();
                            operationStatus = SuccessMessage;
                        }
                        
                        conn.Close();
                    }

            }
            catch (Exception ex)
            {
                operationStatus = "";
                SentrySdk.CaptureException(ex);
            }

            return operationStatus;
        }


        //Add Item with EF
        public string AddItemToCartEF(ItemModel item)
        {
            int possibleQty = 0;
            string operationStatus = "";

            try
            {
                var existingParent = _dbContext.cartitems.Where(it => it.phonenumber.ToString() == item.phonenumber.ToString()
                                    && Convert.ToInt32(it.itemid) == Convert.ToInt32(item.itemid)).FirstOrDefault();

                if (existingParent != null)
                {
                    possibleQty= existingParent.quantity + item.quantity;

                    _dbContext.cartitems.Attach(existingParent);
                    existingParent.quantity = possibleQty;
                    existingParent.unitprice = item.unitprice;
                   
                    _dbContext.Entry(item).Property(x=> x.id).IsModified=false;

                    _dbContext.SaveChanges();                    
                    operationStatus = SuccessMessage;
                   
                }
                else
                {
                    _dbContext.Add(item);
                    _dbContext.SaveChanges();
                    operationStatus = SuccessMessage;
                }

            }
            catch (Exception ex)
            {
                operationStatus = "";
                SentrySdk.CaptureException(ex);

            }

            return operationStatus;
        }

        //GET single item in cart
        public ItemModel GetCartItem(GetSingleItemModel item)
        {            
            ItemModel cartList = null;
            try
            {
                 cartList = _dbContext.cartitems.Where(it => it.phonenumber.ToString() == item.phonenumber.ToString()
                                    && Convert.ToInt32(it.itemid) == Convert.ToInt32(item.itemid)).FirstOrDefault();

            }
            catch (Exception ex)
            {
                //throw  ex;
                SentrySdk.CaptureException(ex);

            }

            return cartList;
        }

        //GET all cart items
        public async Task<IEnumerable<ItemModel>> GetAllCartItem(ItemFilter filter)
        {
            IEnumerable<ItemModel> cartList =null ;
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var dynamicParams = new DynamicParameters();
                    if (filter.filter == "PhoneNumber")
                    {

                        dynamicParams.Add("@PhoneN", filter.phonenumber);
                        cartList = await connection.QueryAsync<ItemModel>("SELECT * from cartitems where PhoneNumber = @PhoneN",
                                       dynamicParams,
                                        commandType: CommandType.Text);
                    }
                    else if (filter.filter == "Quantity")
                    {
                        dynamicParams.Add("@Qty", filter.quantity);
                        dynamicParams.Add("@Qty", filter.phonenumber);
                        cartList =await  connection.QueryAsync<ItemModel>("SELECT * from cartitems where Quantity = @Qty AND PhoneNumber=@PhoneN",
                                       dynamicParams,
                                        commandType: CommandType.Text);
                    }
                    else if (filter.filter == "Item")
                    {
                        dynamicParams.Add("@Item_Name", filter.itemname);
                        dynamicParams.Add("@Qty", filter.phonenumber);

                        cartList = await connection.QueryAsync<ItemModel>("SELECT * from cartitems where ItemName = @Item_Name AND PhoneNumber=@PhoneN",
                                       dynamicParams,
                                        commandType: CommandType.Text);
                    }                   
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);

            }

            return cartList;
        }

        public IEnumerable<ItemModel> GetAllCartItemWithEF(ItemFilter filter)
        {
            IEnumerable<ItemModel> cartList = null;
            try
            {              
                  
                    if (filter.filter == "PhoneNumber")
                    {
                        cartList = _dbContext.cartitems.Where(it => it.phonenumber == filter.phonenumber);
                    }
                    else if (filter.filter == "Quantity")
                    {
                        cartList =  _dbContext.cartitems.Where(it => it.quantity == filter.quantity);
                    }
                    else if (filter.filter == "Item")
                    {
                        cartList = _dbContext.cartitems.Where(it => it.itemname == filter.itemname);
                    }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);

            }

            return cartList;
        }


        //DELETE cart item
        public string DeleteCartItem(DeleteItemModel item)
        {
            string operationStatus = "";
            try
            {
                var deleteCartItem = _dbContext.cartitems.Where(it => it.itemid == item.itemid
                                   && it.phonenumber == item.phonenumber).FirstOrDefault();
                if (deleteCartItem != null)
                    _dbContext.cartitems.Remove(deleteCartItem);
                    _dbContext.SaveChanges();
                    operationStatus = DeleteSuccessMessage;
               
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);

            }

            return operationStatus;
        }

        //get cart items list after item add
        public IEnumerable<ItemModel> GetItemsAfterAdd(string phoneNumber)
        {
            IEnumerable<ItemModel> rows = null;

            try
            {
                rows = _dbContext.cartitems.OrderByDescending(it => it.id).Where(it => it.phonenumber.ToString() == phoneNumber).ToList();
                

                //using (var connection = new NpgsqlConnection(connectionString))
                //{
                //    var dynamicParams = new DynamicParameters();
                //    dynamicParams.Add("@PhoneNumber", phoneNumber);
                //    rows = connection.Query<ItemModel>("SELECT * FROM cartitems WHERE PhoneNumber = @PhoneNumber Order by Id DESC ",
                //                   dynamicParams, commandType: CommandType.Text);
                //}

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);

            }

            return rows;
           
        }

       

    }
}
