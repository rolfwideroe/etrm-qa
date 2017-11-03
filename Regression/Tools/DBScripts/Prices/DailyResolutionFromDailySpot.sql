DECLARE @areaName VARCHAR(55)
DECLARE @fromDate DATE
DECLARE @toDate DATE

SET @areaName='SE3'
SET @fromDate='2010-11-01'
SET @toDate='2013-12-31'



select
TradeDate,AvgPrice as BaseValue 

 from ECMAreaPrices
where AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2





order by tradedate


select
TradeDate,AvgPeak as PeakValue 

 from ECMAreaPrices
where AreaId=(select AreaId from Areas where AreaName=@areaName)
and TradeDate>=@fromDate
and TradeDate<=@toDate
and TypeId=2

and not DATEPART(dw,tradedate) in (1,7)




order by tradedate
