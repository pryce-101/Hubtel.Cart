using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class ShoppingCartDBContext : DbContext
    {
        public ShoppingCartDBContext(DbContextOptions<ShoppingCartDBContext> options): base(options)
        {

        }
        public DbSet<ItemModel> cartitems { get; set; }
       
    }
}
