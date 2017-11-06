DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE

SET @areaName='SP1'
SET @fromDate='2011-01-01'
SET @toDate='2011-11-01'



select
year(tradedate) as YearNumber,datepart(Q,tradedate) as QuarterNumber, avg(itemvalue) as BaseValue ,min(tradedate)as StartOfQuarter,max(tradedate) as EndOfQuarter,COUNT(itemvalue) as NumberOfHours

from ECMAreaPricesDetails
where AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2


group by year(tradedate), datepart(Q,tradedate)
order by min(tradedate)


select
year(tradedate) as YearNumber,datepart(Q,tradedate) as QuarterNumber, avg(itemvalue) as PeakValue ,min(tradedate)as StartOfQ,max(tradedate) as EndOfQ,COUNT(itemvalue) as NumberOfHours

 from ECMAreaPricesDetails
where AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2

and not DATEPART(dw,tradedate) in (1,7)
and ItemCode>=800
and ItemCode<2000


group by year(tradedate), datepart(Q,tradedate)
order by min(tradedate)
