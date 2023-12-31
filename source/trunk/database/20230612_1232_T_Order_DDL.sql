IF (
	EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Order')
	AND
	NOT EXISTS(SELECT 1 FROM SYS.COLUMNS WHERE Name = 'DateTo' AND Object_ID = Object_ID('dbo.Order'))
)
BEGIN
	ALTER TABLE [dbo].[Order]
	ADD [DateTo] [datetime] NULL
END