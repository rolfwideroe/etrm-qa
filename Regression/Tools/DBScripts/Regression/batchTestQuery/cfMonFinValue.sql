
			DECLARE @sqlCashFlowTable NVARCHAR(2048)
			DECLARE @cashFlowTableName NVARCHAR(50)
			DECLARE @monitorName NVARCHAR(255) 
			DECLARE @workspaceName NVARCHAR(255) 
			DECLARE @reportDate NVARCHAR(255)

			SET @monitorName= 'transmon-fixture'
			SET @workspaceName= 'el-future-n2ex-gbp-actualcf-nok'
			SET @reportDate='2014-01-06'


			SET @cashFlowTableName= (SELECT [TableName] FROM [VizCFMonSettings]
										WHERE [FormName]=@workspaceName
										AND [WorkspaceName]=@workspaceName
										AND [ReportDate]=@reportDate)
	print @cashFlowTableName

			DECLARE @cashFlowTable TABLE(
										   [CF_TransId] INT	
										  ,[CF_Date] DATETIME
										  ,[CF_ContractType]  INT
										  ,[CF_ValueType] VARCHAR(2)
										  ,[CF_PortfolioId] INT
										  ,[CF_Currency] VARCHAR(3)
										  ,[CF_Value] FLOAT
										  ,[CF_Resolution] VARCHAR(30)
										  ,[CF_Instrumenttype] VARCHAR(60)
										  ,[CF_Instrumentname]VARCHAR(255)
										)
			
			SET @sqlCashFlowTable=N'(SELECT [CF_TransId]
											  ,[CF_Date]
											  ,[CF_ContractType]
											  ,[CF_ValueType]
											  ,[CF_PortfolioId] 
											  ,[CF_Currency] 
											  ,[CF_Value]
											  ,[CF_Resolution]
											  ,[CF_Instrumenttype]
											  ,[CF_Instrumentname] FROM ['+@cashFlowTableName+'])'

			INSERT INTO @cashFlowTable ([CF_TransId]
											  ,[CF_Date]
											  ,[CF_ContractType]
											  ,[CF_ValueType]
											  ,[CF_PortfolioId] 
											  ,[CF_Currency] 
											  ,[CF_Value]
											  ,[CF_Resolution]
											  ,[CF_Instrumenttype]
											  ,[CF_Instrumentname]) EXEC sp_executesql @sqlCashFlowTable
			
			

declare  @finTable table
						(
						      [CF_Date] DATE
							  ,[CF_ContractType] INT 
							  ,[CF_ValueType] VARCHAR(25)
							  ,[CF_PortfolioId] INT
							  ,[CF_Currency] VARCHAR(25)
							  ,[CF_Value] FLOAT
							  ,[CF_Resolution] VARCHAR(25)
							  ,[CF_Instrumenttype] VARCHAR(255)
							  ,[CF_Instrumentname] VARCHAR(255)
							  ,ExternalId VARCHAR(255)
						)

INSERT INTO  @finTable
						SELECT [CF_Date]
							  ,[CF_ContractType]
							  ,[CF_ValueType]
							  ,[CF_PortfolioId]
							  ,[CF_Currency]
							  ,[CF_Value]
							  ,[CF_Resolution]
							  ,[CF_Instrumenttype]
							  ,[CF_Instrumentname]
							  ,(
									SELECT ExternalId FROM [VizTransMonData] AS TransactionMonitor
										  WHERE TransactionMonitor.ReportID=
														(
															SELECT [ReportID] FROM [VizTransMonReports]
															WHERE
															[Workspace]=@workspaceName
															AND
															[MonitorTitle]=@monitorName
															AND
															[ReportDate]=@reportDate
														)
										  AND TransactionMonitor.Transid=c.CF_TransId
								) as ExternalId
						  FROM @cashFlowTable AS c
						--where c.CF_Value<>0
					--	where c.cf_date='2011-11-01'
					
					--where c.CF_Instrumentname like '%premium%'
					--		  ORDER BY ExternalId, CF_Date,CF_ContractType,CF_ValueType,CF_PortfolioId,CF_Currency,CF_Resolution,CF_Instrumentname
							  
							  
					--select p.,SUM(p.CF_Value) from @cashFlowTable p 
					--where	p.CF_Instrumentname like '%premium%'	  
					
					SELECT * FROM @finTable f
					ORDER BY f.ExternalId, f.CF_Date,CF_ContractType,f.CF_ValueType,f.CF_PortfolioId,f.CF_Currency,f.CF_Resolution,f.CF_Instrumentname
					--WHERE CF_Instrumentname like '%premium%'
					--SELECT F.ExternalId,SUM(F.CF_Value) FROM @finTable f
					--WHERE F.CF_Instrumentname like '%premium%'
					--GROUP BY F.ExternalId
					--ORDER BY F.ExternalId