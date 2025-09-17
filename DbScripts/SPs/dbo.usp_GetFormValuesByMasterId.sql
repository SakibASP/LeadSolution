
/*
=================================
Author : Md. Sakibur Rahman
Created Date : 2025-09-18 

EXEC usp_GetFormValuesByMasterId @MasterId = 1

=================================
*/

CREATE OR ALTER PROCEDURE dbo.usp_GetFormValuesByMasterId
    @MasterId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        FD.[Name],
        FVD.FormValue
    FROM FormValueDetails FVD
    INNER JOIN FormDetails FD ON FVD.FormId = FD.Id
    WHERE FVD.FormMasterId = @MasterId;
END