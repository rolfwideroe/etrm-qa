DECLARE @workspace varchar(255)
DECLARE @monitor varchar(255)
DECLARE @reportDate varchar(255)

set @workspace='{workspace}'
set @monitor='{monitor}'
set @reportDate='{reportdate}'

SELECT  
	  [ExposurePricebasis]
	  ,[QuantityUnit]
      ,[FromTime]
      ,[UntilTime]
	  ,SUM([Exposure]) AS BaseExposure
      ,SUM([PeakExposure]) AS PeakExposure
      ,SUM([OffPeakExposure]) AS OffPeakExposre
  FROM [VizExposureData]
  WHERE [ReportId]=(
						SELECT [ReportId] FROM [VizExposureReports] 
						WHERE [Workspace]=@workspace
						AND [MonitorTitle]=@monitor
						AND [ReportDate]=@reportDate
					)
  GROUP BY [ExposurePricebasis],[FromTime],[UntilTime],[QuantityUnit]
  ORDER BY [ExposurePricebasis],[FromTime],[UntilTime],[QuantityUnit]