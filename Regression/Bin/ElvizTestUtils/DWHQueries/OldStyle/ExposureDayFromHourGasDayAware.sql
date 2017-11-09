DECLARE @workspace varchar(255)
DECLARE @monitor varchar(255)
DECLARE @reportDate varchar(255)

set @workspace='{workspace}'
set @monitor='{monitor}'
set @reportDate='{reportdate}'

SELECT
	  [ExposurePricebasis],
	  [QuantityUnit],
	  CAST(DATEADD(HOUR, CASE WHEN [Commodity]='Gas' THEN -6 ELSE 0 END, [FromTime]) AS DATE) as FromDate,
	  DATEADD(d,1,CAST(DATEADD(HOUR, CASE WHEN [Commodity]='Gas' THEN -6 ELSE 0 END, [FromTime]) AS DATE)) as UntilDate,
	  SUM([Exposure]) AS BaseExposure,
	  SUM([PeakExposure]) AS PeakExposure,
	  SUM([OffPeakExposure]) AS OffPeakExposure,
      AVG([Price]) AS BasePrice,
      AVG([PeakPrice]) AS PeakPrice,
      AVG([OffPeakrice]) AS OffPeakPrice

  FROM [VizExposureData]
  WHERE [ReportId]=(
						SELECT [ReportId] FROM [VizExposureReports] 
						WHERE [Workspace]=@workspace
						AND [MonitorTitle]=@monitor
						AND [ReportDate]=@reportDate
					)
  GROUP BY [ExposurePricebasis],
  CAST(DATEADD(HOUR, CASE WHEN [Commodity]='Gas' THEN -6 ELSE 0 END, [FromTime]) AS DATE),
  [QuantityUnit]
 ORDER BY [ExposurePricebasis],FromDate,UntilDate,[QuantityUnit]

