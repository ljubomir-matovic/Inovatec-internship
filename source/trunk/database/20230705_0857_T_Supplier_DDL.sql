IF (
	NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Supplier')
)
BEGIN
	CREATE TABLE [dbo].[Supplier](
		[Id] [bigint] IDENTITY(1, 1) NOT NULL,
		[Name] [varchar] (500) NOT NULL,
		[PhoneNumber] [varchar] (20) NOT NULL,
		[Country] [varchar] (50) NOT NULL,
		[City] [varchar] (50) NOT NULL,
		[Address] [varchar] (200) NOT NULL,
		[DateCreated] [datetime] NOT NULL,
		[DateModified] [datetime] NULL,
		[IsDeleted] [bit] DEFAULT 0,
		CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED
		(
			[Id] ASC
		)
		WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	)ON [PRIMARY]
END