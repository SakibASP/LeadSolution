/*
=================================
Author : Md. Sakibur Rahman
Created Date : 2025-08-11

=================================
*/


CREATE OR ALTER PROCEDURE dbo.usp_InsertUpdateBusinessSupportedForms
(
    @BusinessId INT,
    @Username NVARCHAR(256),
    @JsonObject NVARCHAR(MAX)
)
AS
BEGIN
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @Now DATETIME = GETDATE();

        BEGIN TRANSACTION;

        IF ISJSON(@JsonObject) = 1 AND LEN(@JsonObject) > 0
        BEGIN
            -- MERGE UPSERT logic
            MERGE INTO dbo.BusinessSupportedFormIds AS TARGET
            USING (
                SELECT 
                    B.FormDetailId AS FormId,
                    B.IsChecked AS IsActive
                FROM OPENJSON(@JsonObject) WITH (
                    FormSelectDetails NVARCHAR(MAX) AS JSON
                ) A
                OUTER APPLY OPENJSON(A.FormSelectDetails)
                WITH (
                    FormDetailId INT,
                    IsChecked BIT
                ) B
            ) AS SOURCE
            ON TARGET.BusinessId = @BusinessId AND TARGET.FormId = SOURCE.FormId

            WHEN MATCHED THEN
                UPDATE SET 
                    TARGET.IsActive = SOURCE.IsActive,
                    TARGET.ModifiedBy = @Username,
                    TARGET.ModifiedDate = @Now

            WHEN NOT MATCHED BY TARGET THEN
                INSERT (BusinessId, FormId, IsActive, CreatedBy, CreatedDate)
                VALUES (@BusinessId, SOURCE.FormId, SOURCE.IsActive, @Username, @Now)

            WHEN NOT MATCHED BY SOURCE AND TARGET.BusinessId = @BusinessId THEN
                DELETE;
        END
        ELSE
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Invalid or empty JSON data.', 16, 1);
            RETURN -1;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState) WITH SETERROR;

        RETURN -1;
    END CATCH;

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
END;
