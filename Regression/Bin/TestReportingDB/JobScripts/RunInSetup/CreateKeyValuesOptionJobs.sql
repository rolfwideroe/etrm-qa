DECLARE @table table (
					JobIndex INT IDENTITY(1,1), 
					FilterName VARCHAR(50),
					ReportDate VARCHAR(50),
					UseVolSurf VARCHAR(50),
					OptionEvalStrategy VARCHAR(50),
					IncludeVolumeTest VARCHAR(50),
					IncludeContractPriceAndValueTest VARCHAR(50)
					)
					  
Insert into @table values ('El-European-EUR','20111116','False','OptionFormula','False','True')
Insert into @table values ('El-European-EUR','20111116','False','Delta','True','False')
Insert into @table values ('El-European-EUR','20111116','False','Binary','True','False')
Insert into @table values ('El-European-EUR','20111116','False','BinaryDelta','True','False')
Insert into @table values ('El-European-EUR','20111116','True','OptionFormula','False','False')

Insert into @table values ('El-EU-Profile-EUR','20111116','False','OptionFormula','False','True')
Insert into @table values ('El-EU-Profile-EUR','20111116','False','Delta','True','False')

Insert into @table values ('El-EU-Struct-EUR','20111116','False','OptionFormula','False','True')
Insert into @table values ('El-EU-Struct-EUR','20111116','False','Delta','True','False')

Insert into @table values ('Gas-EU-ICE-TTF-EUR','20111101','False','OptionFormula','False','True')
Insert into @table values ('Gas-EU-ICE-TTF-EUR','20111101','False','Delta','True','False')
Insert into @table values ('Gas-EU-ICE-TTF-EUR','20111101','False','Binary','True','False')

Insert into @table values ('El-Asian-EUR','20111116','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-EUR','20111116','False','Delta','True','False')
Insert into @table values ('El-Asian-EUR','20111116','False','Binary','True','False')
Insert into @table values ('El-Asian-EUR','20111116','False','BinaryDelta','True','False')
Insert into @table values ('El-Asian-EUR','20111116','True','OptionFormula','False','False')

Insert into @table values ('El-Asian-NOK','20111116','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-NOK','20111116','False','Delta','True','False')

Insert into @table values ('El-Asian-Prof-EUR','20111116','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-Prof-EUR','20111116','False','Delta','True','False')

Insert into @table values ('El-Asian-Spread-EUR','20111116','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-Spread-EUR','20111116','False','Delta','True','False')
Insert into @table values ('El-Asian-Spread-EUR','20111116','False','Binary','True','False')

declare @index int
set @index=1
declare @maxIndex int
set @maxIndex=(select MAX(JobIndex) from @table)

while @index<=@maxIndex
begin

	declare @wsId INT
	declare @filterId VARCHAR(55)
	declare @granularity VARCHAR(55)
	declare @fromDate VARCHAR(55)
	declare @toDate VARCHAR(55)
	declare @shortFromDate VARCHAR(55)
	declare @shortToDate VARCHAR(55)
	declare @shortFromDate2 VARCHAR(55)
	declare @shortToDate2 VARCHAR(55)
	declare @reportDate VARCHAR(55)
	declare @useVolSurf VARCHAR(50)
	declare @optionEvalStrategy VARCHAR(50)
	declare @includeVolumeTest VARCHAR(50)
	declare @includeContractPriceAndValueTest VARCHAR(50)
	
	set @filterId=(SELECT FilterId FROM Filters WHERE FilterName=(SELECT FilterName FROM @table WHERE JobIndex=@index))
	set @reportDate=(SELECT ReportDate FROM @table WHERE JobIndex=@index)
	set @useVolSurf=(SELECT UseVolSurf FROM @table WHERE JobIndex=@index)
	set @optionEvalStrategy=(SELECT OptionEvalStrategy FROM @table WHERE JobIndex=@index)
	set @includeContractPriceAndValueTest=(SELECT IncludeContractPriceAndValueTest FROM @table WHERE JobIndex=@index)
	set @includeVolumeTest=(SELECT IncludeVolumeTest FROM @table WHERE JobIndex=@index)

	DECLARE @jobName varchar(55)
	set @jobName = (SELECT FilterName FROM @table WHERE JobIndex=@index)
	set @jobName='KeyValues-' + @jobName + '-' + @optionEvalStrategy
	if (@useVolSurf='True') set @jobName = @jobName + '-volSurf'

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
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseActualDateSpotRate','False')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IntrinsicEvalutationOfCapacity','False')
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseVolatilitySurface',@useVolSurf)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'OptionEvaluationStrategy',@optionEvalStrategy)
		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseLiveCurrencyRates','False')
				
		declare @kvIndex INT
		set @kvIndex=1
		declare @maxKvIndex INT

		IF OBJECT_ID('tempdb..#keyValuesTypeTable') IS NOT NULL
			DROP TABLE #keyValuesTypeTable
		CREATE TABLE #keyValuesTypeTable (KvIndex INT IDENTITY(1,1), KeyValuesType VARCHAR(50))

		Insert into #keyValuesTypeTable values('Market price')
		Insert into #keyValuesTypeTable values('MtM')
		Insert into #keyValuesTypeTable values('Net PL')
		if (@includeContractPriceAndValueTest = 'True')
		begin
			Insert into #keyValuesTypeTable values('Contract price')
			Insert into #keyValuesTypeTable values('Contract value')
		end
		if (@includeVolumeTest = 'True')
		begin
			Insert into #keyValuesTypeTable values('Volume total')
			Insert into #keyValuesTypeTable values('Volume undelivered')
		end
		
		set @maxKvIndex=(select MAX(KvIndex) from #keyValuesTypeTable)

		INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'NumberOfKeyValues', @maxKvIndex)
		
		while @kvIndex<=@maxKvIndex
		Begin
			declare @type VARCHAR(55)

			set @type=(SELECT KeyValuesType FROM #keyValuesTypeTable WHERE KvIndex=@kvIndex)
			INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'KeyValueType_' + convert(nvarchar(25), @kvIndex-1) ,@type)
			set @kvIndex=@kvIndex+1
		end	

	end
  set @index=@index+1
end

IF OBJECT_ID('tempdb..#keyValuesTypeTable') IS NOT NULL
			DROP TABLE #keyValuesTypeTable