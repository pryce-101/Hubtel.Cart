using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class ItemModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "ItemID is Required")]
        [Range(1,1000000, ErrorMessage ="Item ID cannot be Zero.")]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Item Name is Required")]
        public string ItemName { get; set; }

        [Range(1, 10000, ErrorMessage = "Quantity cannot be Zero.")]
        [Required(ErrorMessage = "Quantity is Required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price is Required")]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d+\.\d{0,2}$", ErrorMessage = "Unit Price amount requires 2 decimal places.")]
        [Range(0, 999999999999.99)]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Phone Number is Required")]
        [MinLength(10, ErrorMessage = "Phone Number cannot be less or more than 10 degits.")]
        [DataType(DataType.PhoneNumber, ErrorMessage ="Phone number format has to be a string" )]
        [RegularExpression(@"([0-9]+)", ErrorMessage = "Phone Number must all be numbers.")]
        public string PhoneNumber { get; set; }
    }

    public class DeleteItemModel {
        [Required(ErrorMessage = "ItemID is Required")]
        [Range(1, 1000000, ErrorMessage = "Item ID cannot be Zero.")]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Phone Number is Required")]
        [MinLength(10, ErrorMessage = "Phone Number cannot be less or more than 10 degits.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Phone number format has to be a string")]
        [RegularExpression(@"([0-9]+)", ErrorMessage = "Phone Number must all be numbers.")]
        public string PhoneNumber { get; set; }
    }

    public class GetItemModel
    {
       
        [Required(ErrorMessage = "ItemID is Required")]
        [Range(1, 1000000, ErrorMessage = "Item ID cannot be Zero.")]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Item Name is Required")]
        public string ItemName { get; set; }

        [Range(1, 10000, ErrorMessage = "Quantity cannot be Zero.")]
        [Required(ErrorMessage = "Quantity is Required")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Phone Number is Required")]
        [MinLength(10, ErrorMessage = "Phone Number cannot be less or more than 10 degits.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Phone number format has to be a string")]
        [RegularExpression(@"([0-9]+)", ErrorMessage = "Phone Number must all be numbers.")]
        public string PhoneNumber { get; set; }
    }

    public class GetSingleItemModel
    {
        [Required(ErrorMessage = "ItemID is Required")]
        [Range(1, 1000000, ErrorMessage = "Item ID cannot be Zero.")]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Phone Number is Required")]
        [MinLength(10, ErrorMessage = "Phone Number cannot be less or more than 10 degits.")]
        //[DataType(DataType.PhoneNumber, ErrorMessage = "Phone number format has to be a string")]
        [RegularExpression(@"([0-9]+)", ErrorMessage = "Phone Number must all be numbers.")]
        public string PhoneNumber { get; set; }
    }




}
