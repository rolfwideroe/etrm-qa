
declare @timeSeriesTypeTable table
						(
							TsIndex INT IDENTITY(1,1), 
							TimeSeriesType VARCHAR(50),
							PreFix VARCHAR(50)
						)

Insert into @timeSeriesTypeTable values('Volume','Volume-') -- disregard MtM valuation
Insert into @timeSeriesTypeTable values('Contract price','ContractPrice-') -- disregard option eval strategy
Insert into @timeSeriesTypeTable values('Contract value','ContractValue-') -- disregard option eval strategy
Insert into @timeSeriesTypeTable values('PL accrued','AccruedPL-')
Insert into @timeSeriesTypeTable values('CF accrued','AccruedCF-')

DECLARE @table table (
						JobIndex INT IDENTITY(1,1), 
						FilterName VARCHAR(50),
						ReportDate VARCHAR(55),
						Currency VARCHAR(50),
						UseVolSurf VARCHAR(50),
						OptionEvalStrategy VARCHAR(50),
						IncludeVolumeTest VARCHAR(50),
						IncludeContractPriceAndValueTest VARCHAR(50)
					  )
	
Insert into @table values ('El-European-EUR','20111116','EUR','False','OptionFormula','False','True')
Insert into @table values ('El-European-EUR','20111116','NOK','False','OptionFormula','False','True')
Insert into @table values ('El-European-EUR','20111116','EUR','False','Delta','True','False')
Insert into @table values ('El-European-EUR','20111116','NOK','False','Delta','True','False')
Insert into @table values ('El-European-EUR','20111116','EUR','False','Binary','True','False')
Insert into @table values ('El-European-EUR','20111116','NOK','False','Binary','True','False')
Insert into @table values ('El-European-EUR','20111116','EUR','False','BinaryDelta','True','False')
Insert into @table values ('El-European-EUR','20111116','EUR','True','OptionFormula','False','False')

Insert into @table values ('El-EU-Profile-EUR','20111116','EUR','False','OptionFormula','False','True')
Insert into @table values ('El-EU-Profile-EUR','20111116','EUR','False','Delta','True','False')

Insert into @table values ('El-EU-Struct-EUR','20111116','EUR','False','OptionFormula','False','True')
Insert into @table values ('El-EU-Struct-EUR','20111116','NOK','False','OptionFormula','False','True')
Insert into @table values ('El-EU-Struct-EUR','20111116','EUR','False','Delta','True','False')

Insert into @table values ('Gas-EU-ICE-TTF-EUR','20111101','EUR','False','OptionFormula','False','True')
Insert into @table values ('Gas-EU-ICE-TTF-EUR','20111101','NOK','False','OptionFormula','False','True')
Insert into @table values ('Gas-EU-ICE-TTF-EUR','20111101','EUR','False','Delta','True','False')
Insert into @table values ('Gas-EU-ICE-TTF-EUR','20111101','NOK','False','Delta','True','False')
Insert into @table values ('Gas-EU-ICE-TTF-EUR','20111101','EUR','False','Binary','True','False')

Insert into @table values ('El-Asian-EUR','20111116','EUR','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-EUR','20111116','NOK','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-EUR','20111116','EUR','False','Delta','True','False')
Insert into @table values ('El-Asian-EUR','20111116','EUR','False','Binary','True','False')
Insert into @table values ('El-Asian-EUR','20111116','EUR','False','BinaryDelta','True','False')
Insert into @table values ('El-Asian-EUR','20111116','EUR','True','OptionFormula','False','False')

Insert into @table values ('El-Asian-NOK','20111116','NOK','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-NOK','20111116','NOK','False','Delta','True','False')

Insert into @table values ('El-Asian-Prof-EUR','20111116','EUR','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-Prof-EUR','20111116','EUR','False','Delta','True','False')

