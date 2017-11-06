





DECLARE @table table (
					JobIndex INT IDENTITY(1,1), 
					WorkspaceName VARCHAR(255)

					  )
					  
INSERT INTO @table 
	select WSName from Workspace 
	where ShareType='Common' AND Locked=1
	and WSId>1035

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
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'WorkspaceType','ECM')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ReportDateOffset','0')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Username','Vizard')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Password','elviz')




	end
  set @index=@index+1
end