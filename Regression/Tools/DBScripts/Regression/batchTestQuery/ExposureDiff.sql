
declare @workspace varchar(50)
set @workspace='el-struct-fixed-eur-exposure-eur'
declare @reportDate DATE
	set @reportDate='2011-11-01'


DECLARE @brandyTable TABLE(
								 [Pricebasis] VARCHAR(55)
								  ,[QuantityUnit] VARCHAR(55)
								  ,[FromTime] DATETIME
								  ,[UntilTime] DATETIME
								  ,[BaseExposure] FLOAT 
								  ,[PeakExposure] FLOAT
								  ,[OffPeakExposre] FLOAT
								  ,[BasePrice] FLOAT
								  ,[PeakPrice] FLOAT
								  ,[OffPeakPrice] FLOAT
							)

DECLARE @drambuiTable TABLE(
								 [Pricebasis] VARCHAR(55)
								  ,[QuantityUnit] VARCHAR(55)
								  ,[FromTime] DATETIME
								  ,[UntilTime] DATETIME
								  ,[BaseExposure] FLOAT 
								  ,[PeakExposure] FLOAT
								  ,[OffPeakExposre] FLOAT
								  ,[BasePrice] FLOAT
								  ,[PeakPrice] FLOAT
								  ,[OffPeakPrice] FLOAT
							)


INSERT INTO @brandyTable(
						[Pricebasis] 
						  ,[QuantityUnit] 
						  ,[FromTime] 
						  ,[UntilTime] 
						  ,[BaseExposure]  
						  ,[PeakExposure] 
						  ,[OffPeakExposre] 
						  ,[BasePrice] 
						  ,[PeakPrice] 
						  ,[OffPeakPricE] 
						)
(
SELECT  
	  [Pricebasis]
	  ,[QuantityUnit]
      ,[FromTime]
      ,[UntilTime]
	  ,SUM([Exposure]) AS BaseExposure
      ,SUM([PeakExposure]) AS PeakExposure
      ,SUM([OffPeakExposure]) AS OffPeakExposre
      ,AVG([Price]) AS BasePrice
      ,AVG([PeakPrice]) AS PeakPrice
      ,AVG([OffPeakrice]) AS OffPeakPrice
  FROM QADatawareHouse_Reg122.dbo.[VizExposureData]
  WHERE [ReportId]=(
						SELECT [ReportId] FROM QADatawareHouse_Reg122.dbo.[VizExposureReports] 
						WHERE [Workspace]=@workspace
						AND [MonitorTitle]=@workspace
						AND [ReportDate]=@reportDate
					)
  GROUP BY [Pricebasis],[FromTime],[UntilTime],[QuantityUnit]
 )


INSERT INTO @drambuiTable(
						[Pricebasis] 
						  ,[QuantityUnit] 
						  ,[FromTime] 
						  ,[UntilTime] 
						  ,[BaseExposure]  
						  ,[PeakExposure] 
						  ,[OffPeakExposre] 
						  ,[BasePrice] 
						  ,[PeakPrice] 
						  ,[OffPeakPricE] 
						)
(
SELECT  
	  [Pricebasis]
	  ,[QuantityUnit]
      ,[FromTime]
      ,[UntilTime]
	  ,SUM([Exposure]) AS BaseExposure
      ,SUM([PeakExposure]) AS PeakExposure
      ,SUM([OffPeakExposure]) AS OffPeakExposre
      ,AVG([Price]) AS BasePrice
      ,AVG([PeakPrice]) AS PeakPrice
      ,AVG([OffPeakrice]) AS OffPeakPrice
  FROM QADatawareHouse_Reg131.dbo.[VizExposureData]
  WHERE [ReportId]=(
						SELECT [ReportId] FROM QADatawareHouse_Reg131.dbo.[VizExposureReports] 
						WHERE [Workspace]=@workspace
						AND [MonitorTitle]=@workspace
						AND [ReportDate]=@reportDate
					)
  GROUP BY [Pricebasis],[FromTime],[UntilTime],[QuantityUnit]
 )
 
 select venus.Pricebasis
		,venus.QuantityUnit
		,venus.FromTime
		,venus.UntilTime
		,(brandy.BaseExposure/NULLIF(venus.BaseExposure,0))-1 as baseExpRelDiff
		,(brandy.PeakExposure/NULLIF(venus.PeakExposure,0))-1 as peakExpRelDiff 
		,(brandy.OffPeakExposre/NULLIF(venus.OffPeakExposre,0))-1 as offPeakExpRelDiff 
		,(brandy.BasePrice/NULLIF(venus.BasePrice,0))-1 as basePriceRelDiff
		,(brandy.PeakPrice/NULLIF(venus.PeakPrice,0))-1 as peakPriceRelDiff 
		,(brandy.OffPeakPrice/NULLIF(venus.OffPeakPrice,0))-1 as offPeakPriceRelDiff 
from @brandyTable venus
 left join @drambuiTable brandy
	on venus.Pricebasis=brandy.Pricebasis
	and venus.QuantityUnit=brandy.QuantityUnit
	and venus.FromTime=brandy.FromTime
	and venus.UntilTime=brandy.UntilTime
 --where ROUND(ABS(venus.BasePrice-brandy.BasePrice),8)>0