IF (NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Item')
	AND 
	EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Category'))
BEGIN
	CREATE TABLE [dbo].[Item](
		[Id] [bigint] IDENTITY(1, 1) NOT NULL,
		[Name] [varchar](50) NOT NULL,
		[CategoryId] [bigint] NOT NULL,
		[DateCreated] [datetime] NULL,
		[DateModified] [datetime] NULL,
		[IsDeleted] [bit] DEFAULT 0,
	CONSTRAINT [FK_Item_Category] FOREIGN KEY 
	(
		[CategoryId]
	)
	REFERENCES [Category]([Id]),
	CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)	ON [PRIMARY]
END