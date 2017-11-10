
declare @reportId int 
set @reportId=(SELECT ReportID FROM VizTransMonReports r where r.Workspace='el-forward-eur-transmon-eur')

select * from
(
SELECT ExternalId,'KeyValues-'+FilterName as JobDescription,reportdate,'Contract price' as ValueType,Currency+'/'+ priceunit as ValueUnit,price as Value From VizTransMonData
WHERE ReportID=@reportId

union
SELECT ExternalId,'KeyValues-'+FilterName as JobDescription,reportdate,'Contract value' as ValueType,Currency as ValueUnit,-price*Mwh as Value From VizTransMonData
WHERE ReportID=@reportId

union
SELECT ExternalId,'KeyValues-'+FilterName as JobDescription,reportdate,'Market price' as ValueType,Currency+'/'+ priceunit as ValueUnit,MktPrice as Value From VizTransMonData
WHERE ReportID=@reportId

union
SELECT ExternalId,'KeyValues-'+FilterName as JobDescription,reportdate,'MtM' as ValueType,Currency as ValueUnit,MtMValue as Value From VizTransMonData
WHERE ReportID=@reportId

union
SELECT ExternalId,'KeyValues-'+FilterName as JobDescription,reportdate,'Remaining volume' as ValueType,priceunit as ValueUnit,Remhours*Volume as Value From VizTransMonData
WHERE ReportID=@reportId

union
SELECT ExternalId,'KeyValues-'+FilterName as JobDescription,reportdate,'Total volume' as ValueType,priceunit as ValueUnit,MWh as Value From VizTransMonData
WHERE ReportID=@reportId
) a

order by ValueType,ExternalId

