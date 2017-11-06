
			DECLARE @sqlCashFlowTable20122 NVARCHAR(2048)
			DECLARE @cashFlowTableName20122 NVARCHAR(50)
			DECLARE @monitorName NVARCHAR(255) 
			DECLARE @workspaceName NVARCHAR(255) 
			DECLARE @reportDate NVARCHAR(255)

			DECLARE @sqlCashFlowTable20132 NVARCHAR(2048)
			DECLARE @cashFlowTableName20132 NVARCHAR(50)

			SET @monitorName= 'transmon-fixture'
			SET @workspaceName= 'el-asian-nok-actualpl-nok-delta'
			SET @reportDate='2011-11-01'


			SET @cashFlowTableName20122= (SELECT [TableName] FROM QADatawareHouse_Reg122.dbo.[VizCFMonSettings] 
										WHERE [FormName]=@workspaceName
										AND [WorkspaceName]=@workspaceName
										AND [ReportDate]=@reportDate)

			SET @cashFlowTableName20132= (SELECT [TableName] FROM QADatawareHouse_Reg132.dbo.[VizCFMonSettings] 
										WHERE [FormName]=@workspaceName
										AND [WorkspaceName]=@workspaceName
										AND [ReportDate]=@reportDate
										)
print @cashFlowTableName20122
			DECLARE @cashFlowTable20122 TABLE(
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
										
						DECLARE @cashFlowTable20131 TABLE(
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
			
			SET @sqlCashFlowTable20122=N'(SELECT [CF_TransId]
											  ,[CF_Date]
											  ,[CF_ContractType]
											  ,[CF_ValueType]
											  ,[CF_PortfolioId] 
											  ,[CF_Currency] 
											  ,[CF_Value]
											  ,[CF_Resolution]
											  ,[CF_Instrumenttype]
											  ,[CF_Instrumentname] FROM [QADatawareHouse_Reg122].[dbo].['+@cashFlowTableName20122+'])'


			SET @sqlCashFlowTable20132=N'(SELECT [CF_TransId]
											  ,[CF_Date]
											  ,[CF_ContractType]
											  ,[CF_ValueType]
											  ,[CF_PortfolioId] 
											  ,[CF_Currency] 
											  ,[CF_Value]
											  ,[CF_Resolution]
											  ,[CF_Instrumenttype]
											  ,[CF_Instrumentname] FROM [QADatawareHouse_Reg132].[dbo].['+@cashFlowTableName20132+'])'

			INSERT INTO @cashFlowTable20122 ([CF_TransId]
											  ,[CF_Date]
											  ,[CF_ContractType]
											  ,[CF_ValueType]
											  ,[CF_PortfolioId] 
											  ,[CF_Currency] 
											  ,[CF_Value]
											  ,[CF_Resolution]
											  ,[CF_Instrumenttype]
											  ,[CF_Instrumentname]) EXEC sp_executesql @sqlCashFlowTable20122
			
			
			INSERT INTO @cashFlowTable20131 ([CF_TransId]
											  ,[CF_Date]
											  ,[CF_ContractType]
											  ,[CF_ValueType]
											  ,[CF_PortfolioId] 
											  ,[CF_Currency] 
											  ,[CF_Value]
											  ,[CF_Resolution]
											  ,[CF_Instrumenttype]
											  ,[CF_Instrumentname]) EXEC sp_executesql @sqlCashFlowTable20132

			
			DECLARE @table20122 TABLE(
										  [CFDate] DATETIME
										  ,[Instrument] varchar(255)
										  ,[Value] FLOAT
										  ,[ExternalId]  Varchar(255)
										)
			
			DECLARE @table20132 TABLE(
							  [CFDate] DATETIME
							  ,[Instrument] varchar(255)
							  ,[Value] FLOAT
							  ,[ExternalId]  Varchar(255)
							  ,[LoadProfile]  Varchar(255)
							)
			
			insert into @table20122 (CFDate,Instrument,value,ExternalId) (
						SELECT [CF_Date]
								,[CF_Instrumentname]
							   ,[CF_Value]
							  ,(
									SELECT ExternalId FROM QADatawareHouse_Reg122.dbo.[VizTransMonData] AS TransactionMonitor
										  WHERE TransactionMonitor.ReportID=
														(
															SELECT [ReportID] FROM QADatawareHouse_Reg122.dbo.[VizTransMonReports]
															WHERE
															[Workspace]=@workspaceName
															AND
															[MonitorTitle]=@monitorName
															AND
															[ReportDate]=@reportDate
														)
										  AND TransactionMonitor.Transid=c.CF_TransId
								) as ExternalId
						  FROM @cashFlowTable20122 AS c
						  )
						  
			
			insert into @table20132(CFDate,Instrument, Value,ExternalId,LoadProfile) (
						SELECT [CF_Date]
								,[CF_Instrumentname]
							   ,[CF_Value]
							  ,(
									SELECT ExternalId FROM QADatawareHouse_Reg132.dbo.[VizTransMonData] AS TransactionMonitor
										  WHERE TransactionMonitor.ReportID=
														(
															SELECT [ReportID] FROM QADatawareHouse_Reg132.dbo.[VizTransMonReports]
															WHERE
															[Workspace]=@workspaceName
															AND
															[MonitorTitle]=@monitorName
															AND
															[ReportDate]=@reportDate
								
														)
										  AND TransactionMonitor.Transid=c.CF_TransId
								) as ExternalId
								,(
									SELECT LoadProfile FROM QADatawareHouse_Reg132.dbo.[VizTransMonData] AS TransactionMonitor
										  WHERE TransactionMonitor.ReportID=
														(
															SELECT [ReportID] FROM QADatawareHouse_Reg132.dbo.[VizTransMonReports]
															WHERE
															[Workspace]=@workspaceName
															AND
															[MonitorTitle]=@monitorName
															AND
															[ReportDate]=@reportDate
								
														)
										  AND TransactionMonitor.Transid=c.CF_TransId
								) as LoadProfile
						  FROM @cashFlowTable20131 AS c
						  )
				
--select * from @table20122  s where s.ExternalId='El-Asian-NOK-0365'
		
		select	drambuie.ExternalId,
				drambuie.LoadProfile ,
				drambuie.Instrument,
				drambuie.CFDate,
				round(brandy.Value,2) as brandyValue,
				round(drambuie.Value,2) as drambuieValue
				,round(brandy.Value/drambuie.Value-1,5) as Diff
			--	 drambuie.Value-brandy.Value as Diff, 
			--	 (drambuie.value/brandy.value)-1 as RelDif  
		from @table20122 brandy
		full join @table20132 drambuie
		on brandy.ExternalId=drambuie.ExternalId
			and brandy.CFDate=drambuie.CFDate
			and brandy.Instrument=drambuie.Instrument

		where brandy.CFDate>'20111101'
		and abs(round(drambuie.Value/brandy.Value-1,8))>0
		--and brandy.ExternalId='El-Asian-NOK-0365'
		order by brandy.ExternalId,brandy.CFDate