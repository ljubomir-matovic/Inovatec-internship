IF NOT EXISTS (SELECT 1 FROM Information_schema.Routines WHERE Specific_schema='dbo' AND specific_Name = 'CreateOrder' AND Routine_Type='PROCEDURE')
BEGIN
	EXEC ('CREATE PROCEDURE [dbo].[CreateOrder] (@officeId int) AS 	BEGIN 	RETURN 1;	END')
END
GO
ALTER PROCEDURE [dbo].[CreateOrder]
@officeId int
AS
IF(EXISTS(SELECT ItemId,SUM(Amount) FROM [dbo].[OrderRequest] WHERE OfficeId=@officeId AND DateTo IS NULL AND Amount IS NOT NULL GROUP BY ItemId))
BEGIN
	DECLARE @orderId int, @itemId int, @amount int	
	DECLARE @IDs TABLE(ID INT)

    INSERT [dbo].[Order](State,IsDeleted,DateCreated,OfficeId)
        OUTPUT inserted.ID INTO @IDs(ID)
    SELECT 0, 0, CURRENT_TIMESTAMP, @officeId

    SELECT @orderId=ID FROM @IDs

    DECLARE order_items_cursor CURSOR
        FOR SELECT ItemId,SUM(Amount) FROM [dbo].[OrderRequest] WHERE OfficeId=@officeId AND DateTo IS NULL AND IsDeleted=0 AND Amount IS NOT NULL GROUP BY ItemId

    OPEN order_items_cursor

    FETCH NEXT FROM order_items_cursor   
    INTO @itemId, @amount

    WHILE @@FETCH_STATUS=0
    BEGIN

        INSERT INTO [dbo].[OrderItem](ItemId,OrderId,Amount,DateCreated,IsDeleted)
        VALUES(@itemId,@orderId,@amount,CURRENT_TIMESTAMP,0)
    
        FETCH NEXT FROM order_items_cursor   
        INTO @itemId, @amount
    END

    CLOSE order_items_cursor
    DEALLOCATE order_items_cursor

    UPDATE OrderRequest SET DateTo=CURRENT_TIMESTAMP
    WHERE DateTo IS NULL AND Amount IS NOT NULL

	RETURN @orderId
END
GO