DECLARE @workspace varchar(255)
DECLARE @monitor varchar(255)
DECLARE @reportDate varchar(255)

set @workspace='{workspace}'
set @monitor='{monitor}'
set @reportDate='{reportdate}'

SELECT  
	  [ExposurePricebasis]
      ,[FromTime]
      ,[UntilTime]
	  ,AVG([Price]) AS BasePrice
      ,AVG([PeakPrice]) AS PeakPrice
      ,AVG([OffPeakrice]) AS OffPeakPrice
  FROM [VizExposureData]
  WHERE [ReportId]=(
						SELECT [ReportId] FROM [VizExposureReports] 
						WHERE [Workspace]=@workspace
						AND [MonitorTitle]=@monitor
						AND [ReportDate]=@reportDate
					)
  GROUP BY [ExposurePricebasis],[FromTime],[UntilTime]
  ORDER BY [ExposurePricebasis],[FromTime],[UntilTime]