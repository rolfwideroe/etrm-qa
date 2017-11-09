
  
  DECLARE @table table (
						WIndex INT IDENTITY(1,1), 
						WsId INT,
						WSName VARCHAR(50)
					  )
					  
  insert into @table select WsId,WsName from Workspace
							where wsid in (
							select wsid from WSForm
							where FormName='frmCFMon')
							and locked =1
							and WSName like '%EUR%ActualPL%NOK'
							and WSName not like 'Gencon%'
							order by wsid
  
  declare @index int
  set @index=1
  declare @maxIndex int
  set @maxIndex=(select MAX(WIndex) from @table)
  
  while @index<=@maxIndex
  begin



	declare @wsId INT

	set @wsId=(Select WsId from @table WHERE WIndex=@index)
  DECLARE @alias varchar(55)
  set @alias=(SELECT WSname from Workspace where WSId=@wsId)
  
  if not exists(select * from StoredJobs where [Description]=@alias)
	begin

		declare @filterId VARCHAR(55)
		declare @granularity VARCHAR(55)
		declare @useActualDateFX VARCHAR(55)
		declare @fromDate VARCHAR(55)
		declare @toDate VARCHAR(55)
		declare @reportDate Date
		declare @reportDateAsString VARCHAR(55)



		set @filterId=(select [Value] from WSFormSettings where WSId=@wsId and FormId =(select FormId from WSForm where WSId=@wsId and FormName='frmCFMon') and Setting='FilterId')

		set @useActualDateFX=(select CASE [Value] WHEN '0' THEN 'False'
											  ELSE 'True'
											  END
											 from WSFormSettings where WSId=@wsId and FormId =(select FormId from WSForm where WSId=@wsId and FormName='frmCFMon') and Setting='UseActualDateSpotRates')
		set @fromDate=(select [Value] from WSFormSettings where WSId=@wsId and FormId =(select FormId from WSForm where WSId=@wsId and FormName='frmCFMon') and Setting='FromDate')
		set @toDate=(select [Value] from WSFormSettings where WSId=@wsId and FormId =(select FormId from WSForm where WSId=@wsId and FormName='frmCFMon') and Setting='ToDate')


		set @reportDate=(SELECT CASE  WHEN  TRY_CONVERT(date,[value],101) is null then TRY_CONVERT(date,[value],103)
										ELSE TRY_CONVERT(date,[value],101)
										END
									from WSFormSettings where WSId=@wsId and FormId =(select FormId from WSForm where WSId=@wsId and FormName='frmCFMon') and Setting='reportdate')

		set @reportDateAsString=CONVERT(VARCHAR,TRY_CONVERT(date,@reportDate,103),112)

		set @granularity=(select CASE [Value] WHEN 'Monthly' THEN 'Month' 
										  WHEN 'Daily' THEN 'Day'
										  ELSE [Value]
							END

		from WSFormSettings where WSId=@wsId and FormId =(select FormId from WSForm where WSId=@wsId and FormName='frmCFMon') and Setting='Granularity')



		INSERT INTO StoredJobs VALUES('Reporting Db Calculated Values Export',@alias)

		declare @maxStoredJobId INT
		set @maxStoredJobId=(Select max(StoredJobId) from StoredJobs)

		INSERT INTO ScheduledJobs VALUES (@maxStoredJobId,'Vizard',GETDATE(),'Vizard',GETDATE(),0,NULL,NULL)

		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Name',@alias)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FilterId',@filterId)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeReportDate','False')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteReportDate','20111101')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Intraday','False')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseActualDateSpotRate',@useActualDateFX)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ReportCurrency', 'NOK')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_0','PL accrued')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_0',@granularity)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_0',@alias)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_0','False')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_0',@fromDate)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_0','0')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_0','False')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_0',@toDate)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_0','0')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'NumberOfTimeSeries','1')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeBase_0','True')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypePeak_0','False')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeOffPeak_0','False')

	end

	set @index=@index+1
  end