/*
=================================
Author : Md. Sakibur Rahman
Created Date : 2025-08-09 

EXEC usp_GetBusinessSupportedForms @BusinessId = 4

=================================
*/


CREATE OR ALTER PROCEDURE dbo.usp_GetBusinessSupportedForms
    @BusinessId INT
AS
BEGIN
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SET NOCOUNT ON;

    SELECT 
        FD.Id AS FormDetailId,
        FD.Name AS Label,
        DT.Name AS InputType,
        FD.IsSelectInput,
        ISNULL(BSF.IsActive, 0) AS IsActive
    FROM dbo.FormDetails FD
    INNER JOIN dbo.DataTypes DT ON FD.TypeId = DT.Id
    LEFT JOIN dbo.BusinessSupportedFormIds BSF ON FD.Id = BSF.FormId AND BSF.BusinessId = @BusinessId AND BSF.IsActive = 1
    WHERE FD.IsActive = 1

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
END
GO
