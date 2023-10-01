IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='OrderItem' AND TABLE_SCHEMA='dbo')
BEGIN
	CREATE TABLE [dbo].[OrderItem](
		[Id] [bigint] IDENTITY(1, 1) NOT NULL,
		[ItemId] [bigint] NOT NULL,
		[OrderId] [bigint] NOT NULL,
		[Amount] [int] NOT NULL,
		[DateCreated] [datetime] NOT NULL,
		[DateModified] [datetime] NULL,
		[IsDeleted] [bit] NOT NULL CONSTRAINT DF_OrderItem_IsDeleted DEFAULT 0,
	CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY 
	(
		[OrderId]
	)
	REFERENCES [Order]([Id]),
	CONSTRAINT [FK_OrderItem_Item] FOREIGN KEY 
	(
		[ItemId]
	)
	REFERENCES [Item]([Id]),
	CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
END