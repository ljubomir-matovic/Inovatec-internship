IF (NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='User'))
BEGIN
	CREATE TABLE [dbo].[User](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[FirstName] [varchar](50) NOT NULL,
		[LastName] [varchar](50) NOT NULL,
		[Email] [varchar](70) NOT NULL,
		[Password] [varchar](70) NOT NULL,
		[Role] [int] NOT NULL,
		[DateCreated] [datetime] NOT NULL,
		[IsDeleted] [bit] NOT NULL,
		CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	)
END



