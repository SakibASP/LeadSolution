
CREATE OR ALTER VIEW dbo.ViewFormValues 
AS
SELECT DISTINCT 
	fd.Id
	,fd.FormMasterId
	,fm.BusinessId
	,fd.FormId
	,fd.FormValue
	,fm.SubmissionId
	,fm.CreatedDate
FROM FormValueDetails fd
INNER JOIN FormValueMaster fm ON fd.FormMasterId = fm.Id
