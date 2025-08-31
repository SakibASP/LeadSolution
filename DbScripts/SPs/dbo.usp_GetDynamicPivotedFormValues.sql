USE [LeadSolution]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetDynamicPivotedFormValues]    Script Date: 2025-08-15 2:11:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
=================================
Author : Md. Sakibur Rahman
Created Date : 2025-08-10 

EXEC usp_GetDynamicPivotedFormValues 
@BusinessId = 4
,@FromDate = '2025-08-25'
,@ToDate = '2025-08-27'

=================================
*/


ALTER   PROCEDURE [dbo].[usp_GetDynamicPivotedFormValues]
    @BusinessId INT,
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @cols NVARCHAR(MAX), @query NVARCHAR(MAX);

    -- Get distinct field names for pivot columns based on the BusinessId
    SELECT @cols = STRING_AGG(QUOTENAME(x.Name), ',')
    FROM (
        SELECT DISTINCT fd.Name
        FROM FormDetails fd
        JOIN BusinessSupportedFormId bsf ON fd.Id = bsf.FormId AND bsf.IsActive = 1
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
        pvt.SubmissionId,
        pvt.BusinessId,
        pvt.CreatedDate [Created Date],
        --pvt.BusinessName,
        ' + @cols + '
    FROM
    (
        SELECT
            fv.SubmissionId,
            fv.BusinessId,
            bi.BusinessName,
            fd.Name AS FieldName,
            fv.FormValue,
            FORMAT(fv.CreatedDate, ''yyyy-MM-dd hh:mm tt'') CreatedDate
        FROM FormValues fv
        INNER JOIN FormDetails fd ON fv.FormId = fd.Id
        INNER JOIN AspNetBusinessInfo bi ON fv.BusinessId = bi.Id
        INNER JOIN BusinessSupportedFormId bsf ON fv.FormId = bsf.FormId AND bsf.IsActive = 1 AND fv.BusinessId = bsf.BusinessId
        WHERE 
            bsf.BusinessId = @BusinessId 
            AND CAST(fv.CreatedDate AS DATE) BETWEEN @FromDate AND @ToDate
    ) src
    PIVOT
    (
        MAX(FormValue)
        FOR FieldName IN (' + @cols + ')
    ) pvt
    ORDER BY pvt.SubmissionId;
    ';

    -- Execute with parameter to avoid SQL injection
    EXEC sp_executesql @query, N'@BusinessId INT, @FromDate DATE, @ToDate DATE', @BusinessId, @FromDate, @ToDate;
END
