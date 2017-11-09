


declare @keyValuesTypeTable table
						(
							KvIndex INT IDENTITY(1,1), 
							KeyValuesType VARCHAR(50)
						)
Insert into @keyValuesTypeTable values('Contract price')
Insert into @keyValuesTypeTable values('Contract value')
Insert into @keyValuesTypeTable values('Market price')
Insert into @keyValuesTypeTable values('MtM')
Insert into @keyValuesTypeTable values('Volume total')
Insert into @keyValuesTypeTable values('Volume undelivered')
Insert into @keyValuesTypeTable values('Net PL')


DECLARE @table table (
					JobIndex INT IDENTITY(1,1), 
					FilterName VARCHAR(50),
					ReportDate VARCHAR(50)
					  )
					  
  Insert into @table values ('El-Future-NordPool-EUR','20111101')
  Insert into @table values ('El-FixedPriceFloatingVolume-EUR','20111101')
  Insert into @table values ('El-AvgRateFuture-NASDAQ','20151125')
  Insert into @table values ('El-Forward-EUR','20111101')
  Insert into @table values ('El-ReserveCapacity-EUR','20111101')
  Insert into @table values ('El-Spot-EUR','20111101')
  Insert into @table values ('El-StructFlex-CapFloor-EUR','20111101')
  Insert into @table values ('El-StructFlex-Fixed-EUR','20111101')
  Insert into @table values ('El-StructFlex-Index-EUR','20111101')
  Insert into @table values ('El-StructFlex-Spot-EUR','20111101')
  Insert into @table values ('ExpHistoricalPrices-Spot-EUR','20111101')
  Insert into @table values ('ExpHistoricalPrices-FSD-EUR','20111101')
  Insert into @table values ('Electricity-FTR-EUR','20111101')
  Insert into @table values ('Gas-Forward-MW-EUR','20111101')
  Insert into @table values ('Gas-Forward-GJd-EUR','20111101')
  Insert into @table values ('Gas-Forward-MWhd-EUR','20111101')
  Insert into @table values ('Gas-Forward-Sm3d-EUR','20111101')
  Insert into @table values ('Gas-Forward-THD-GBP','20111101')
  Insert into @table values ('Gas-Future-ICE-TTF-EUR','20111101')
  Insert into @table values ('Gas-StructFlex-CapFloor-EUR','20111101')
  Insert into @table values ('Gas-Storage-TTF-Mix','20111101')
  Insert into @table values ('Gas-StructFlex-Fixed-GJd-EUR','20111101')
  Insert into @table values ('Gas-StructFlex-Fixed-MW-EUR','20111101')
  Insert into @table values ('Gas-StructFlex-Fixed-MWhd-EUR','20111101')
  Insert into @table values ('Gas-StructFlex-Fixed-Thd-GBP','20111101')
  Insert into @table values ('Gas-StructFlex-Index-Sm3d-EUR','20111101')
	
	Insert into @table values ('Gas-FFF-ICEMonthAheadNBP-GBP','20111101')
	Insert into @table values ('Gas-Floating-Index-Mwhd-EUR','20111101')
	Insert into @table values ('El-Floating-Index-8020-EUR','20111101')
	
	Insert into @table values ('El-Struct-Fixed-EUR','20111101')
	Insert into @table values ('El-Struct-Spot-EUR','20111101')
	
	Insert into @table values ('ElCert-Forward-OTC','20111101')
	Insert into @table values ('Emission-Future-ICE-EUR','20111101')
	Insert into @table values ('Emission-Future-NP-EUR','20111101')
	
	Insert into @table values ('Oil-Future-ICE-USD','20111101')
	Insert into @table values ('Oil-Future-Brent-USD','20170301')
	Insert into @table values ('El-Floating-Index-8020-EUR','20111101')
	Insert into @table values ('Special-Cases-ZeroVolume','20111101')
	Insert into @table values ('Special-Cases-GridPointPricebasis','20111101')
	Insert into @table values ('Special-Cases-FSD-Null','20111101')
	
	Insert into @table values ('Oil-FFF-ICEBrentOilFLFSwap-USD',		'20111101')
	Insert into @table values ('Currency-Forward',						'20111101')
	Insert into @table values ('Currency-Future',						'20111101')
	Insert into @table values ('Currency-AvgRateForward',				'20111101')
	Insert into @table values ('GreenCert-Forward-OTC-WithPricebasis',	'20111101')

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

	set @filterId=(SELECT FilterId FROM Filters WHERE FilterName=(SELECT FilterName FROM @table WHERE JobIndex=@index))

	set @useActualDateFX='False'
	set @reportDate=(SELECT ReportDate FROM @table WHERE JobIndex=@index)

	DECLARE @jobName varchar(55)
	set @jobName='KeyValues-' + (SELECT FilterName FROM @table WHERE JobIndex=@index)

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
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Intraday','False')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseActualDateSpotRate',@useActualDateFX)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IntrinsicEvalutationOfCapacity','True')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseVolatilitySurface','False')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseLiveCurrencyRates','False')
		
		declare @kvIndex INT
		set @kvIndex=1
		declare @maxKvIndex INT
		set @maxKvIndex=(select MAX(KvIndex) from @keyValuesTypeTable)

		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'NumberOfKeyValues', @maxKvIndex)

		while @kvIndex<=@maxKvIndex
		Begin
			declare @type VARCHAR(55)

			set @type=(SELECT KeyValuesType FROM @keyValuesTypeTable WHERE KvIndex=@kvIndex)

			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'KeyValueType_' + convert(nvarchar(25), @kvIndex-1) ,@type)
			set @kvIndex=@kvIndex+1
		end
	end
  set @index=@index+1
end