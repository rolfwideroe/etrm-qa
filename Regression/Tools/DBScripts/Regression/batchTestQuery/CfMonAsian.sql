
			DECLARE @sqlCashFlowTable NVARCHAR(2048)
			DECLARE @cashFlowTableName NVARCHAR(50)
			DECLARE @monitorName NVARCHAR(255) 
			DECLARE @workspaceName NVARCHAR(255) 
			DECLARE @reportDate NVARCHAR(255)

			SET @monitorName= 'transmon-fixture'
			SET @workspaceName= 'el-asian-nok-actualcf-eur-mtm'
			SET @reportDate='2011-11-01'

			SET @cashFlowTableName= (SELECT [TableName] FROM [VizCFMonSettings]
										WHERE [FormName]=@workspaceName
										AND [WorkspaceName]=@workspaceName
										AND [ReportDate]=@reportDate)
	print @cashFlowTableName

			DECLARE @cashFlowTable TABLE(
										   [CF_TransId] INT	
										  ,[CF_Date] DATETIME
										  ,[CF_ContractType]  INT
										  ,[CF_ValueType] VARCHAR(2)
										  ,[CF_PortfolioId] INT
										  ,[CF_Currency] VARCHAR(3)
										  ,[CF_Value] FLOAT
										  ,[CF_Resolution] VARCHAR(30)
										  ,[CF_Instrumenttype] VARCHAR(60)
										  ,[CF_Instrumentname]VARCHAR(255)
										)
			
			SET @sqlCashFlowTable=N'(SELECT [CF_TransId]
											  ,[CF_Date]
											  ,[CF_ContractType]
											  ,[CF_ValueType]
											  ,[CF_PortfolioId] 
											  ,[CF_Currency] 
											  ,[CF_Value]
											  ,[CF_Resolution]
											  ,[CF_Instrumenttype]
											  ,[CF_Instrumentname] FROM ['+@cashFlowTableName+'])'

			INSERT INTO @cashFlowTable ([CF_TransId]
											  ,[CF_Date]
											  ,[CF_ContractType]
											  ,[CF_ValueType]
											  ,[CF_PortfolioId] 
											  ,[CF_Currency] 
											  ,[CF_Value]
											  ,[CF_Resolution]
											  ,[CF_Instrumenttype]
											  ,[CF_Instrumentname]) EXEC sp_executesql @sqlCashFlowTable
			
			
				DECLARE @cashFlowAsian TABLE(
										  
										  CashFlowDate DATETIME
										  ,InstrumentName  VARCHAR(255)
										  ,CashFlow FLOAT
										  ,ExtId VARCHAR(255)
										  ,PutCall VARCHAR(60)
										  ,TotalHours int
										  ,PriceBasis VARCHAR(60)
										  ,LoadProfile VARCHAR(60)
										  
			
											)

				insert into @cashFlowAsian(
											CashFlowDate 
										  ,InstrumentName  
										  ,CashFlow 
										  ,ExtId 
										  ,PutCall 
										  ,TotalHours,PriceBasis,LoadProfile
				)

						SELECT [CF_Date]
							  ,[CF_Instrumentname]
							  ,CF_Value
							  ,TransMon.ExternalId
							  ,TransMon.PutCall
							  ,TransMon.MWh
							  ,TransMon.PriceBasis
							  ,TransMon.LoadProfile
							  
						

						  FROM @cashFlowTable AS c
						  left join 
						  							  (
									SELECT * FROM [VizTransMonData] AS TransactionMonitor
										  WHERE TransactionMonitor.ReportID=
														(
															SELECT [ReportID] FROM [VizTransMonReports]
															WHERE
															[Workspace]=@workspaceName
															AND
															[MonitorTitle]=@monitorName
															AND
															[ReportDate]=@reportDate
														)
								
								) as TransMon
						  on TransMon.Transid=c.CF_TransId
					
							
						  
						  
						  
						  
						  
						  where c.CF_Date<'2011-11-01'
						  and CF_Instrumentname like '%settlement%'
							  ORDER BY ExternalId, CF_Date,CF_ContractType,CF_ValueType,CF_PortfolioId,CF_Currency,CF_Resolution,CF_Instrumentname
							  
							
							  
		declare @currencySourceId int set @currencySourceId=2

declare @fromDate Date
Declare @toDate Date

set @fromDate='2010-01-01'
set @toDate='2011-11-30'

declare @FloatTable Table(
							FloatingPrice Float
							,FromDate Date
							,ToDate Date
)

Insert INTO @FloatTable (FloatingPrice,FromDate,ToDate)

select

avg(p.ItemValue*n.Rate*e.Rate) FloatingPrice 
, CAST(
      CAST(year(p.TradeDate) AS VARCHAR(4)) +
      RIGHT('0' + CAST(month(TradeDate) AS VARCHAR(2)), 2) +
      RIGHT('0' + CAST(1 AS VARCHAR(2)), 2) 
   AS DATETIME) as FromDate ,max(tradedate)ToDate
from QAPrices_Reg132.dbo.ECMAreaPricesDetails p

left join (select * from QAPrices_Reg132.dbo.CurrencyRates
where CurrencyPeriodId=1
and RateDate>=@fromDate
and RateDate<=@toDate
and CurrencySourceId=@currencySourceId
and CurrencyId = 1) n

on n.RateDate=p.TradeDate

left join (select * from QAPrices_Reg132.dbo.CurrencyRates
where CurrencyPeriodId=1
and RateDate>=@fromDate
and RateDate<=@toDate 
and CurrencySourceId=@currencySourceId
and CurrencyId = 4) e

on e.RateDate=p.TradeDate

where p.AreaId in (select AreaId from QAPrices_Reg132.dbo.Areas where AreaName in('sp1'))
and TypeId=2
and TradeDate>=@fromDate
and TradeDate<=@toDate
and not DATEPART(dw,tradedate) in (1,7)
and ItemCode>=800
and ItemCode<2000


group by year(tradedate),month(tradedate)
order by year(tradedate),month(tradedate)


select *,a.CashFlow-(f.FloatingPrice-335)*a.TotalHours from @cashFlowAsian a
left join @FloatTable f
on f.FromDate=a.CashFlowDate

where a.PutCall = 'CALL'
and a.PriceBasis='NordPool System Price'
and a.LoadProfile='Peak'

select * from @cashFlowAsian
select * from @FloatTable