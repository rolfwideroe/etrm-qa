





DECLARE @table table (
					JobIndex INT IDENTITY(1,1), 
					WorkspaceName VARCHAR(255)

					  )
			
USE QASystem_Reg162
		  
INSERT INTO @table 
select EP_PropertyValue from EP_ElementProperties p
inner join ED_ElementData d on p.EP_ED_Id=d.ED_Id
inner join EC_ElementCategory c on c.EC_Id=d.ED_EC_Id
where p.EP_PropertyName='name'
and d.ED_AppName='erm'
and p.EP_ED_Id in
(
  select EP_ED_Id from [EP_ElementProperties]
  where EP_PropertyName='workspacetype'and EP_PropertyValue='Common'
  AND EP_ED_Id in (
					  select EP_ED_Id from [EP_ElementProperties]
						where EP_PropertyName='workspacelocked'and EP_PropertyValue='True'
					) 

  group by EP_ED_Id
)
order by EP_PropertyValue

SELECT * FROM @table

USE QAECM_Reg162

declare @index int
set @index=1
declare @maxIndex int
set @maxIndex=(select MAX(JobIndex) from @table)

while @index<=@maxIndex
begin

	declare @wsId INT
	declare @filterId VARCHAR(55)
	declare @granularity VARCHAR(55)
	declare @useActualDateFX VARCHAR(55)
	declare @fromDate VARCHAR(55)
	declare @toDate VARCHAR(55)
	declare @shortFromDate VARCHAR(55)
	declare @shortToDate VARCHAR(55)
	declare @shortFromDate2 VARCHAR(55)
	declare @shortToDate2 VARCHAR(55)
	declare @reportDate VARCHAR(55)




	DECLARE @jobName varchar(255)
	DECLARE @wsName varchar(255)
	set @wsName=(SELECT WorkspaceName FROM @table WHERE JobIndex=@index)
	set @jobName='PreTest_' + @wsName

	if not exists (select * from StoredJobs WHERE [Description]=@jobName)  
	begin

		INSERT INTO StoredJobs VALUES('Run Workspace Job',@jobName)

		declare @maxStoredJobId INT
		set @maxStoredJobId=(Select max(StoredJobId) from StoredJobs)

		INSERT INTO ScheduledJobs VALUES (@maxStoredJobId,'Vizard',GETDATE(),'Vizard',GETDATE(),0,NULL)

		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'WorkspaceName',@wsName)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'WorkspaceType','ERM')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ReportDateOffset','0')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Username','Vizard')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Password','elviz')




	end
  set @index=@index+1
end