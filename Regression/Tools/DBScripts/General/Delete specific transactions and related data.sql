--Disclaimer:
--This script removes transactions completely from the database, without leaving a trace in the audit trail.
--Use at own risk, and make sure you have a working (and up to date) backup first!

CREATE TABLE #DeleteTransactions (TransactionId int)

Insert INTO #DeleteTransactions (TransactionId)
Select Id From Transactions Where Id >128
--in 
----Insert list of transactions ids to be deleted here:
----(Note that both originating and referring transaction must be specified)
--(1, 2,...)

Delete from Transaction_PropertyStrings
Where RefId in (select TransactionId From #DeleteTransactions)

Delete from Transaction_PropertyIntegers
Where RefId in (select TransactionId From #DeleteTransactions)

Delete from Transaction_PropertyFloats 
Where RefId in (select TransactionId From #DeleteTransactions)

Delete from Transaction_PropertyDates
Where RefId in (select TransactionId From #DeleteTransactions)

Delete from Transaction_PropertyBools 
Where RefId in (select TransactionId From #DeleteTransactions)

Delete from TransactionFees 
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from BinaryDeclarations
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from ContractSplits
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from DailyCashFlows
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from DailyVolumes 
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from TransactionCascadingRoleMappings 
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from TransactionCascadings 
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from TransactionFees 
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from TransactionPeriodData
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from TransactionPeriodAggregatedData
Where TransactionId in (select TransactionId From #DeleteTransactions)

Delete from AuditTrail
where Object = 'Transaction' 
	and Id in (select TransactionId From #DeleteTransactions)

Delete from ChangeApproval
where Entity = 'Trans'
	and RefId in (select TransactionId From #DeleteTransactions)

Delete from DED_Details 
	where DED_DE_Id in 
	(Select DE_Id 
		From DE_Delivery 
		Where  DE_Delivery.DE_DES_Id = (Select DES_Id FROM DES_Source Where DES_Name ='Transactions')
			And DE_Delivery.DE_ExternalContractKey in (select TransactionId From #DeleteTransactions))

Delete from DE_Delivery
where 
	DE_Delivery.DE_DES_Id = (Select DES_Id FROM DES_Source Where DES_Name ='Transactions')
	And DE_Delivery.DE_ExternalContractKey in (select TransactionId From #DeleteTransactions)

DELETE FROM TransactionGenConLinks WHERE TransactionId IN (select TransactionId From #DeleteTransactions)

DELETE FROM TransactionTimeSeriesLinks WHERE TransactionId IN (select TransactionId From #DeleteTransactions)

DELETE FROM TimeSeriesMetaData

DELETE FROM TimeSeriesVersions

DELETE FROM TimeSeriesVersionRevisions

DELETE FROM [TimeSeriesFixedResolutionValuesHistory]

DELETE FROM TimeSeriesFixedResolutionValues

DELETE FROM [TimeSeriesVersions]

DELETE FROM TimeSeriesRevisions

DELETE FROM TimeSeries

DELETE FROM [TransactionPeriodData] WHERE TRANSACTIONID IN  (select TransactionId From #DeleteTransactions)

DELETE FROM [TransactionPeriodAggregatedData]WHERE TransactionId IN (select TransactionId From #DeleteTransactions)


Delete from Transactions
Where Id in (select TransactionId From #DeleteTransactions)

  delete from TransactionElcertificateVersionedQuotaSchemes
  where TransactionId in (select TransactionId From #DeleteTransactions)


DROP Table #DeleteTransactions 