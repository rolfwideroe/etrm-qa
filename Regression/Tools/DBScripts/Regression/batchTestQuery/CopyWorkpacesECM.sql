
  
  DECLARE @table table (
						WSId INT IDENTITY(1,1), 
						WSName VARCHAR(50)
					  )
					  
  insert into @table select WSName from Workspace where WSName like 'El-European-EUR%'
  
  declare @index int
  set @index=1
  declare @maxIndex int
  set @maxIndex=(select MAX(WSId) from @table)
  
  while @index<=@maxIndex
  begin
	--SELECT t.WSName FROM @table t where t.WSId=@index

  
  
  
  declare @copyFromWorkSpace varchar(55)
  declare @workspace varchar(55)
  declare @workspaceId INT
  declare @copyFromWorkSpaceId INT
  declare @filterName Varchar(55)
  declare @filterId INT
  
  set @filterName='Electricity-Asian-Struct'
  set @copyFromWorkSpace=(SELECT t.WSName FROM @table t where t.WSId=@index)
   
  set @workspace=REPLACE(@copyFromWorkSpace,'El-European-EUR','El-Asian-Struct')
  
  INSERT INTO Workspace VALUES(@workspace,'Common',1,-1)
  set @workspaceId = (select WSId from Workspace where WSName=@workspace)
  set @copyFromWorkSpaceId= (select WSId from Workspace where WSName=@copyFromWorkSpace)
  set @filterId=(select f.FilterId from Filters f where f.FilterName=@filterName)
  
  
 -- select @workspaceId, FormId,FormName  from WSForm f where f.WSId=@copyFromWorkSpaceId
  
  insert into WSForm 
    select @workspaceId, FormId,FormName  from WSForm f where f.WSId=@copyFromWorkSpaceId
  
  insert into WSFormSettings
	  select @workspaceId,FormId,Setting,Value,SettingType,WindowSetting from WSFormSettings where WSId=@copyFromWorkSpaceId
	  
  update WSFormSettings set Value=@filterId where Setting='FilterId' and WSId=@workspaceId
    update WSFormSettings set Value=@workspace where Setting in ('CustomName','Caption') and Value=@copyFromWorkSpace and WSId=@workspaceId
  	set @index=@index+1
  end