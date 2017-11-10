DECLARE @reportDate DATE
set @reportDate='2011-11-01'

DECLARE @volumeJob Varchar(255)
set @volumeJob='volume-emission-Future-ICE-EUR'


select 

c.ExternalId as ExternalId
,tsVal.FromDateTime as FromDateTime
,tsVal.FromDateTimeUTC as FromDateTimeUTC
,ts.Resolution as Resolution
,tsVal.LoadHours as LoadHours
,ts.TimeZoneName as TimeZoneName
,ts.LoadType as LoadType
,'Contract Price' as ValueType
,convert(varchar(25), NULL) as GreekPricebasis
,c.Currency as ValueUnit
,c.price as TimeSerieValue



from TimeSeries ts
join TimeSeriesSets tsSets
on ts.TimeSeriesSetId=tsSets.TimeSeriesSetId

join ContractExports c
on c.ContractExportId=ts.ContractExportId

join TimeSeriesValues tsVal 
on ts.TimeSeriesId=tsVal.TimeSeriesId

  where tsVal.[Value]<>0
  AND tsSets.TimeSeriesSetId=(
	              SELECT top 1 s.TimeSeriesSetId FROM TimeSeriesSets s
								
								where s.JobName=@volumeJob
								)

  and ts.LoadType='Base'
order by 

 ExternalId
 ,Resolution
 ,ValueType
 ,LoadType
 ,GreekPricebasis
 ,ValueUnit
 ,FromDateTimeUTC
 ,FromDateTime


	
