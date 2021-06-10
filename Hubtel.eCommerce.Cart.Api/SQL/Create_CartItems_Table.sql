USE [Hubtel]
GO

/****** Object:  Table [dbo].[CartItems]    Script Date: 6/8/2021 6:26:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CartItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[ItemName] [varchar](200) NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
	[PhoneNumber] [nchar](15) NOT NULL
	
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


