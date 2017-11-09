			DECLARE @sqlCashFlowTable NVARCHAR(2048)
			DECLARE @cashFlowTableName NVARCHAR(50)
			DECLARE @monitorName NVARCHAR(255) 
			DECLARE @workspaceName NVARCHAR(255) 
			DECLARE @reportDate NVARCHAR(255)

			SET @workspaceName= '{workspace}'
			SET @monitorName= '{monitor}'
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
			
						DECLARE @finalCashFlowTable TABLE(
										 [CF_Date] DATETIME
										  ,[CF_ContractType]  INT
										  ,[CF_ValueType] VARCHAR(2)
										  ,[CF_PortfolioId] INT
										  ,[CF_Currency] VARCHAR(3)
										  ,[CF_Value] FLOAT
										  ,[CF_Resolution] VARCHAR(30)
										  ,[CF_Instrumenttype] VARCHAR(60)
										  ,[CF_Instrumentname]VARCHAR(255)
										  ,[ExternalId] VARCHAR(255) NOT NULL
										)


				
				INSERT INTO @finalCashFlowTable
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
						  

				SELECT *
				------For Creating Test files---------------
				/*
					  ,'<record>'+CONVERT(VARCHAR,f.CF_Date,121)+','
	                  +CAST(f.CF_ContractType AS VARCHAR)+','
	                  +f.CF_ValueType+','
					  +CAST(f.CF_PortfolioId AS VARCHAR)+','
					  +f.CF_Currency+','
					  +FORMAT(f.CF_Value,'G')+','
					  +f.CF_Resolution+','
					  +f.CF_Instrumenttype+','
					  +f.CF_Instrumentname+','
					  +f.ExternalId+'</record>' AS XmlColumn
				*/
				------------------------------------------
				 FROM @finalCashFlowTable f
				ORDER BY f.ExternalId, f.CF_Date,f.CF_ContractType,f.CF_ValueType,f.CF_PortfolioId,f.CF_Currency,f.CF_Resolution,f.CF_Instrumentname