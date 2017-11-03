DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE
DECLARE @fromCurrency VARCHAR(5)
DECLARE @toCurrency VARCHAR (5)
DECLARE @currencySource VARCHAR(25)


SET @areaName='sp1'
SET @fromDate='2010-11-01'
SET @toDate='2010-11-30'


set @fromCurrency='EUR'
set @toCurrency='NOK'
set @currencySource='VIZ'

------------------------------------------------------------------

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
and c1.CurrencySourceId=(Select CurrencySourceId FROM CurrencySources WHERE CurrencySourceName=@currencySource)

and c2.CurrencyId=(SELECT CurrencyId FROM Currencies where ISO_Currency=@toCurrency)
and c2.CurrencyPeriodId=1
and c2.CurrencySourceId=(Select CurrencySourceId FROM CurrencySources WHERE CurrencySourceName=@currencySource)

and AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2
order by p.TradeDate,p.ItemCode
--and not DATEPART(dw,tradedate) in (1,7)




--------------------------------------------------------------



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
and c1.CurrencySourceId=(Select CurrencySourceId FROM CurrencySources WHERE CurrencySourceName=@currencySource)

and c2.CurrencyId=(SELECT CurrencyId FROM Currencies where ISO_Currency=@toCurrency)
and c2.CurrencyPeriodId=1
and c2.CurrencySourceId=(Select CurrencySourceId FROM CurrencySources WHERE CurrencySourceName=@currencySource)

and AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2




and not DATEPART(dw,p.tradedate) in (1,7)
and p.ItemCode>=800
and p.ItemCode<=1900
order by p.TradeDate,p.ItemCode

---------------------------------------------------------------------------
