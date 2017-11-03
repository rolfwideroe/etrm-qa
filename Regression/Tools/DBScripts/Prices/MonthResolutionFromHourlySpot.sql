DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE

SET @areaName='N2EX SYSTEM'
SET @fromDate='2013-12-01'
SET @toDate='2013-12-31'



select
year(tradedate) as YearNumber,datepart(M,tradedate) as MonthNumber, avg(itemvalue) as BaseValue ,min(tradedate)as StartOfMonth,max(tradedate) as EndOfMonth,COUNT(itemvalue) as NumberOfHours

 from ECMAreaPricesDetails
where AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2




group by year(tradedate), datepart(M,tradedate)
order by min(tradedate)


select
year(tradedate) as YearNumber,datepart(M,tradedate) as MonthNumber, avg(itemvalue) as PeakValue ,min(tradedate)as StartOfMonth,max(tradedate) as EndOfMonth,COUNT(itemvalue) as NumberOfHours

 from ECMAreaPricesDetails
where AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2

and not DATEPART(dw,tradedate) in (1,7)
and ItemCode>=800
and ItemCode<2000


group by year(tradedate), datepart(M,tradedate)
order by min(tradedate)
