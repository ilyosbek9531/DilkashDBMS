USE Dilkash
go
create or alter procedure udpGetAllFoods
as
begin
  SELECT FoodId, FoodName, FoodDescription, FoodImage, FoodType, Availability, Price, CreatedAt
  FROM Food
end
-----
go
exec udpGetAllFoods
go
----------------------------
create or alter procedure udpInsertFood(
  @FoodName nchar(100),
  @FoodDescription nchar(200),
  @FoodImage varbinary(max),
  @FoodType nchar(100),
  @Availability bit OUT,
  @Price int,
  @CreatedAt date
)
as
begin
 begin try
   insert into Food(FoodName, FoodDescription, FoodImage, FoodType, Availability, Price, CreatedAt)
   output inserted.FoodId
   values(@FoodName, @FoodDescription, @FoodImage, @FoodType, @Availability, @Price, @CreatedAt)

   return (0)
 end try
 begin catch
   return (1)
 end catch
end
go
------------------------------

-- Stored procedure for updating an food
CREATE OR ALTER PROCEDURE udpUpdateFood(
    @FoodId INT,
    @FoodName nchar(100),
    @FoodDescription nchar(200),
    @FoodImage varbinary(max),
    @FoodType nchar(100),
    @Availability bit OUT,
    @Price int,
    @CreatedAt date
)
AS
BEGIN
    BEGIN TRY
        UPDATE Food
        SET FoodName = @FoodName,
            FoodDescription = @FoodDescription,
            FoodImage = @FoodImage,
            FoodType = @FoodType,
            Availability = @Availability,
            Price = @Price,
            CreatedAt = @CreatedAt
        WHERE FoodId = @FoodId;

        -- Return success code
        RETURN 0;
    END TRY
    BEGIN CATCH
        -- Return error code
        RETURN 1;
    END CATCH
END
GO

-- Stored procedure for deleting an food by ID
CREATE OR ALTER PROCEDURE udpDeleteFood(
    @FoodId INT
)
AS
BEGIN
    BEGIN TRY
        DELETE FROM Food
        WHERE FoodId = @FoodId;

        -- Return success code
        RETURN 0;
    END TRY
    BEGIN CATCH
        -- Return error code
        RETURN 1;
    END CATCH
END
GO

-- Stored procedure for getting an food by ID
CREATE OR ALTER PROCEDURE udpGetFoodById(
    @FoodId INT
)
AS
BEGIN
    BEGIN TRY
        SELECT FoodId, FoodName, FoodDescription, FoodImage, FoodType, Availability, Price, CreatedAt
        FROM Food
        WHERE FoodId = @FoodId;

        -- Return success code
        RETURN 0;
    END TRY
    BEGIN CATCH
        -- Return error code
        RETURN 1;
    END CATCH
END
GO
