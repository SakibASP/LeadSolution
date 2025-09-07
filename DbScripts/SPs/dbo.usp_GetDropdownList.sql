/*
=================================
Author : Md. Sakibur Rahman
Created Date : 2025-07-30 

EXEC usp_GetDropdownList @Id = 6

=================================
*/


CREATE OR ALTER PROCEDURE dbo.usp_GetDropdownList
	@Id INT,
	@Param1 NVARCHAR(256) = NULL,
	@Param2 NVARCHAR(256) = NULL,
	@Param3 NVARCHAR(256) = NULL,
	@Param4 NVARCHAR(256) = NULL
AS 
BEGIN

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON;

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
		WHERE (Id = @Param1 OR @Param1 IS NULL);
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
		WHERE (u.Id = @Param1 OR @Param1 IS NULL);
	END
ELSE IF(@Id = 6) -- Country List
	BEGIN
		SELECT 
			c.Id,c.CommonName Name
		FROM  gen.Countries c;
	END
ELSE IF(@Id = 7) -- State List
	BEGIN
		SELECT 
			TOP 100 s.Id,s.Name 
		FROM  gen.States s 
		WHERE 
			-- 147 stands for Bangladesh
			(s.CountryId = (SELECT TOP 1 Id FROM gen.Countries WHERE Id = CONVERT(int,@Param1)) OR s.CountryId = 147)
			AND (s.Name like '%'+@Param2+'%' OR @Param2 IS NULL);
	END
ELSE IF(@Id = 8) -- City List
	BEGIN
		SELECT 
			TOP 100 c.Id,c.Name 
		FROM  gen.Cities c 
		WHERE c.StateId = (SELECT TOP 1 Id FROM gen.States WHERE Id = CONVERT(int,@Param1))
			AND (c.Name like '%'+@Param2+'%' OR @Param2 IS NULL);
	END


SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

END