Insert into @table values ('El-Asian-Spread-EUR','20111116','EUR','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-Spread-EUR','20111116','NOK','False','OptionFormula','False','True')
Insert into @table values ('El-Asian-Spread-EUR','20111116','EUR','False','Delta','True','False')
Insert into @table values ('El-Asian-Spread-EUR','20111116','EUR','False','Binary','True','False')



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
		
		declare @includeVolumeTest bit
		set @includeVolumeTest = (SELECT IncludeVolumeTest FROM @table WHERE JobIndex=@index)

		declare @includePriceAndValueTests bit
		set @includePriceAndValueTests = (SELECT IncludeContractPriceAndValueTest FROM @table WHERE JobIndex=@index)

		declare @filterId VARCHAR(55)
		declare @useActualDateFX VARCHAR(55)
		declare @fromDate VARCHAR(55)
		declare @toDate VARCHAR(55)
		declare @longFromDate VARCHAR(55)
		declare @longToDate VARCHAR(55)
		declare @shortFromDate VARCHAR(55)
		declare @shortToDate VARCHAR(55)
		declare @shortFromDate2 VARCHAR(55)
		declare @shortToDate2 VARCHAR(55)
		declare @reportDate VARCHAR(55)
		declare @useVolSurf VARCHAR(50)
		declare @optionEvalStrategy VARCHAR(50)
		declare @currency VARCHAR(50)

		set @filterId=(SELECT FilterId FROM Filters WHERE FilterName=(SELECT FilterName FROM @table WHERE JobIndex=@index))

		set @useActualDateFX='False'
		set @fromDate='20111101'
		set @toDate='20121231'
		set @longFromDate='20100101'
		set @longToDate='20161231'
		set @shortFromDate='20111030'
		set @shortToDate='20111031'
		set @shortFromDate2='20120325'
		set @shortToDate2='20120326'
		set @reportDate=(SELECT ReportDate FROM @table WHERE JobIndex=@index)
		set @useVolSurf=(SELECT UseVolSurf FROM @table WHERE JobIndex=@index)
		set @optionEvalStrategy=(SELECT OptionEvalStrategy FROM @table WHERE JobIndex=@index)
		set @currency=(SELECT Currency FROM @table WHERE JobIndex=@index)

		DECLARE @jobName varchar(55)
		set @jobName=@preFix+(SELECT FilterName FROM @table WHERE JobIndex=@index)+'-'+@optionEvalStrategy+'-'+@currency
		if (@useVolSurf = 'True') set @jobName = @jobName + '-VolSurf'
		
		if  (@type!='Volume' or @includeVolumeTest=1) and (@type not like 'Contract %' or @includePriceAndValueTests=1)
			begin 

				if not exists (select * from StoredJobs j 
							   inner join ScheduledJobs s on s.StoredJobId=j.StoredJobId
							   WHERE j.[Description]=@jobName)  
				begin
					
					print('Inserting job ' + @jobName)
					
					INSERT INTO StoredJobs VALUES('Reporting Db Calculated Values Export',@jobName)

					declare @maxStoredJobId INT
					set @maxStoredJobId=(Select max(StoredJobId) from StoredJobs)

					INSERT INTO ScheduledJobs VALUES (@maxStoredJobId,'Vizard',GETDATE(),'Vizard',GETDATE(),0,NULL, NULL)

					-- common parameters
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Name',@jobName)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FilterId',@filterId)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeReportDate','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteReportDate',@reportDate)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ReportCurrency',@currency)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Intraday','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseActualDateSpotRate',@useActualDateFX)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IntrinsicEvalutationOfCapacity','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseVolatilitySurface',@useVolSurf)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'OptionEvaluationStrategy',@optionEvalStrategy)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'UseLiveCurrencyRates','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'NumberOfTimeSeries','6')

					
					-- parameters per time series
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_0',@type)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_0','Month')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesAlias_0',@jobName+'-Month')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_0','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_0',@longFromDate)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_0','0')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_0','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_0',@longToDate)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_0','0')
					if  @type='Volume'
					begin 
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeBase_0','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypePeak_0','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeOffPeak_0','True')
					end
					
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_1',@type)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_1','Day')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesAlias_1',@jobName+'-Day')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_1','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_1',@fromDate)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_1','0')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_1','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_1',@toDate)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_1','0')
					if  @type='Volume'
					begin 
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeBase_1','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypePeak_1','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeOffPeak_1','True')
					end
					
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_2',@type)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_2','Hour')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesAlias_2',@jobName+'-HourFall')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_2','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_2',@shortFromDate)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_2','0')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_2','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_2',@shortToDate)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_2','0')
					if  @type='Volume'
					begin 
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeBase_2','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypePeak_2','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeOffPeak_2','True')
					end

					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_3',@type)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_3','Hour')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesAlias_3',@jobName+'-HourSpring')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_3','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_3',@shortFromDate2)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_3','0')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_3','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_3',@shortToDate2)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_3','0')
					if  @type='Volume'
					begin 
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeBase_3','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypePeak_3','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeOffPeak_3','True')
					end

					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_4',@type)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_4','Min_15')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesAlias_4',@jobName+'-15MinFall')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_4','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_4',@shortFromDate)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_4','0')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_4','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_4',@shortToDate)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_4','0')
					if  @type='Volume'
					begin 
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeBase_4','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypePeak_4','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeOffPeak_4','True')
					end
					
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesType_5',@type)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'Resolution_5','Min_15')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'TimeSeriesAlias_5',@jobName+'-15MinSpring')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeFromDate_5','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteFromDate_5',@shortFromDate2)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'FromDateOffset_5','0')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'IsRelativeToDate_5','False')
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'AbsoluteToDate_5',@shortToDate2)
					INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'ToDateOffset_5','0')
					if  @type='Volume'
					begin 
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeBase_5','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypePeak_5','True')
						INSERT INTO StoredJobParameters VALUES (@maxStoredJobId,'LoadTypeOffPeak_5','True')
					end
					

			end
		
		end

	set @index=@index+1
  end

set @tsIndex=@tsIndex+1
end