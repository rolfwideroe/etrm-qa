/****** Script for SelectTopNRows command from SSMS  ******/

DECLARE @workspaceNamesLike varchar(55)
DECLARE @changeFrom varchar(55)
DECLARE @chagneTo varchar(55)
DECLARE @newFilterName varchar(55)


set @workspaceNamesLike ='El-Asian-EUR%'
set @changeFrom='el-asian-eur'
set @chagneTo='0Gas-Shit-NOK'
set @newFilterName='Electricity-Forward-EUR'

		declare @newFilterId INT
		set @newFilterId=(select FilterId from DAILY_QAECM_Reg153.dbo.Filters where FilterName=@newFilterName)

  DECLARE @table table (
						WSId INT IDENTITY(1,1), 
						WSName VARCHAR(50)
					  )
	
  insert into @table
  SELECT [EP_PropertyValue] FROM [EP_ElementProperties]
  where EP_PropertyName ='name'
  and EP_PropertyValue like @workspaceNamesLike
 
declare @workspaceIndex int

  set @workspaceIndex=1
  declare @maxIndex int
  set @maxIndex=(select MAX(WSId) from @table)
  
  while @workspaceIndex<=@maxIndex
  begin

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


		  declare @string nvarchar(max)

		  set @string=(select [ED_Data] from ED_ElementData  where [ED_Id]=@maxEdId)

		  DECLARE @output table (
								splitdata VARCHAR(255)
							  )

		  DECLARE @start INT, @end INT 
			SELECT @start = 1, @end = CHARINDEX(',', @string) 
			WHILE @start < LEN(@string) + 1 
			BEGIN 
				IF @end = 0  
					SET @end = LEN(@string) + 1
       
				INSERT INTO @output (splitdata)  
				VALUES(SUBSTRING(@string, @start, @end - @start)) 
				SET @start = @end + 1 
				SET @end = CHARINDEX(',', @string, @start)
			END

			declare @financialFilterPartToReplace varchar(255)
			set @financialFilterPartToReplace =(select top 1 splitData from @output WHERE splitdata LIKE 'FinancialFilterId=%')
			delete from @output

		  -- print replace(@string,@financialFilterPartToReplace,'FinancialFilterId='+cast(@newFilterId as nvarchar) )



		  update ED_ElementData set ED_Data=replace(@string,@financialFilterPartToReplace,'FinancialFilterId='+cast(@newFilterId as nvarchar))
		  where [ED_Id]=@maxEdId
	
	set @workspaceIndex=@workspaceIndex+1
	
	end
	
 --select * from EP_ElementProperties where EP_ED_Id=308

-- delete from EP_ElementProperties where ep_id>=2093