IF (EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Order' AND TABLE_SCHEMA='dbo') 
AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='OrderRequest' AND TABLE_SCHEMA='dbo'))
BEGIN
	EXEC sp_rename 'Order', 'OrderRequest'
	EXEC sp_rename 'dbo.PK_Order', 'PK_OrderRequest'
	EXEC sp_rename 'dbo.FK_Order_Item', 'FK_OrderRequest_Item'
	EXEC sp_rename 'dbo.FK_Order_User', 'FK_OrderRequest_User'
END

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Order' AND TABLE_SCHEMA='dbo')
BEGIN
	CREATE TABLE [dbo].[Order](
		[Id] [bigint] IDENTITY(1, 1) NOT NULL,
		[State] [int] NULL,
		[DateCreated] [datetime] NOT NULL,
		[DateModified] [datetime] NULL,
		[IsDeleted] [bit] DEFAULT 0,
	CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
END

