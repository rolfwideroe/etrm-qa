DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE
DECLARE @fromCurrency VARCHAR(5)
DECLARE @toCurrency VARCHAR (5)

SET @areaName='sp1'
SET @fromDate='2010-11-01'
SET @toDate='2010-11-30'


------------------------------------------------------------------

select
p.TradeDate,p.ItemCode/100 as HourNumber, p.ItemValue as Base
from ECMAreaPricesDetails p
where  AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2
order by p.TradeDate,p.ItemCode
--and not DATEPART(dw,tradedate) in (1,7)




--------------------------------------------------------------



select
p.TradeDate,p.ItemCode/100 as HourNumber, p.ItemValue as Peak
from ECMAreaPricesDetails p
where  AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2




and not DATEPART(dw,p.tradedate) in (1,7)
and p.ItemCode>=800
and p.ItemCode<=1900
order by p.TradeDate,p.ItemCode

---------------------------------------------------------------------------
