/*
=================================
Author : Md. Sakibur Rahman
Created Date : 2025-07-30 

EXEC usp_get_dropdownList @Id = 1

=================================
*/

CREATE OR ALTER PROCEDURE dbo.usp_get_dropdownList
	@Id INT,
	@Param1 NVARCHAR(256) = NULL,
	@Param2 NVARCHAR(256) = NULL,
	@Param3 NVARCHAR(256) = NULL,
	@Param4 NVARCHAR(256) = NULL
AS 
BEGIN

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

IF(@Id = 1) -- Service Types
	BEGIN
		SELECT 
			Id,Name 
		FROM dbo.AspNetServiceTypes
		WHERE IsActive = 1;
	END
ELSE IF(@Id = 2) -- Data Types
	BEGIN
		SELECT 
			Id,Name 
		FROM dbo.DataTypes
		WHERE IsActive = 1;
	END
ELSE IF(@Id = 3) -- Users
	BEGIN
		SELECT 
			Id,UserName Name
		FROM dbo.AspNetUsers
		WHERE (Id LIKE @Param1 OR @Param1 IS NULL);
	END
ELSE IF(@Id = 4) -- Business Wise Users
	BEGIN
		SELECT 
			u.Id,u.UserName Name
		FROM dbo.AspNetUsers u
		INNER JOIN AspNetUserBusinessInfo ub ON u.Id = ub.UserId
		WHERE ub.BusinessId = CONVERT(int,@Param1);
	END
ELSE IF(@Id = 5) -- User Wise Businesses
	BEGIN
		SELECT 
			b.Id,b.BusinessName Name
		FROM  AspNetUserBusinessInfo ub
		INNER JOIN dbo.AspNetUsers u ON ub.UserId = u.Id
		INNER JOIN AspNetBusinessInfo b ON ub.BusinessId = b.Id
		WHERE u.Id = @Param1;
	END


SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

END
