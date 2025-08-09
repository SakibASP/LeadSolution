/*
=================================
Author : Md. Sakibur Rahman
Created Date : 2025-08-10 

EXEC usp_GetDynamicPivotedFormValues @BusinessId = 4

=================================
*/


CREATE OR ALTER PROCEDURE dbo.usp_GetDynamicPivotedFormValues
    @BusinessId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @cols NVARCHAR(MAX), @query NVARCHAR(MAX);

    -- Get distinct field names for pivot columns based on the BusinessId
    SELECT @cols = STRING_AGG(QUOTENAME(x.Name), ',')
    FROM (
        SELECT DISTINCT fd.Name
        FROM FormDetails fd
        JOIN BusinessSupportedFormIds bsf ON fd.Id = bsf.FormId
        WHERE bsf.BusinessId = @BusinessId
    ) AS x;

    -- If no columns found, return empty result set
    IF @cols IS NULL
    BEGIN
        SELECT 'No fields found for this BusinessId' AS Message;
        RETURN;
    END

    SET @query = '
    SELECT
        --pvt.SubmissionId,
        --pvt.BusinessId,
        --pvt.BusinessName,
        ' + @cols + '
    FROM
    (
        SELECT
            fv.SubmissionId,
            fv.BusinessId,
            bi.BusinessName,
            fd.Name AS FieldName,
            fv.FormValue
        FROM FormValues fv
        JOIN FormDetails fd ON fv.FormId = fd.Id
        JOIN AspNetBusinessInfo bi ON fv.BusinessId = bi.Id
        JOIN BusinessSupportedFormIds bsf ON fv.FormId = bsf.FormId
        WHERE bsf.BusinessId = @BusinessId
    ) src
    PIVOT
    (
        MAX(FormValue)
        FOR FieldName IN (' + @cols + ')
    ) pvt
    ORDER BY pvt.SubmissionId;
    ';

    -- Execute with parameter to avoid SQL injection
    EXEC sp_executesql @query, N'@BusinessId INT', @BusinessId;
END
