DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE

SET @areaName='N2EX SYSTEM'
SET @fromDate='2013-12-01'
SET @toDate='2013-12-31'



select
year(tradedate) as YearNumber,datepart(M,tradedate) as MonthNumber, avg(AvgPrice) as BaseValue ,min(tradedate)as StartOfMonth,max(tradedate) as EndOfMonth,COUNT(AvgPrice) as NumberOfDays

 from ECMAreaPrices
where AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2




group by year(tradedate), datepart(M,tradedate)
order by min(tradedate)


select
year(tradedate) as YearNumber,datepart(M,tradedate) as MonthNumber, avg(AvgPeak) as PeakValue ,min(tradedate)as StartOfMonth,max(tradedate) as EndOfMonth,COUNT(AvgPeak) as NumberOfDays

 from ECMAreaPrices
where AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2

and not DATEPART(dw,tradedate) in (1,7)



group by year(tradedate), datepart(M,tradedate)
order by min(tradedate)
