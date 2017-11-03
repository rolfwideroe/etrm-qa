DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE
DECLARE @fromCurrency VARCHAR(5)
DECLARE @toCurrency VARCHAR (5)

SET @areaName='se3'
SET @fromDate='2010-11-01'
SET @toDate='2010-11-30'


------------------------------------------------------------------

select
p.TradeDate, avg(p.ItemValue) as Base,count(p.ItemValue) as NumHours
from ECMAreaPricesDetails p
where  AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2
group by p.TradeDate
order by p.TradeDate
--and not DATEPART(dw,tradedate) in (1,7)




--------------------------------------------------------------



select
p.TradeDate, avg(p.ItemValue) as Peak,count(p.ItemValue) as NumHours
from ECMAreaPricesDetails p
where  AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2




and not DATEPART(dw,p.tradedate) in (1,7)
and p.ItemCode>=800
and p.ItemCode<=1900
group by p.TradeDate
order by p.TradeDate

---------------------------------------------------------------------------
