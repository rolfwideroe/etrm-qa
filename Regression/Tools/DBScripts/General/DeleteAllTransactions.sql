Delete from Transaction_PropertyStrings

Delete from Transaction_PropertyIntegers

Delete from Transaction_PropertyFloats 

Delete from Transaction_PropertyDates

Delete from Transaction_PropertyBools 

Delete from TransactionFees 

Delete from BinaryDeclarations

Delete from ContractSplits

Delete from DailyCashFlows

Delete from DailyVolumes 

Delete from TransactionCascadingRoleMappings 

Delete from TransactionCascadings 

Delete from TransactionFees 

Delete from TransactionPeriodData

Delete from TransactionPeriodAggregatedData

Delete from AuditTrail
where Object = 'Transaction' 

Delete from ChangeApproval
where Entity = 'Trans'

Delete from DED_Details 

Delete from DE_Delivery

delete from TransactionElcertificateVersionedQuotaSchemes

delete from TransactionGenConLinks

delete from TransactionTimeSeriesLinks

Delete from Transactions



declare @maxid int
set @maxid=0
DBCC CHECKIDENT (Transactions, reseed, @maxid)
