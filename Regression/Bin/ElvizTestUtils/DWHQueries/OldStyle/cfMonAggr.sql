			DECLARE @sqlCashFlowTable NVARCHAR(2048)
			DECLARE @cashFlowTableName NVARCHAR(50)
			DECLARE @monitorName NVARCHAR(255) 
			DECLARE @workspaceName NVARCHAR(255) 
			DECLARE @reportDate NVARCHAR(255)

			SET @monitorName= '{monitor}'
			SET @workspaceName= '{workspace}'
			SET @reportDate='{reportdate}'


			SET @cashFlowTableName= (SELECT [TableName] FROM [VizCFMonSettings]
										WHERE [FormName]=@monitorName
										AND [WorkspaceName]=@workspaceName
										AND [ReportDate]=@reportDate)


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
			
			




							SELECT [CF_Date] as CashFlowDate
								,[CF_ContractType] AS ContractType
								,[CF_ValueType] AS ValueType
								,[CF_PortfolioId] As PortfolioId
								,[CF_Currency] as Currency
								,[CF_Value] as CashFlowValue
								,[CF_Resolution] AS Resolution
								,[CF_Instrumenttype] as InstrumentType
								,[CF_Instrumentname] as InstrumentName
						  FROM @cashFlowTable AS c
						  ORDER BY CF_PortfolioId,CF_Instrumentname,CF_ContractType,CF_Resolution,CF_ValueType,CF_Currency,CF_Date

