DECLARE @new_alias VARCHAR(50)
DECLARE @old_alias VARCHAR(50)
DECLARE @VizECM VARCHAR(50)

DECLARE @Old_VizECM VARCHAR(50)
set @OLd_VizECM='QAECM_Reg122' --original name of VizECM (Before copying it)
set @VizECM='QAECM_Reg123' --New name of vizECM after copying and resotring it 
--set @old_alias='PROD Environment' --this is the old alias of VizECM if they change it  
set @new_alias='QAREG123' --this is the new  alias of VizECM


-- modify the physical name on SystemINfo
Update SystemInfo 
set SysDatabaseName = @VizECM
where SysDatabaseName=@OLd_VizECM

-- modify the alias name on SystemINfo
Update SystemInfo 
set SysOwnerName = @new_alias
where SysDatabaseName=@VizECM

-- in case there is a different physical names on prod than on test
update [EP_ElementProperties]
set EP_PropertyValue=@VizECM
where EP_PropertyName='Dbname'
--and EP_PropertyValue=@OLd_VizECM

update [EP_ElementProperties]    
set [EP_PropertyValue]=@new_alias
where   EP_PropertyName='installation'
--and EP_PropertyValue=@old_alias
