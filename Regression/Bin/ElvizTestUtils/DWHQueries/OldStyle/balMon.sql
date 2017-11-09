DECLARE @workspace VARCHAR(255)
DECLARE @monitor VARCHAR(255)
DECLARE @reportDate DATE

SET @workspace='{workspace}'
SET @monitor='{monitor}'
SET @reportDate='{reportdate}'

SELECT 
      [Period]
      ,[From] As FromDate
      ,[To] AS ToDate
      ,[Hours]
      ,[PeakFlag]
      ,[GenconMWh]
      ,[GenConMW]
      ,[GenconPrice]
      ,[GenconValue]
      ,[RetailMWh]
      ,[RetailMW]
      ,[RetailPrice]
      ,[RetailValue]
      ,[PhysMWh]
      ,[PhysMW]
      ,[PhysPrice]
      ,[PhysValue]
      ,[FinMWh]
      ,[FinMW]
      ,[FinPrice]
      ,[FinValue]
      ,[NetMWh]
      ,[NetMW]
      ,[PeakNetMWh]
      ,[PeakNetMW]
      ,[OffPeakNetMWh]
      ,[OffPeakNetMW]
      ,[AvgPrice]
      ,[NetResult]
      ,[MarketPrice]
      ,[Peakprice]
      ,[PeakHours]
      ,[OffPeakPrice]
      ,[PeakGenConMW]
      ,[OffPeakGenConMW]
      ,[PeakRetailMW]
      ,[OffPeakRetailMW]
      ,[PeakPhysMW]
      ,[OffPeakPhysMW]
      ,[PeakFinMW]
      ,[OffPeakFinMW]
      ,[AvgPurchPrice]
      ,[AvgPurchPricePeak]
      ,[AvgPurchPriceOffPeak]
      ,[RetailLatestForecastMwh]
  FROM [VizBalMonData]
   WHERE [VizBalMonData].[ReportID]=(
										SELECT [VizBalMonReports].[ReportID]
										FROM [VizBalMonReports] 
										WHERE [VizBalMonReports].[Workspace]=@workspace
										AND [VizBalMonReports].[MonitorTitle]=@monitor
										AND [VizBalMonReports].[Reportdate]=@reportDate
									)