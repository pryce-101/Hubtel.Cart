using Dapper;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Repo
{

    public class ShoppingCartRepo: IShoppingCart
    {
        private readonly string connectionString;
        private readonly string SuccessMessage = "Item Added to Cart Successfully!";
        private readonly string DeleteSuccessMessage = "Item Successfully Deleted from Cart. Thank You.";
       // private readonly string FailedMessage = "Adding Item to cart Failed. Try Again.";
       // private readonly string DeleteFailedMessage = "Item Deletion Failed. Try Again.";

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
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        bool rowsAffected0 = false;
                        SqlCommand fst_sqlCommand = new SqlCommand("SELECT Top 1 * from [Hubtel].[dbo].[CartItems] WHERE PhoneNumber = @PhoneNumber AND ItemID = @ItemID ", conn);
                        fst_sqlCommand.Parameters.AddWithValue("@ItemID", item.ItemID);
                        fst_sqlCommand.Parameters.AddWithValue("@PhoneNumber", item.PhoneNumber);
                        SqlDataReader reader = fst_sqlCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            rowsAffected0 = reader.HasRows;
                        }
                         reader.Close();
                        
                        if (rowsAffected0)
                        {                            
                            SqlCommand update_sqlCommand = new SqlCommand("UPDATE [Hubtel].[dbo].[CartItems] SET Quantity = Quantity + @Quantity WHERE ItemID = @ItemID AND PhoneNumber = @PhoneNumber ", conn);
                            update_sqlCommand.Parameters.AddWithValue("@ItemID", item.ItemID);
                            update_sqlCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            update_sqlCommand.Parameters.AddWithValue("@PhoneNumber", item.PhoneNumber);
                            var rowsAffected1 = update_sqlCommand.ExecuteNonQuery();
                            operationStatus = SuccessMessage;
                        }
                        else
                        {
                            // (ItemID,ItemName,Quantity,UnitPrice,PhoneNumber) 
                            SqlCommand sqlCommand = new SqlCommand("INSERT INTO [Hubtel].[dbo].[CartItems] VALUES (@ItemID,@ItemName,@Quantity,@UnitPrice,@PhoneNumber)", conn);
                            sqlCommand.Parameters.AddWithValue("@ItemID", item.ItemID);
                            sqlCommand.Parameters.AddWithValue("@ItemName", item.ItemName);
                            sqlCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            sqlCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                            sqlCommand.Parameters.AddWithValue("@PhoneNumber", item.PhoneNumber.TrimEnd());
                            var rowsAffected = sqlCommand.ExecuteNonQuery();
                            operationStatus = SuccessMessage;
                        }
                        
                        conn.Close();
                    }

            }
            catch (Exception ex)
            {
                operationStatus = "";
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
                var existingParent = _dbContext.CartItems.Where(it => item.PhoneNumber.ToString() == item.PhoneNumber.ToString()
                                    && Convert.ToInt32(it.ItemID) == Convert.ToInt32(item.ItemID)).FirstOrDefault();

                if (existingParent != null)
                {
                    possibleQty= existingParent.Quantity + item.Quantity;

                    _dbContext.CartItems.Attach(existingParent);
                    existingParent.Quantity = possibleQty;
                    existingParent.UnitPrice = item.UnitPrice;
                   
                    _dbContext.Entry(item).Property(x=> x.Id).IsModified=false;

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
            }

            return operationStatus;
        }

        //GET single item in cart
        public ItemModel GetCartItem(GetSingleItemModel item)
        {            
            ItemModel cartList = null;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                   
                    var dynamicParams = new DynamicParameters();
                    dynamicParams.Add("@PhoneN", item.PhoneNumber);
                    dynamicParams.Add("@ItemID", item.ItemID);
                    cartList = connection.QueryFirstOrDefault<ItemModel>("SELECT * from [Hubtel].[dbo].[CartItems] where PhoneNumber = @PhoneN and ItemID = @ItemID",
                                   dynamicParams, commandType: CommandType.Text);
                }

            }
            catch (Exception ex)
            {
                throw  ex;
            }
            
            return cartList;
        }

        //GET all cart items
        public async Task<IEnumerable<ItemModel>> GetAllCartItem(ItemFilter filter)
        {
            IEnumerable<ItemModel> cartList = null;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var dynamicParams = new DynamicParameters();
                    if (filter.filter == "PhoneNumber")
                    {

                        dynamicParams.Add("@PhoneN", filter.PhoneNumber);
                        cartList = await connection.QueryAsync<ItemModel>("SELECT * from [Hubtel].[dbo].[CartItems] where PhoneNumber = @PhoneN",
                                       dynamicParams,
                                        commandType: CommandType.Text);
                    }
                    else if (filter.filter == "Quantity")
                    {
                        dynamicParams.Add("@Qty", filter.Quantity);
                        cartList =await  connection.QueryAsync<ItemModel>("SELECT * from [Hubtel].[dbo].[CartItems] where Quantity = @Qty",
                                       dynamicParams,
                                        commandType: CommandType.Text);
                    }
                    else if (filter.filter == "Item")
                    {
                        dynamicParams.Add("@Item_Name", filter.ItemName);
                        cartList = await connection.QueryAsync<ItemModel>("SELECT * from [Hubtel].[dbo].[CartItems] where ItemName = @Item_Name",
                                       dynamicParams,
                                        commandType: CommandType.Text);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cartList;
        }


        //DELETE cart item
        public string DeleteCartItem(DeleteItemModel item)
        {
            string operationStatus = "";
            try
            {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand sqlCommand = new SqlCommand("DELETE FROM [Hubtel].[dbo].[CartItems] WHERE ItemID=@ItemID AND PhoneNumber = @PhoneNumber", conn);
                        sqlCommand.Parameters.AddWithValue("@ItemID", item.ItemID);
                        sqlCommand.Parameters.AddWithValue("@PhoneNumber", item.PhoneNumber);
                        var rowAffected = sqlCommand.ExecuteNonQuery();
                        if (rowAffected > 0)
                        {
                            operationStatus = DeleteSuccessMessage;
                        }
                        else
                        {                           
                            operationStatus = "";
                        }
                        conn.Close();
                    }               
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return operationStatus;
        }

        //get cart items list after item add
        public IEnumerable<ItemModel> GetItemsAfterAdd(string phoneNumber)
        {
            IEnumerable<ItemModel> rows = null;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var dynamicParams = new DynamicParameters();
                    dynamicParams.Add("@PhoneNumber", phoneNumber);
                    rows = connection.Query<ItemModel>("SELECT * FROM [Hubtel].[dbo].[CartItems] WHERE PhoneNumber = @PhoneNumber Order by Id DESC ",
                                   dynamicParams, commandType: CommandType.Text);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rows;
           
        }

    }
}
