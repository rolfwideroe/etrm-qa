  DECLARE @table table (
						JobIndex INT IDENTITY(1,1), 
						FilterName VARCHAR(50)
					   )
					  
Insert into @table values ('El-Future-NordPool-EUR')
Insert into @table values ('El-FixedPriceFloatingVolume-EUR')
Insert into @table values ('El-Forward-EUR')
Insert into @table values ('El-ReserveCapacity-EUR')
Insert into @table values ('El-StructFlex-CapFloor-EUR')
Insert into @table values ('El-StructFlex-Fixed-EUR')
Insert into @table values ('El-StructFlex-Index-EUR')
Insert into @table values ('El-StructFlex-Spot-EUR')
Insert into @table values ('Gas-Forward-MW-EUR')
Insert into @table values ('Gas-Forward-GJd-EUR')
Insert into @table values ('Gas-Forward-MWhd-EUR')
Insert into @table values ('Gas-Forward-Sm3d-EUR')
Insert into @table values ('Gas-Forward-THD-GBP')
Insert into @table values ('Gas-Future-ICE-TTF-EUR')
Insert into @table values ('Gas-StructFlex-CapFloor-EUR')
Insert into @table values ('Gas-Storage-TTF-Mix')
Insert into @table values ('Gas-StructFlex-Fixed-GJd-EUR')
Insert into @table values ('Gas-StructFlex-Fixed-MW-EUR')
Insert into @table values ('Gas-StructFlex-Fixed-MWhd-EUR')
Insert into @table values ('Gas-StructFlex-Fixed-Thd-GBP')
Insert into @table values ('Gas-StructFlex-Index-Sm3d-EUR')
	
Insert into @table values ('Gas-FFF-ICEMonthAheadNBP-GBP')
Insert into @table values ('Gas-Floating-Index-Mwhd-EUR')
Insert into @table values ('El-Floating-Index-8020-EUR')
	
Insert into @table values ('El-Struct-Fixed-EUR')
Insert into @table values ('El-Struct-Spot-EUR')
	
Insert into @table values ('ElCert-Forward-OTC')
Insert into @table values ('Emission-Future-ICE-EUR')
Insert into @table values ('Emission-Future-NP-EUR')
	
Insert into @table values ('Oil-Future-ICE-USD')
Insert into @table values ('El-Floating-Index-8020-EUR')
Insert into @table values ('Special-Cases-ZeroVolume')
Insert into @table values ('Special-Cases-GridPointPricebasis')
	
Insert into @table values ('Oil-FFF-ICEBrentOilFLFSwap-USD')
Insert into @table values ('GreenCert-Forward-OTC-WithPricebasis')

declare @index int
set @index=1
declare @maxIndex int
set @maxIndex=(select MAX(JobIndex) from @table)
  
declare @type VARCHAR(55)
declare @preFix VARCHAR(55) 

set @type='PL accrued'

while @index<=@maxIndex
	begin

		declare @wsId INT
		declare @filterId VARCHAR(55)
		declare @granularity VARCHAR(55)
		declare @useActualDateFX VARCHAR(55)
		declare @fromDate VARCHAR(55)
		declare @toDate VARCHAR(55)
		declare @reportDate VARCHAR(55)
		declare @reportCurrency VARCHAR(5)

		set @filterId=(SELECT FilterId FROM Filters WHERE FilterName=(SELECT FilterName FROM @table WHERE JobIndex=@index))

		set @useActualDateFX='True'
		set @reportCurrency='NOK'
		set @fromDate='20110301'
		set @toDate='20111130'
		set @reportDate='20111101'

		DECLARE @jobName varchar(55)
		set @jobName= 'AccruedPLNOK-' + (SELECT FilterName FROM @table WHERE JobIndex=@index)

		if not exists (select * from StoredJobs j 
                       inner join ScheduledJobs s on s.StoredJobId=j.StoredJobId
                       WHERE j.[Description]=@jobName)  
		begin

			INSERT INTO StoredJobs VALUES('Reporting Db Calculated Values Export',@jobName)

			declare @maxStoredJobId INT
			set @maxStoredJobId=(Select max(StoredJobId) from StoredJobs)

			INSERT INTO ScheduledJobs VALUES (@maxStoredJobId,'Vizard',GETDATE(),'Vizard',GETDATE(),0,NULL, NULL)

			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Name',@jobName)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FilterId',@filterId)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeReportDate','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteReportDate',@reportDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ReportCurrency',@reportCurrency)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Intraday','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseActualDateSpotRate',@useActualDateFX)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IntrinsicEvalutationOfCapacity','True')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseVolatilitySurface','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseLiveCurrencyRates','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'NumberOfTimeSeries','3')

			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_0',@type)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_0','Hour')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_0',@jobName+'-Hour')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_0','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_0',@fromDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_0','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_0','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_0',@toDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_0','0')
		
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_1',@type)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_1','Day')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_1',@jobName+'-Day')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_1','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_1',@fromDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_1','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_1','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_1',@toDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_1','0')

			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_2',@type)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_2','Month')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Alias_2',@jobName+'-Month')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_2','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_2',@fromDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_2','0')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_2','False')
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_2',@toDate)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_2','0')	
			
		end
	set @index=@index+1
  end
