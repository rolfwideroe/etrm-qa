
DECLARE @workSpace VARCHAR(255)
DECLARE @reportDate DATE

SET @workSpace='{workspace}'
SET @reportDate='{reportdate}'

SELECT
      Reports.MonitorTitle
      ,Data.StepV
      ,SUM(Data.MarketValue) as MtM
      ,SUM(Data.Profitloss) as PL
    
  FROM VizSensAnalysisData Data
  LEFT JOIN VizSensAnalysisSettings Settings on Data.ReportId=Settings.Reportid
  LEFT JOIN VizSensAnalysisReports Reports on Reports.ReportID=Settings.Reportid
  
  WHERE Data.ReportId IN
				   (
						SELECT ws.ReportID from VizSensAnalysisReports ws
						WHERE ws.Workspace=@workSpace
						AND ws.ReportDate=@reportDate
					)
   
  AND Data.StepH=0

  GROUP BY Reports.MonitorTitle,Data.StepV
  ORDER BY Reports.MonitorTitle,Data.StepV