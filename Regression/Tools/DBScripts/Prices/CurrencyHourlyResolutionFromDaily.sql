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

select h.TradeDate,h.HourNumber,cp.Value as BaseFromDaily
from
(select d.TradeDate,d.ItemCode/100 as HourNumber from ECMAreaPricesDetails d
where TradeDate>=@fromDate
and TradeDate<=@toDate
group by d.TradeDate,d.ItemCode) h

join
(
select
p.TradeDate, p.AvgPrice*c1.Rate*c2.Rate as Value
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
) cp

on cp.TradeDate=h.TradeDate
order by h.TradeDate,h.HourNumber

--------------------------------------------------------------


select h.TradeDate,h.HourNumber,cp.Value as PeakFromDaily
from
(select d.TradeDate,d.ItemCode/100 as HourNumber from ECMAreaPricesDetails d
where TradeDate>=@fromDate
and TradeDate<=@toDate
group by d.TradeDate,d.ItemCode) h

join
(
select
p.TradeDate, p.AvgPeak*c1.Rate*c2.Rate as Value
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


) cp

on cp.TradeDate=h.TradeDate

where not DATEPART(dw,h.tradedate) in (1,7)
and h.HourNumber>=8
and h.HourNumber<=19
order by h.TradeDate,h.HourNumber

---------------------------------------------------------------------------
