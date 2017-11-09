--The table structure for settlement filters is not really necessary, 
--but could come in handy if more filters or period combinations are needed later.
DECLARE @table table (
						JobIndex INT IDENTITY(1,1), 
						JobName VARCHAR(255),
						FilterName VARCHAR(50),
						ReportDate VARCHAR(50),
						FromDate VARCHAR(50),
						ToDate VARCHAR(50)
					  )
					  
--!!! Note that the filter with the name 'SettlementMix' must exist before this script is run
Insert into @table values ('Intraday-Special-Cases-FSD-Model-Mix','Special-Cases-FSD-Model','20111101','20110101','20130430' )

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
		declare @reportDate VARCHAR(55)

		set @filterId=(SELECT FilterId FROM Filters WHERE FilterName=(SELECT FilterName FROM @table WHERE JobIndex=@index))

		set @useActualDateFX='False'
		set @fromDate=(SELECT FromDate FROM @table WHERE JobIndex=@index)
		set @toDate=(SELECT ToDate FROM @table WHERE JobIndex=@index)
		set @reportDate=(SELECT ReportDate FROM @table WHERE JobIndex=@index)

		DECLARE @jobName varchar(55)
		set @jobName=(SELECT JobName FROM @table WHERE JobIndex=@index)

		if not exists (select * from StoredJobs WHERE [Description]=@jobName)  
		begin

			INSERT INTO StoredJobs VALUES('Reporting Db Calculated Values Export',@jobName)

			declare @maxStoredJobId INT
			set @maxStoredJobId=(Select max(StoredJobId) from StoredJobs)

			INSERT INTO ScheduledJobs VALUES (@maxStoredJobId,'Vizard',GETDATE(),'Vizard',GETDATE(),0,NULL,NULL)

			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Name',@jobName)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FilterId',@filterId)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeReportDate','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteReportDate',@reportDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Intraday','True')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseActualDateSpotRate',@useActualDateFX)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IntrinsicEvalutationOfCapacity','True')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseVolatilitySurface','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseLiveCurrencyRates','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'NumberOfTimeSeries','3')

			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_0','CF accrued')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_0','Hour')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_0','AccruedCF-Day')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_0','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_0',@fromDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_0','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_0','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_0',@toDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_0','0')
			
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_1','PL accrued')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_1','Month')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_1','PL accrued hour')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_1','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_1',@fromDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_1','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_1','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_1',@toDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_1','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeBase_1','True')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypePeak_1','True')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeOffPeak_1','True')
			
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_2','Volume in MWh')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_2','Day')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_2','Volume in MWh 15min')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_2','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_2',@fromDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_2','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_2','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_2',@toDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_2','0')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeBase_2','True')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypePeak_2','True')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeOffPeak_2','True')

		end
	set @index=@index+1
  end
