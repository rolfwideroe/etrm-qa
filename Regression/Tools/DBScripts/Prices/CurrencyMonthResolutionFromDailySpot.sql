DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE
DECLARE @fromCurrency VARCHAR(5)
DECLARE @toCurrency VARCHAR (5)

SET @areaName='ICE Brent Index'
SET @fromDate='2010-11-01'
SET @toDate='2010-12-31'


set @fromCurrency='USD'
set @toCurrency='NOK'

select
year(p.tradedate) as YearNumber,datepart(M,p.tradedate) as MonthNumber, avg(p.AvgPrice*c1.Rate*c2.Rate) as BaseValue ,min(p.tradedate)as StartOfMonth,max(p.tradedate) as EndOfMonth,COUNT(p.AvgPrice) as NumberOfDays
from ECMAreaPrices p
join CurrencyRates c1
on c1.RateDate=p.TradeDate

join CurrencyRates c2
on c2.RateDate=p.TradeDate

where 
c1.CurrencyId=(SELECT CurrencyId FROM Currencies where ISO_Currency=@fromCurrency)
and c1.CurrencyPeriodId=1
and c1.CurrencySourceId=1

and c2.CurrencyId=(SELECT CurrencyId FROM Currencies where ISO_Currency=@toCurrency)
and c2.CurrencyPeriodId=1
and c2.CurrencySourceId=1

and AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2

--and not DATEPART(dw,tradedate) in (1,7)

group by year(tradedate), datepart(M,tradedate)
order by min(tradedate)


select
year(p.tradedate) as YearNumber,datepart(M,p.tradedate) as MonthNumber, avg(p.AvgPrice*c1.Rate*c2.Rate) as PeakValue ,min(p.tradedate)as StartOfMonth,max(p.tradedate) as EndOfMonth,COUNT(p.AvgPrice) as NumberOfDays
from ECMAreaPrices p
join CurrencyRates c1
on c1.RateDate=p.TradeDate

join CurrencyRates c2
on c2.RateDate=p.TradeDate

where 
c1.CurrencyId=(SELECT CurrencyId FROM Currencies where ISO_Currency=@fromCurrency)
and c1.CurrencyPeriodId=1
and c1.CurrencySourceId=1

and c2.CurrencyId=(SELECT CurrencyId FROM Currencies where ISO_Currency=@toCurrency)
and c2.CurrencyPeriodId=1
and c2.CurrencySourceId=1

and AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2

and not DATEPART(dw,tradedate) in (1,7)

group by year(tradedate), datepart(M,tradedate)
order by min(tradedate)