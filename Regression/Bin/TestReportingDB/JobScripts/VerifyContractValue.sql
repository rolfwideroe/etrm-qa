
DECLARE @filterName Varchar(255)
set @filterName='Gas-StructFlex-CapFloor-EUR'

SELECT 
	  con.ExternalId
      ,v.[TimeSerieFromDate] as FromDateTime
      ,v.[TimeSerieFromDateUtc] as FromDateTimeUTC
      ,v.[TimeSerieResolution] as Resolution
      ,v.[TimeSerieLoadHours] as LoadHours
      ,v.[TimeSerieTimeZone] as TimeZoneName
      ,v.[TimeSerieLoad] as LoadType
      --,v.[TimeSerieValueType] as ValueType
	  ,'Contract value' as ValueType
    --  ,v.[TimeSerieValueUnit] 
	  ,'' as GreekPricebasis
	  --,v.[TimeSerieValueUnit] as ValueUnit
	  ,	 con.Currency as ValueUnit
      ,-v.[TimeSerieValue] * cPrice.[TimeSerieValue] as TimeSerieValue
  FROM [QAReporting_Reg161].[dbo].[TimeSeriesView] v
  
  
  join (select * from [TimeSeriesView] c
		 where c.JobExecutionId=
							(
								SELECT top 1 s.JobExecutionId FROM TimeSeriesSets s
								
								where s.Alias='contractPrice-'+@filterName
								)
		) cPrice
   on v.[ContractExportId]=cPrice.[ContractExportId]
	  and v.[TimeSerieFromDateUtc]=cPrice.[TimeSerieFromDateUtc]
	  and v.[TimeSerieResolution]=cPrice.[TimeSerieResolution]
	  and v.[TimeSerieLoad]=cPrice.[TimeSerieLoad]
 
   join ContractExports con on con.ContractExportId=v.ContractExportId
   where v.JobExecutionId=
							(
								SELECT top 1 s.JobExecutionId FROM TimeSeriesSets s
								
								where s.Alias='volume-'+@filtername
								)
  and v.[TimeSerieLoad]='Base'
	order by 
	 ExternalId
 ,Resolution
 ,ValueType
 ,LoadType
 ,GreekPricebasis
 ,ValueUnit
 ,FromDateTimeUTC
 ,FromDateTime