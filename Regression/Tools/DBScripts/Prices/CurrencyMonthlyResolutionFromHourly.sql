DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE
DECLARE @fromCurrency VARCHAR(5)
DECLARE @toCurrency VARCHAR (5)

SET @areaName='sp1'
SET @fromDate='2010-11-01'
SET @toDate='2010-11-30'


set @fromCurrency='EUR'
set @toCurrency='NOK'

------------------------------------------------------------------

select
year(b.tradedate) as YearNumber,datepart(M,b.TradeDate) as MonthNumber, avg(b.Base) as BaseValue ,min(b.TradeDate)as StartOfMonth,max(b.TradeDate) as EndOfMonth,COUNT(b.Base) as NumberOfHours

from
(
select
p.TradeDate,p.ItemCode/100 as HourNumber, p.ItemValue*c1.Rate*c2.Rate as Base
from ECMAreaPricesDetails p
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
--order by p.TradeDate,p.ItemCode
--and not DATEPART(dw,tradedate) in (1,7)


) b

group by year(b.TradeDate), datepart(M,tradedate)
order by min(b.TradeDate)

--------------------------------------------------------------

select
year(p.tradedate) as YearNumber,datepart(M,p.TradeDate) as MonthNumber, avg(p.Peak) as PeakValue ,min(p.TradeDate)as StartOfMonth,max(p.TradeDate) as EndOfMonth,COUNT(p.Peak) as NumberOfHours

from 
(
select
p.TradeDate,p.ItemCode/100 as HourNumber, p.ItemValue*c1.Rate*c2.Rate as Peak
from ECMAreaPricesDetails p
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




and not DATEPART(dw,p.tradedate) in (1,7)
and p.ItemCode>=800
and p.ItemCode<=1900
--order by p.TradeDate,p.ItemCode
) p

group by year(p.TradeDate), datepart(M,p.tradedate)
order by min(p.TradeDate)
---------------------------------------------------------------------------
