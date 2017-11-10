DECLARE @workSpace VARCHAR(255)
DECLARE @monitor VARCHAR(255)
DECLARE @reportDate DATE

SET @workSpace='{workspace}'
SET @monitor='{monitor}'
SET @reportDate='{reportdate}'

SELECT
      [SimulationNum]
      ,[FromDate]
      ,[ToDate]
      ,[CashFlow]
      ,[NetVolume]
      ,[Production]
      ,[PortfolioId]
      ,[PortfolioName]
  FROM [BradyCFaRMonData] d
  WHERE PortfolioName='Total'
  AND ReportID = (
					SELECT r.ReportID FROM BradyCFaRMonReports r
					WHERE r.MonitorTitle=@monitor
					AND r.Workspace=@workSpace
					AND r.ReportDate=@reportDate
					
					)
   ORDER BY d.SimulationNum,FromDate
