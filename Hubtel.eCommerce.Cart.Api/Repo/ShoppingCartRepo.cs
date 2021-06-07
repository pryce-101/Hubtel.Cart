using Dapper;
using Hubtel.eCommerce.Cart.Api.Models;
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
        private readonly string FailedMessage = "Adding Item to cart Failed. Try Again.";
        private readonly string DeleteSuccessMessage = "Item Successfully Deleted from Cart. Thank You.";
        private readonly string DeleteFailedMessage = "Item Deletion Failed. Try Again.";
       
        public ShoppingCartRepo(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
          
        }

        //Add item to cart
        public string AddItemToCart(ItemModel item)
        {
            string operationStatus= "";
           
            try
            {
                //formValidate
                if (!String.IsNullOrEmpty(ValidateFormEntry(item)))
                {
                   return operationStatus = ValidateFormEntry(item);
                }
                else
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
            }
            catch (Exception ex)
            {
                operationStatus = FailedMessage;
            }

            return operationStatus;
        }

        //GET single item in cart
        public ItemModel GetCartItem(int itemID, string phoneNumber)
        {            
            ItemModel cartList = null;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var dynamicParams = new DynamicParameters();
                    dynamicParams.Add("@PhoneN", phoneNumber);
                    dynamicParams.Add("@itemID", itemID);
                    cartList = connection.QueryFirstOrDefault<ItemModel>("SELECT * from [Hubtel].[dbo].[CartItems] where PhoneNumber = @PhoneN and ItemID = @itemID",
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
        public IEnumerable<ItemModel> GetAllCartItem(ItemFilter filter)
        {
            IEnumerable<ItemModel> cartList = null;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var dynamicParams = new DynamicParameters();
                    if (filter.filter == "PhoneNumber")
                    {

                        dynamicParams.Add("@PhoneN", filter.PhoneNumber);
                        cartList = connection.Query<ItemModel>("SELECT * from [Hubtel].[dbo].[CartItems] where PhoneNumber = @PhoneN",
                                       dynamicParams,
                                        commandType: CommandType.Text);
                    }
                    else if (filter.filter == "Quantity")
                    {
                        dynamicParams.Add("@Qty", filter.Quantity);
                        cartList = connection.Query<ItemModel>("SELECT * from [Hubtel].[dbo].[CartItems] where Quantity = @Qty",
                                       dynamicParams,
                                        commandType: CommandType.Text);
                    }
                    else if (filter.filter == "Item")
                    {
                        dynamicParams.Add("@Item_Name", filter.ItemName);
                        cartList = connection.Query<ItemModel>("SELECT * from [Hubtel].[dbo].[CartItems] where ItemName = @Item_Name",
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
        public string DeleteCartItem(int itemID, string phoneNumber)
        {
            string operationStatus = "";
            try
            {
                if (!String.IsNullOrEmpty(ValidateDeleteEntry(itemID,phoneNumber)))
                {
                    operationStatus = ValidateDeleteEntry(itemID, phoneNumber);
                }
                else
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand sqlCommand = new SqlCommand("DELETE FROM [Hubtel].[dbo].[CartItems] WHERE ItemID=@ItemID AND PhoneNumber = @PhoneNumber", conn);
                        sqlCommand.Parameters.AddWithValue("@ItemID", itemID);
                        sqlCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        var rowAffected = sqlCommand.ExecuteNonQuery();
                        if (rowAffected > 0)
                        {
                            operationStatus = DeleteSuccessMessage;
                        }
                        else
                        {
                            operationStatus = DeleteFailedMessage;

                        }
                        conn.Close();
                    }               
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return operationStatus;
        }


        //add item form validation
        public string ValidateFormEntry(ItemModel items)
        {
            if (items.ItemID == 0 )
            {
                return "Item ID is Required";
            }
            else if (String.IsNullOrEmpty(items.ItemName))
            {
                return "Item Name is Required";
            }
            else if (items.Quantity == 0)
            {
                return "Item Quantity is Required";

            }
            else if (String.IsNullOrEmpty(items.PhoneNumber))
            {
                return "Phone Number is Required";

            }
            return "";
        }

        //delete item from cart form validation
        public string ValidateDeleteEntry(int itemID, string phoneNumber)
        {
            if (itemID == 0)
            {
                return "Item ID is Required";
            }
            else if (String.IsNullOrEmpty(phoneNumber))
            {
                return "Phone Number is Required";

            }
            return "";
        }

        public string GetSingleItemValidation(int itemID, string phoneNum)
        {    
            if (itemID == 0)
            {
                return "Item ID is Required";
            }
            else if (String.IsNullOrEmpty(phoneNum))
            {
                return "Phone Number is Required";

            }
            return "";
        }
    }
}
