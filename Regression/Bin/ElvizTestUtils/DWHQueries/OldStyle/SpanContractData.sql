DECLARE @reportDate DATE
DECLARE @workspaceName VARCHAR(55)
DECLARE @monitorName VARCHAR(55)

SET @reportDate='{reportdate}'
SET @workspaceName='{workspace}'
SET @monitorName='{monitor}'

SELECT 
      [ContractName]
      ,[Portfolio]
      ,[Pricebasis]
      ,[Fromdate]
      ,[Todate]
      ,Case [Loadtype]
      WHEN 0 THEN 'Base'
      WHEN 1 THEN 'Peak'
      ELSE Loadtype
      END as LoadType
      ,[Numberofhours]
      ,[Effect]
      ,[ClosingPrice]
      ,[ClosingPriceCurrency]
      ,[LiquidationValue]
  FROM [BradySPANContractData]
  WHERE SPANId=(
				 SELECT r.SPANId FROM BradySPANReportHeader r
				 WHERE r.ReportDate=@reportDate
				 AND r.Workspace=@workspaceName
				 AND r.FormName=@monitorName
				)
ORDER BY Portfolio,Pricebasis,ContractName