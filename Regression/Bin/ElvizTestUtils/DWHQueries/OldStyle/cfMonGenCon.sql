
			DECLARE @sqlCashFlowTable NVARCHAR(2048)
			DECLARE @cashFlowTableName NVARCHAR(50)
			DECLARE @monitorName NVARCHAR(255) 
			DECLARE @workspaceName NVARCHAR(255) 
			DECLARE @reportDate NVARCHAR(255)

			SET @monitorName= '{monitor}'
			SET @workspaceName= '{workspace}'
			SET @reportDate='{reportdate}'


			SET @cashFlowTableName= (SELECT [TableName] FROM [VizCFMonSettings]
										WHERE [FormName]=@workspaceName
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
									SELECT ExternalId FROM [VizGenConMonData] AS GenConMonitor
										  WHERE GenConMonitor.ReportID=
														(
															SELECT [ReportID] FROM [VizGenConMonReports]
															WHERE
															[Workspace]=@workspaceName
															AND
															[MonitorTitle]=@monitorName
															AND
															[ReportDate]=@reportDate
														)
										  AND GenConMonitor.ContractId=c.CF_TransId
								) as ExternalId
						  FROM @cashFlowTable AS c
  
					
					SELECT 
					[CF_Date] as CashFlowDate
					,[CF_ContractType] AS ContractType
					,[CF_ValueType] AS ValueType
					,[CF_PortfolioId] As PortfolioId
					,[CF_Currency] as Currency
					,[CF_Value] as CashFlowValue
					,[CF_Resolution] AS Resolution
					,[CF_Instrumenttype] as InstrumentType
					,[CF_Instrumentname] as InstrumentName
					,ExternalId
					 FROM @finTable f
					ORDER BY f.ExternalId, f.CF_Date,CF_ContractType,f.CF_ValueType,f.CF_PortfolioId,f.CF_Currency,f.CF_Resolution,f.CF_Instrumentname
				