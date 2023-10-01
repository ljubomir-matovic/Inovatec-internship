IF (NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='OrderAttachments'))
BEGIN
	CREATE TABLE [dbo].[OrderAttachment](
		[Id] [bigint] IDENTITY(1, 1) NOT NULL,
		[Name] varchar(100) NOT NULL,
		[ContentType] varchar(100) NOT NULL,
		[Content] varbinary(max) NOT NULL,
		[OrderId] [bigint] NOT NULL,
		[DateCreated] [datetime] NOT NULL,
		[DateModified] [datetime] NULL,
		[IsDeleted] [bit] DEFAULT 0,
	CONSTRAINT [FK_OrderAttachment_Order] FOREIGN KEY 
	(
		[OrderId]
	)
	REFERENCES [Order]([Id]),
	CONSTRAINT [PK_OrderAttachment] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)	ON [PRIMARY]
END