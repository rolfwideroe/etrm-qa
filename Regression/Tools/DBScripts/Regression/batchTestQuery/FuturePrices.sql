declare @venue varchar(55)
declare @area varchar(55)
declare @instrument varchar(55)
declare @type varchar(10)
declare @reportDate date
declare @tradeDate date
declare @contractPrice float
declare @volume float
set @reportDate='2014-01-06'

set @venue='nord pool'
set @area='n2ex system'
set @instrument='eukblmdec-13'
set @type='close'
set @tradeDate='2013-11-01'
set @contractPrice=40.43
set @venue='nord pool'
set @volume=1320

declare @prodId INT 

Set @prodId=(

select p.ProdId from Products p
where p.ExecutionVenueId=(SELECT e.ExecutionVenueId FROM ExecutionVenues e WHERE E.ExecutionVenueName=@venue)
and p.AreaId=(SELECT a.AreaId FROM Areas a where a.AreaName=@area)
and p.ProdName=@instrument
)

declare @typeId INT
set @typeId=(select v.TypeId from ValueTypes v where v.TypeName=@type)

declare @table table (
TradeDate Date,
ClosePrice Float,
PrevClose Float
)

insert into @table

select f.TradeDate, 

f.Value as ClosePrice,
(

select top 1 ff.Value from Prices ff where ff.ProdId=@prodId and ff.TypeId=@typeId and ff.TradeDate<f.TradeDate order by ff.TradeDate desc

) as PrevClosePrice


from Prices f

where f.ProdId =@prodId
and f.TypeId=@typeId
order by f.TradeDate desc

update @table set PrevClose = @contractPrice WHERE TradeDate=@tradeDate

select top 1 * from @table

select YEAR(t.TradeDate),MONTH(t.TradeDate), round(SUM(t.ClosePrice-t.PrevClose)*@volume,2)  from @table t
where t.TradeDate>=@tradeDate
and t.TradeDate<=@reportDate

group by YEAR(t.TradeDate),MONTH(t.TradeDate)
order by YEAR(t.TradeDate),MONTH(t.TradeDate)

--select TradeDate, round((t.ClosePrice-t.PrevClose)*@volume,2)  from @table t
--where t.TradeDate>=@tradeDate
--and t.TradeDate<=@reportDate

