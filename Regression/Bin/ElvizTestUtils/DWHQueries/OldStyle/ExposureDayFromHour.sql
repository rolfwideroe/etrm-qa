DECLARE @workspace varchar(255)
DECLARE @monitor varchar(255)
DECLARE @reportDate varchar(255)

set @workspace='{workspace}'
set @monitor='{monitor}'
set @reportDate='{reportdate}'

SELECT  
	  [ExposurePricebasis]
	  ,[QuantityUnit]
	    ,CAST(CAST(Year([FromTime]) AS varchar) + '-' + CAST(Month([FromTime]) AS varchar) + '-' + CAST(Day([FromTime]) AS varchar) AS DATETIME) as FromDate
		,DATEADD(d,1,CAST(CAST(Year([FromTime]) AS varchar) + '-' + CAST(Month([FromTime]) AS varchar) + '-' + CAST(Day([FromTime]) AS varchar) AS DATETIME)) as UntilDate
	  ,SUM([Exposure]) AS BaseExposure
      ,SUM([PeakExposure]) AS PeakExposure
      ,SUM([OffPeakExposure]) AS OffPeakExposre

  FROM [VizExposureData]
  WHERE [ReportId]=(
						SELECT [ReportId] FROM [VizExposureReports] 
						WHERE [Workspace]=@workspace
						AND [MonitorTitle]=@monitor
						AND [ReportDate]=@reportDate
						AND [InstallationId] = (select InstId from Installation where InstallationName = '{installationName}')
					)
  GROUP BY [ExposurePricebasis]
  ,Year([FromTime])
  ,Month([FromTime])
  ,Day([FromTime])
  ,[QuantityUnit]
 ORDER BY [ExposurePricebasis],FromDate,UntilDate,[QuantityUnit]

