declare @workspace varchar(50)
set @workspace='el-struct-fixed-eur-currexposure-nok'
declare @reportDate DATE
	set @reportDate='2011-11-01'
	
DECLARE @venusTable TABLE(	
							[ExposureAgainstCurrency] VARCHAR(55),
							[Date] DATETIME,
							[AggregatedCurrencyExposure] FLOAT
						  )

DECLARE @brandyTable TABLE(	
							[ExposureAgainstCurrency] VARCHAR(55),
							[Date] DATETIME,
							[AggregatedCurrencyExposure] FLOAT
						  )

INSERT INTO @venusTable(
							[ExposureAgainstCurrency],
							[Date],
							[AggregatedCurrencyExposure]
						)
						(
							SELECT					
								  [ExposureAgainstCurrency]
								  ,[Date]
								  ,SUM([CurrencyExposure]) AS AggregatedCurrencyExposure
							      
							  FROM QADatawareHouse_Reg112.dbo.[VizCurrExposureData]
							  WHERE [ReportId]=(
													SELECT [ReportId] FROM QADatawareHouse_Reg112.dbo.[VizCurrExposureReports]
													WHERE [Workspace]=@workspace
													AND [MonitorTitle]=@workspace
													AND [ReportDate]=@reportDate
												)
							  GROUP BY [ExposureAgainstCurrency],[Date]
						 )

INSERT INTO @brandyTable(
							[ExposureAgainstCurrency],
							[Date],
							[AggregatedCurrencyExposure]
						)
						(
							SELECT					
								  [ExposureAgainstCurrency]
								  ,[Date]
								  ,SUM([CurrencyExposure]) AS AggregatedCurrencyExposure
							      
							  FROM QADatawareHouse_Reg122.dbo.[VizCurrExposureData]
							  WHERE [ReportId]=(
													SELECT [ReportId] FROM QADatawareHouse_Reg122.dbo.[VizCurrExposureReports]
													WHERE [Workspace]=@workspace
													AND [MonitorTitle]=@workspace
													AND [ReportDate]=@reportDate
												)
							  GROUP BY [ExposureAgainstCurrency],[Date]
						 )
						 
 select venus.ExposureAgainstCurrency
		,venus.[Date]
		,venus.AggregatedCurrencyExposure as VenusExposure
		,brandy.AggregatedCurrencyExposure as BrandyExposure
		,brandy.AggregatedCurrencyExposure-venus.AggregatedCurrencyExposure as absDif
		,(brandy.AggregatedCurrencyExposure/NULLIF(venus.AggregatedCurrencyExposure,0))-1 as exposureRelDif
from @venusTable venus
 left join @brandyTable brandy
	on venus.ExposureAgainstCurrency=brandy.ExposureAgainstCurrency
	and venus.[Date] =brandy.[Date]
	
 where ROUND(ABS(venus.AggregatedCurrencyExposure-brandy.AggregatedCurrencyExposure),5)>0