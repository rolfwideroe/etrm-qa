
declare @timeSeriesTypeTable table
						(
							TsIndex INT IDENTITY(1,1), 
							TimeSeriesType VARCHAR(50),
							PreFix VARCHAR(50)
						)
Insert into @timeSeriesTypeTable values('Reserve capacity traded volume','ReserveCapacityTradedVolume-')

DECLARE @table table (
						JobIndex INT IDENTITY(1,1), 
						FilterName VARCHAR(50),
						ReportDate VARCHAR(50),
						FromDate VARCHAR(50),
						ToDate VARCHAR(50),
						ShortFromDate VARCHAR(50),
						ShortToDate VARCHAR(50),
						ShortFromDate2 VARCHAR(50),
						ShortToDate2 VARCHAR(50)
					  )
					  
Insert into @table values ('El-ReserveCapacity-EUR','20111101','20110101','20111231','20111030','20111031','20120325','20120326')

declare @tsIndex INT
set @tsIndex=1
declare @maxTsIndex INT
set @maxTsIndex=(select MAX(TsIndex) from @timeSeriesTypeTable)

while @tsIndex<=@maxTsIndex
Begin
  declare @index int
  set @index=1
  declare @maxIndex int
  set @maxIndex=(select MAX(JobIndex) from @table)
  
  	declare @type VARCHAR(55)
	declare @preFix VARCHAR(55) 

	set @type=(SELECT TimeSeriesType FROM @timeSeriesTypeTable WHERE TsIndex=@tsIndex)
	set @preFix=(SELECT PreFix FROM @timeSeriesTypeTable WHERE TsIndex=@tsIndex)

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

		set @filterId=(SELECT FilterId FROM Filters WHERE FilterName=(SELECT FilterName FROM @table WHERE JobIndex=@index))

		set @useActualDateFX='False'
		set @fromDate=(SELECT FromDate FROM @table WHERE JobIndex=@index)
		set @toDate=(SELECT ToDate FROM @table WHERE JobIndex=@index)
		set @shortFromDate=(SELECT ShortFromDate FROM @table WHERE JobIndex=@index)
		set @shortToDate=(SELECT ShortToDate FROM @table WHERE JobIndex=@index)
		set @shortFromDate2=(SELECT ShortFromDate2 FROM @table WHERE JobIndex=@index)
		set @shortToDate2=(SELECT ShortToDate2 FROM @table WHERE JobIndex=@index)
		set @reportDate=(SELECT ReportDate FROM @table WHERE JobIndex=@index)

		DECLARE @jobName varchar(55)
		set @jobName=@preFix+(SELECT FilterName FROM @table WHERE JobIndex=@index)

		if not exists (select * from StoredJobs j 
                       inner join ScheduledJobs s on s.StoredJobId=j.StoredJobId
                       WHERE j.[Description]=@jobName)  
		begin

			INSERT INTO StoredJobs VALUES('Reporting Db Calculated Values Export',@jobName)

			declare @maxStoredJobId INT
			set @maxStoredJobId=(Select max(StoredJobId) from StoredJobs)

			INSERT INTO ScheduledJobs VALUES (@maxStoredJobId,'Vizard',GETDATE(),'Vizard',GETDATE(),0,NULL,NULL)

			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Name',@jobName)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FilterId',@filterId)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeReportDate','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteReportDate',@reportDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Intraday','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseActualDateSpotRate',@useActualDateFX)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IntrinsicEvalutationOfCapacity','True')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseVolatilitySurface','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseLiveCurrencyRates','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'NumberOfTimeSeries','5')

			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_0',@type)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_0','Month')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_0',@jobName+'-Month')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_0','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_0',@fromDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_0','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_0','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_0',@toDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_0','0')
			
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_1',@type)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_1','Hour')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_1',@jobName+'-HourFall')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_1','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_1',@shortFromDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_1','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_1','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_1',@shortToDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_1','0')

			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_2',@type)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_2','Hour')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_2',@jobName+'-HourSpring')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_2','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_2',@shortFromDate2)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_2','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_2','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_2',@shortToDate2)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_2','0')
						
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_3',@type)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_3','Min_15')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_3',@jobName+'-15MinFall')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_3','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_3',@shortFromDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_3','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_3','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_3',@shortToDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_3','0')
			
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_4',@type)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_4','Min_15')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_4',@jobName+'-15MinSpring')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_4','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_4',@shortFromDate2)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_4','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_4','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_4',@shortToDate2)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_4','0')
		end
	set @index=@index+1
  end

set @tsIndex=@tsIndex+1
end