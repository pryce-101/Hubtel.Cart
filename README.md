# Hubtel.Cart
**25/6/21 UPDATE**

Minor changes to code base return type/values for endpoint => GetAllCartItems

Added Sentry for the exception handeling.

Sentry Documentation => Sentry.io.

Kindly refer to the png image file named (sentry_hubtel.cart_error_log.png).



**PREVIOUS UPDATE DOCUMENTATION**
Swagger UI Url => https://localhost:44344/swagger/index.html
(port number may vary on your system)

*Kindly alter the connection string in the code base located 
in appsettings.json and appsettings.Development.json.

**Changes**
(Database)
From MSSQL to PostgreSql.
Kindly Check below for the table script to run on your local
postgress database.


**Endpoints Using Entity Framework** 
DeleteItem
GetItemsAfterAdd
GetSingleCartItem
AddItemToCartEF

**Async Task**
GetAllCartItems



****************************************************
             PostgreSQL TABLE SCRIPT
****************************************************
-- public.cartitems definition

-- Drop table

-- DROP TABLE public.cartitems;

CREATE TABLE public.cartitems (
	id int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY,
	itemid int4 NOT NULL,
	itemname varchar(200) NOT NULL,
	quantity int4 NOT NULL,
	unitprice numeric(10,2) NOT NULL,
	phonenumber varchar(15) NOT NULL
);
