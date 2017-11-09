DECLARE @workspace varchar(255)
DECLARE @monitor varchar(255)
DECLARE @reportDate varchar(255)

set @workspace='{workspace}'
set @monitor='{monitor}'
set @reportDate='{reportdate}'

SELECT  
	  [ExposurePricebasis]
	  ,[QuantityUnit]
      ,YEAR([FromTime]) AS YearNumber
	  ,MONTH([FromTime]) as MonthNumber
	  ,SUM([Exposure]) AS BaseExposure
      ,SUM([PeakExposure]) AS PeakExposure
      ,SUM([OffPeakExposure]) AS OffPeakExposre
  FROM [VizExposureData]
  WHERE [ReportId]=(
						SELECT [ReportId] FROM [VizExposureReports] 
						WHERE [Workspace]=@workspace
						AND [MonitorTitle]=@monitor
						AND [ReportDate]=@reportDate
						--AND [InstallationId] = (select InstId from Installation where InstallationName = '{installationName}') --Need this line when running locally, but it seems to cause error in the automatic test run
					)
  GROUP BY [ExposurePricebasis],YEAR([FromTime]),MONTH([FromTime]),[QuantityUnit]
  ORDER BY [ExposurePricebasis],YEAR([FromTime]),MONTH([FromTime]),[QuantityUnit]