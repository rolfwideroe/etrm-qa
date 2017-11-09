SELECT     
      [ExposureAgainstCurrency]
      ,[Date]
      ,SUM([CurrencyExposure]) AS AggregatedCurrencyExposure
      
  FROM [VizCurrExposureData]
  WHERE [ReportId]=(
						SELECT [ReportId] FROM [VizCurrExposureReports]
						WHERE [Workspace]='{workspace}'
						AND [MonitorTitle]='{monitor}'
						AND [ReportDate]='{reportdate}'
					)
  GROUP BY [ExposureAgainstCurrency],[Date]
  ORDER BY [ExposureAgainstCurrency],[Date]