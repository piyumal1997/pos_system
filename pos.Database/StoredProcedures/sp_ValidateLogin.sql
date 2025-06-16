USE [RetailManagementSystem]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ValidateLogin')
DROP PROCEDURE [dbo].[sp_ValidateLogin]
GO

CREATE PROCEDURE [dbo].[sp_ValidateLogin]
    @UserName VARCHAR(12),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Employee_ID,
        firstName,
        lastName,
        userName,
        Role_ID
    FROM 
        Employee
    WHERE 
        userName = @UserName
        AND password = @PasswordHash
        AND status = 'Active';
END
GO