IF (
	EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Comment')
)
BEGIN
	IF (
		NOT EXISTS(SELECT 1 FROM SYS.COLUMNS WHERE Name = 'Type' AND Object_ID = Object_ID('dbo.Comment'))
	)
	BEGIN
		ALTER TABLE [dbo].[Comment]
		ADD [Type] [tinyint] NOT NULL DEFAULT 0
	END

	IF (
		NOT EXISTS(SELECT 1 FROM SYS.COLUMNS WHERE Name = 'OrderState' AND Object_ID = Object_ID('dbo.Comment'))
	)
	BEGIN
		ALTER TABLE [dbo].[Comment]
		ADD [OrderState] [tinyint] NULL
	END
		
	IF (
		EXISTS(SELECT 1 FROM SYS.COLUMNS WHERE Name = 'Text' AND Object_ID = Object_ID('dbo.Comment'))
	)
	BEGIN
		ALTER TABLE [dbo].[Comment]
		ALTER COLUMN [Text] [varchar] (500) NULL
	END
END