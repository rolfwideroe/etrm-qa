/****** Script for SelectTopNRows command from SSMS  ******/

DECLARE @workspaceNamesLike varchar(55)
DECLARE @changeFrom varchar(55)
DECLARE @chagneTo varchar(55)
DECLARE @newFilterName varchar(55)






  DECLARE @table table (
						WSId INT IDENTITY(1,1), 
						WSName VARCHAR(50)
					  )

INSERT INTO @table VALUES('El-Asian-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Asian-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Asian-NOK-Exposure-EUR')
INSERT INTO @table VALUES('El-Asian-Profile-EUR-Exposure-EUR-VolSurf')
INSERT INTO @table VALUES('El-Asian-Profile-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Asian-Profile-EUR-Exposure-NOK-VolSurf')
INSERT INTO @table VALUES('El-Asian-Profile-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Asian-Spread-EUR-Exposure-EUR-Volsurf')
INSERT INTO @table VALUES('El-Asian-Spread-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Asian-Spread-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Asian-Struct-Exposure-EUR')
INSERT INTO @table VALUES('El-EU-Profile-EUR-Exposure-EUR-VolSurf')
INSERT INTO @table VALUES('El-EU-Profile-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-EU-Profile-EUR-Exposure-NOK-VolSurf')
INSERT INTO @table VALUES('El-EU-Profile-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-EU-Struct-EUR-Exposure-EUR-VolSurf')
INSERT INTO @table VALUES('El-EU-Struct-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-EU-Struct-EUR-Exposure-NOK-VolSurf')
INSERT INTO @table VALUES('El-EU-Struct-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-European-EUR-Exposure-EUR-VolSurf')
INSERT INTO @table VALUES('El-European-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-European-EUR-Exposure-NOK-VolSurf')
INSERT INTO @table VALUES('El-European-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-FixedPriceFloatingVolume-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-FixedPriceFloatingVolume-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-FixedPriceFloatingVolume-NOK-Exposure-EUR')
INSERT INTO @table VALUES('El-FixedPriceFloatingVolume-NOK-Exposure-NOK')
INSERT INTO @table VALUES('El-Floating-Spot-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Floating-Spot-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Forward-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Forward-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Forward-NOK-Exposure-EUR')
INSERT INTO @table VALUES('El-Forward-NOK-Exposure-NOK')
INSERT INTO @table VALUES('El-Future-EEX-Spain-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Future-EEX-Spain-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Future-EEXFrance-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Future-EEXFrance-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Future-EEXItaly-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Future-EEXItaly-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Future-NordPool-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Future-NordPool-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Spot-EUR-Exposure-EUR-Aggr')
INSERT INTO @table VALUES('El-Spot-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Spot-EUR-Exposure-NOK-Aggr')
INSERT INTO @table VALUES('El-Spot-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Struct-Fixed-EUR-Exposure-EUR-Aggr')
INSERT INTO @table VALUES('El-Struct-Fixed-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Struct-Fixed-EUR-Exposure-NOK-Aggr')
INSERT INTO @table VALUES('El-Struct-Fixed-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-Struct-Spot-EUR-Exposure-EUR-Aggr')
INSERT INTO @table VALUES('El-Struct-Spot-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-Struct-Spot-EUR-Exposure-NOK-Aggr')
INSERT INTO @table VALUES('El-Struct-Spot-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-StructFlex-CapFloor-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-StructFlex-Fixed-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-StructFlex-Fixed-EUR-Exposure-NOK')
INSERT INTO @table VALUES('El-StructFlex-Spot-EUR-Exposure-EUR')
INSERT INTO @table VALUES('El-StructFlex-Spot-EUR-Exposure-NOK')
INSERT INTO @table VALUES('ElCert-Forward-OTC-Exposure-EUR')
INSERT INTO @table VALUES('ElCert-Forward-OTC-Exposure-SEK')
INSERT INTO @table VALUES('Emission-Future-ICE-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Emission-Future-ICE-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Emission-Future-NP-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Emission-Future-NP-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-EUR-ICE-Exposure-EUR-VolSurf')
INSERT INTO @table VALUES('Gas-EUR-ICE-Exposure-EUR')
INSERT INTO @table VALUES('Gas-EUR-ICE-Exposure-NOK-VolSurf')
INSERT INTO @table VALUES('Gas-EUR-ICE-Exposure-NOK')
INSERT INTO @table VALUES('Gas-FFF-ICEMonthAheadNBP-GBP-Exposure-GBP')
INSERT INTO @table VALUES('Gas-FFF-ICEMonthAheadNBP-GBP-Exposure-NOK')
INSERT INTO @table VALUES('Gas-Forward-GJd-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-Forward-GJd-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-Forward-MW-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-Forward-MW-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-Forward-MWhd-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-Forward-MWhd-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-Forward-Sm3d-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-Forward-Sm3d-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-Forward-THD-GBP-Exposure-GBP')
INSERT INTO @table VALUES('Gas-Forward-THD-GBP-Exposure-NOK')
INSERT INTO @table VALUES('Gas-Future-Endex-TTF-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-Future-Endex-TTF-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-Future-ICE-TTF-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-Future-ICE-TTF-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-IndOption-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-Storage-TTF-Mix-Exposure-EUR')
INSERT INTO @table VALUES('Gas-Storage-TTF-Mix-Exposure-EUR-IntraDay')
INSERT INTO @table VALUES('Gas-Storage-TTF-Mix-Exposure-EUR')
INSERT INTO @table VALUES('Gas-Storage-TTF-Mix-Exposure-NOK')
INSERT INTO @table VALUES('Gas-Storage-TTF-Mix-Exposure-NOK-IntraDay')
INSERT INTO @table VALUES('Gas-Storage-TTF-Mix-Exposure-NOK')
INSERT INTO @table VALUES('Gas-StructFlex-CapFloor-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-StructFlex-Fixed-GJd-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-StructFlex-Fixed-GJd-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-StructFlex-Fixed-MW-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-StructFlex-Fixed-MW-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-StructFlex-Fixed-MWhd-EUR-Exposure-EUR')
INSERT INTO @table VALUES('Gas-StructFlex-Fixed-MWhd-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Gas-StructFlex-Fixed-Thd-GBP-Exposure-GBP')
INSERT INTO @table VALUES('Gas-StructFlex-Fixed-Thd-GBP-Exposure-NOK')
INSERT INTO @table VALUES('Gas-StructFlex-Index-Sm3d-EUR-Exposure-EUR')
INSERT INTO @table VALUES('GenCon-Consumption-Spot-Exposure-EUR')
INSERT INTO @table VALUES('GenCon-Consumption-Spot-Exposure-NOK')
INSERT INTO @table VALUES('Oil-American-USD-Exposure-EUR-VolSurf')
INSERT INTO @table VALUES('Oil-Asian-USD-Exposure-EUR')
INSERT INTO @table VALUES('Oil-FFF-ICEBrentOilFLFSwap-USD-Exposure-NOK')
INSERT INTO @table VALUES('Oil-FFF-ICEBrentOilFLFSwap-USD-Exposure-USD')
INSERT INTO @table VALUES('Oil-Future-ICE-USD-Exposure-EUR')
INSERT INTO @table VALUES('Oil-Future-ICE-USD-Exposure-USD')
INSERT INTO @table VALUES('Special-Cases-AsianBundle-Exposure-NOK')
INSERT INTO @table VALUES('Special-Cases-AsianDandQ-EUR-Exposure-NOK')
INSERT INTO @table VALUES('Special-Cases-AsianDandQ-Exposure-NOK')
INSERT INTO @table VALUES('Special-Cases-AsianSpreadNOK-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-AvgRollCurrency-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-CalcPriceBasis-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-CalcPriceBasis-Exposure-NOK')
INSERT INTO @table VALUES('Special-Cases-Discounting-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-FPFV-FSD-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-FSD-All-Exposure-EUR-Aggr')
INSERT INTO @table VALUES('Special-Cases-FSD-All-Exposure-NOK-Aggr')
INSERT INTO @table VALUES('Special-Cases-FSD-Model-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-FSD-Null-Exposure-EUR-ReplaceWith0')
INSERT INTO @table VALUES('Special-Cases-FSD-Null-Exposure-EUR-ReplaceWithForecast')
INSERT INTO @table VALUES('Special-Cases-FSD-Null-Exposure-EUR-ThrowException')
INSERT INTO @table VALUES('Special-Cases-GridPointPricebasis-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-GridPointPricebasis-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-Index-ICEEUARollingDec-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-IndexAverage-Exposure-USD')
INSERT INTO @table VALUES('Special-Cases-NotDiscountable-Exposure-EUR')
INSERT INTO @table VALUES('Special-Cases-Rounding-Exposure-USD')
INSERT INTO @table VALUES('Special-Cases-ZeroVolume-Exposure-NOK')



  --insert into @table
  --SELECT [EP_PropertyValue] FROM [EP_ElementProperties]
  --where EP_PropertyName ='name'
  --and EP_PropertyValue like @workspaceNamesLike
 
