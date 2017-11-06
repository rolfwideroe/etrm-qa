DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE
DECLARE @fromCurrency VARCHAR(5)
DECLARE @toCurrency VARCHAR (5)

SET @areaName='EEX SYSTEM'
SET @fromDate='2011-10-03'
SET @toDate='2012-12-30'


set @fromCurrency='EUR'
set @toCurrency='EUR'

------------------------------------------------------------------

select
year(b.tradedate) as YearNumber,datepart(ISOWK,b.TradeDate) as WeekNumber, avg(b.Base) as BaseValue ,min(b.TradeDate)as StartOfWeek,max(b.TradeDate) as EndOfWeek,COUNT(b.Base) as NumberOfHours

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

group by year(b.TradeDate), datepart(ISOWK,b.tradedate)
order by min(b.TradeDate)

--------------------------------------------------------------

select
year(p.tradedate) as YearNumber,datepart(ISOWK,p.TradeDate) as WeekNumber, avg(p.Peak) as PeakValue ,min(p.TradeDate)as StartOfWeek,max(p.TradeDate) as EndOfWeek,COUNT(p.Peak) as NumberOfHours

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

group by year(p.TradeDate), datepart(ISOWK,p.tradedate)
order by min(p.TradeDate)
---------------------------------------------------------------------------
