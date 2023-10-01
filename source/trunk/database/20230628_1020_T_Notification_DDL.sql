IF (NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Notification'))
BEGIN
	CREATE TABLE [dbo].[Notification](
			[Id] [bigint] IDENTITY(1, 1) NOT NULL,
			[Data] varchar(max) NOT NULL,
			[Description] varchar(255) NOT NULL,
			[IsRead] [bit] CONSTRAINT DF_Notification_IsRead DEFAULT 0,
			[UserId] [bigint] NOT NULL,
			[DateCreated] [datetime] NOT NULL,
			[DateModified] [datetime] NULL,
			[IsDeleted] [bit] CONSTRAINT DF_Notification_IsDeleted DEFAULT 0,
		CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED
		(
			[Id] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
		CONSTRAINT FK_Notification_User FOREIGN KEY (UserId) REFERENCES [dbo].[User](Id)
	)
END