declare @workspaceIndex int

  set @workspaceIndex=1
  declare @maxIndex int
  set @maxIndex=(select MAX(WSId) from @table)
  
  while @workspaceIndex<=@maxIndex
  begin

		set @changeFrom=(SELECT WSName FROM @table WHERE WSId=@workspaceIndex)
		set @chagneTo='zNew-'+@changeFrom

		DECLARE @maxEdId INT

		SET @maxEdId = (SELECT MAX([ED_Id]) from ED_ElementData)

		set @maxEdId=@maxEdId+1

		declare @oldEdId INT

		declare @name varchar(255)

		set @name=(select WSName from @table where WSId=@workspaceIndex)
		print @name
		set @oldEdId=(
						SELECT EP_ED_Id
					  FROM [EP_ElementProperties]
					  where EP_PropertyName='name'
					  and EP_PropertyValue =@name
		  )



		INSERT INTO ED_ElementData
		select 
			  @maxEdId
			  ,[ED_AppName]
			  ,[ED_Data]
			  ,[ED_CrBy]
			  ,[ED_CrDate]
			  ,[ED_UpdBy]
			  ,[ED_UpdDate]
			  ,[ED_EC_Id]
		from ED_ElementData
		WHERE ED_ID =@oldEdId

		declare @data nvarchar(max)

		set @data =(select ED_Data from ED_ElementData where ed_id=@maxEdId)

		--print @data
		set @data=REPLACE(@data,@changeFrom,@chagneTo)

		--print @data

		update ED_ElementData set ED_Data=@data where ED_Id=@maxEdId

		--Insert into EP_ElementProperties

		  DECLARE @epTable table (
								IndexId INT IDENTITY(1,1), 
								EP_Id int
							  )

			INSERT INTO @epTable
				select  [EP_Id] from EP_ElementProperties where EP_ED_Id=@oldEdId
		
		
		declare @epIndex int
		declare @epMaxIndex int

		set @epIndex=(select min(IndexId)  from @epTable) 
		set @epMaxIndex=(select max(IndexId)  from @epTable) 

		  while @epIndex<=@epMaxIndex
		  begin
	
			declare @epMax Int
			set @epMax= (select max(ep_id) from EP_ElementProperties)
			set @epMax=@epMax+1

			insert into EP_ElementProperties
			select 
			  @epMax
			  ,[EP_PropertyName]
			  ,[EP_PropertyValue]
			  ,[EP_CrBy]
			  ,[EP_CrDate]
			  ,[EP_UpdBy]
			  ,[EP_UpdDate]
			  ,@maxEdId
			  from EP_ElementProperties where EP_Id=(select EP_Id from @epTable where IndexId=@epIndex)
		--print 'insertet prop'
	
		set @epIndex=@epIndex+1
		end
		
		--select * from @epTable
		delete from @epTable
		

		  update EP_ElementProperties set EP_PropertyValue=REPLACE(@name,@changeFrom,@chagneTo)
		  where EP_PropertyName='name'
		  and EP_ED_Id=@maxEdId


		  --declare @string nvarchar(max)

		  --set @string=(select [ED_Data] from ED_ElementData  where [ED_Id]=@maxEdId)

		  --DECLARE @output table (
				--				splitdata VARCHAR(255)
				--			  )

		 -- DECLARE @start INT, @end INT 
			--SELECT @start = 1, @end = CHARINDEX(',', @string) 
			--WHILE @start < LEN(@string) + 1 
			--BEGIN 
			--	IF @end = 0  
			--		SET @end = LEN(@string) + 1
       
			--	INSERT INTO @output (splitdata)  
			--	VALUES(SUBSTRING(@string, @start, @end - @start)) 
			--	SET @start = @end + 1 
			--	SET @end = CHARINDEX(',', @string, @start)
			--END

			--declare @financialFilterPartToReplace varchar(255)
			--set @financialFilterPartToReplace =(select top 1 splitData from @output WHERE splitdata LIKE 'FinancialFilterId=%')
			--delete from @output

		  -- print replace(@string,@financialFilterPartToReplace,'FinancialFilterId='+cast(@newFilterId as nvarchar) )



		--  update ED_ElementData set ED_Data=replace(@string,@financialFilterPartToReplace,'FinancialFilterId='+cast(@newFilterId as nvarchar))
		 -- where [ED_Id]=@maxEdId
	
	set @workspaceIndex=@workspaceIndex+1
	
	end
	
 --select * from EP_ElementProperties where EP_ED_Id=308

-- delete from EP_ElementProperties where ep_id>=2093