
			DECLARE @sqlCashFlowTable NVARCHAR(2048)
			DECLARE @cashFlowTableName NVARCHAR(50)
			DECLARE @monitorName NVARCHAR(255) 
			DECLARE @workspaceName NVARCHAR(255) 
			DECLARE @reportDate NVARCHAR(255)

			SET @monitorName= 'transmon-fixture'
			SET @workspaceName= 'Fees-EUR'
			SET @reportDate='2011-11-01'


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
							  ,Portfolio VARCHAR(55)
							  ,[CF_Currency] VARCHAR(25)
							  ,[CF_Value] FLOAT
							  ,[CF_Resolution] VARCHAR(25)
							  ,[CF_Instrumenttype] VARCHAR(255)
							  ,[CF_Instrumentname] VARCHAR(255)
							  ,ExternalId VARCHAR(255)
							  ,BuySell VARCHAR(5)
						)

INSERT INTO  @finTable
						SELECT [CF_Date]
							  ,[CF_ContractType]
							  ,[CF_ValueType]
							  ,TransMon.Portfolio
							  ,[CF_Currency]
							  ,[CF_Value]
							  ,[CF_Resolution]
							  ,[CF_Instrumenttype]
							  ,[CF_Instrumentname]
								,TransMon.ExternalId
								,TransMon.BuySell
						  FROM @cashFlowTable AS c
						  JOIN (
									SELECT ExternalId,BuySell,Transid,Portfolio FROM [VizTransMonData] AS TransactionMonitor
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
										  
								) TransMon
						on c.CF_TransId=TransMon.Transid

					
					SELECT * FROM @finTable f
					ORDER BY f.ExternalId, f.CF_Date,CF_ContractType,f.CF_ValueType,f.Portfolio,f.CF_Currency,f.CF_Resolution,f.CF_Instrumentname,BuySell
