DECLARE @workspace varchar(255)
DECLARE @monitor varchar(255)
DECLARE @reportDate varchar(255)

set @workspace='{workspace}'
set @monitor='{monitor}'
set @reportDate='{reportdate}'

SELECT  
	  [ExposurePricebasis]
      ,YEAR([FromTime]) AS YearNumber
	  ,MONTH([FromTime]) as MonthNumber
	  ,AVG([Price]) AS BasePrice
      ,AVG([PeakPrice]) AS PeakPrice
      ,AVG([OffPeakrice]) AS OffPeakPrice
  FROM [VizExposureData]
  WHERE [ReportId]=(
						SELECT [ReportId] FROM [VizExposureReports] 
						WHERE [Workspace]=@workspace
						AND [MonitorTitle]=@monitor
						AND [ReportDate]=@reportDate
						--AND [InstallationId] = (select InstId from Installation where InstallationName = '{installationName}')
					)
  GROUP BY [ExposurePricebasis],YEAR([FromTime]),MONTH([FromTime]),[QuantityUnit]
  ORDER BY [ExposurePricebasis],YEAR([FromTime]),MONTH([FromTime]),[QuantityUnit]