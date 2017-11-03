declare @reportId int

set @reportId=4


declare @newInstallationId INT

INSERT INTO Installation VALUES('AAA')

SET @newInstallationId=(SELECT SCOPE_IDENTITY())

PRINT @newInstallationId

declare @newReportId INT
SET @newReportId=(SELECT MAX([PR_ID])+1 FROM [PR_ReportHeader])
print @newReportId

INSERT INTO [PR_ReportHeader]
  SELECT
	  @newReportId
      ,[PR_RpDate]
      ,@newInstallationId
      ,[PR_RpCur]
      ,[PR_WorkSpaceID]
      ,[PR_WorkSpace]
      ,[PR_FrmName]
      ,[PR_CrDate]
      ,[PR_CrBy]
  FROM [PR_ReportHeader]
  WHERE PR_ID=@reportId


SELECT * FROM PR_ReportHeader r where r.PR_ID=@reportId



select * from PD_DataValues d where d.PD_PR_ID=@reportId

INSERT INTO PD_DataValues
SELECT [PD_Recno]
      ,@newReportId     --[PD_PR_ID]
      ,[PD_PR_RpDate]
      ,	@newInstallationId			--[PD_PR_InstID]
      ,[PD_Period]
      ,[PD_Product]
      ,[PD_PortfolioName]
      ,[PD_PortfolioID]
      ,[PD_TrFilterName]
      ,[PD_GcFiltername]
      ,[PD_LoadType]
      ,[PD_DelType]
      ,[PD_Pricebasis]
      ,[PD_PriceSource]
      ,[PD_TimeZone]
      ,[PD_Currency]
      ,[PD_CurrSource]
      ,[PD_Commodity]
      ,[PD_InstType]
      ,[PD_Instrument]
      ,[PD_InstPerFrom]
      ,[PD_InstPerTo]
      ,[PD_PosStatus]
      ,[PD_CrDate]
      ,[PD_ExecutionVenue]
      ,[PD_GroupField1]
      ,[PD_GroupField2]
      ,[PD_GroupField3]
      ,[PD_CustomAttribute]
      ,[PD_TransactionIds]
  FROM [PD_DataValues]
  WHERE PD_PR_ID=@reportId


INSERT INTO [PP_PriceValue]
SELECT [PP_Recno]
      ,@newReportId--[PP_PR_ID]
      ,[PP_PR_RpDate]
      ,@newInstallationId --[PP_PR_InstID]
      ,[PP_Bkprice]
      ,[PP_Bkvalue]
      ,[PP_Mktprice]
      ,[PP_Uprice]
      ,[PP_Volat]
      ,[PP_CrDate]
      ,[PP_PriceUnit]
  FROM [PP_PriceValue]
  WHERE PP_PR_ID=@reportId

INSERT INTO PA_Amount
	SELECT [PA_Recno]
      ,@newReportId --[PA_PR_ID]
      ,[PA_PR_RpDate]
      ,@newInstallationId--[PA_PR_InstID]
      ,[PA_QTY]
      ,[PA_MinVol]
      ,[PA_MaxVol]
      ,[PA_UsedVol]
      ,[PA_Hours]
      ,[PA_DeltaQuantity]
      ,[PA_CrDate]
      ,[PA_QuantityUnit]
  FROM [PA_Amount]
  WHERE PA_PR_ID=@reportId

  INSERT INTO PG_Greek
	SELECT [PG_Recno]
      ,@newReportId --[PG_PR_ID]
      ,[PG_PR_RpDate]
      ,@newInstallationId --[PG_PR_InstID]
      ,[PG_Expiry]
      ,[PG_Strike]
      ,[PG_Gamma]
      ,[PG_Theta]
      ,[PG_Vega]
      ,[PG_Rho]
      ,[PG_Delta]
      ,[PG_CrDate]
  FROM [PG_Greek]
  WHERE PG_PR_ID=@reportId

--select * from PG_Greek g where g.PG_PR_ID=@reportId

	INSERT INTO [PC_Calculated]
	SELECT [PC_Recno]
      ,@newReportId--[PC_PR_ID]
      ,[PC_PR_RpDate]
      ,@newInstallationId--[PC_PR_InstID]
      ,[PC_Trade_PL]
      ,[PC_Real_PL]
      ,[PC_Unreal_PL]
      ,[PC_Net_PL]
      ,[PC_GrossValue]
      ,[PC_MtmValue]
      ,[PC_CFstoDate]
      ,[PC_FwdCFs]
      ,[PC_NetCfs]
      ,[PC_ExpMWyr]
      ,[PC_ExpMWh]
      ,[PC_FeesPaid]
      ,[PC_CrDate]
      ,[PC_RealTrade_PL]
      ,[PC_UnrealTrade_PL]
  FROM [PC_Calculated]
  WHERE PC_PR_ID=@reportId


--select * from PC_Calculated c where c.PC_PR_ID=@reportId