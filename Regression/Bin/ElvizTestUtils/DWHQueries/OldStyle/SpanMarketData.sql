DECLARE @reportDate DATE
DECLARE @workspaceName VARCHAR(55)
DECLARE @monitorName VARCHAR(55)

SET @reportDate='{reportdate}'
SET @workspaceName='{workspace}'
SET @monitorName='{monitor}'

SELECT 
      [RiskGroup]
      ,[LiquidationValue]
      ,[NettedScenarioRisk]
      ,[GroupRisk]
  FROM [BradySPANMarketData]
  WHERE SPANId=(
  				SELECT r.SPANId FROM BradySPANReportHeader r
				 WHERE r.ReportDate=@reportDate
				 AND r.Workspace=@workspaceName
				 AND r.FormName=@monitorName
				)
  ORDER BY RiskGroup