IF(NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Equipment') AND 
EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Item'))
BEGIN
	CREATE TABLE Equipment(
		Id BIGINT IDENTITY(1,1) NOT NULL,
		ItemId BIGINT NOT NULL,
		UserId BIGINT NULL,
		IsDeleted BIT NOT NULL,
		DateCreated DATETIME NOT NULL,
		DateModified DATETIME NULL,
		CONSTRAINT [PK_Equipment] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
		CONSTRAINT [FK_Equipment_User] FOREIGN KEY (UserId) REFERENCES [dbo].[User](Id),
		CONSTRAINT [FK_Equipment_Item] FOREIGN KEY (ItemId) REFERENCES [dbo].[Item](Id)
	)
END