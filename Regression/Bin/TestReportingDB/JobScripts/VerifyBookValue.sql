DECLARE @reportDate DATE
set @reportDate='2015-11-25'

DECLARE @jobExecutionId INT
set @jobExecutionId='176'


select 

c.ExternalId as ExternalId
,tsVal.FromDateTime as FromDateTime
,tsVal.FromDateTimeUTC as FromDateTimeUTC
,ts.Resolution as Resolution
,tsVal.LoadHours as LoadHours
,ts.TimeZoneName as TimeZoneName
,ts.LoadType as LoadType
,ts.Unit as ValueType
,convert(varchar(25), NULL) as GreekPricebasis
,ts.Unit as ValueUnit
,tsVal.[Value] as TimeSerieValue


from TimeSeries ts
join TimeSeriesSets tsSets
on ts.TimeSeriesSetId=tsSets.TimeSeriesSetId

join Contracts c
on c.ContractId=ts.ContractId

join TimeSeriesValues tsVal 
on ts.TimeSeriesId=tsVal.TimeSeriesId

  AND tsVal.[Value]<>0
  AND tsSets.TimeSeriesSetId=(
	              SELECT s.TimeSeriesSetId FROM TimeSeriesSets s
								
								Where s.ReportDate=@reportDate
								And s.JobExecutionId=@jobExecutionId
								)


order by 

 ExternalId
 ,Resolution
 ,ValueType
 ,LoadType
 ,GreekPricebasis
 ,ValueUnit
 ,FromDateTimeUTC
 ,FromDateTime


	
