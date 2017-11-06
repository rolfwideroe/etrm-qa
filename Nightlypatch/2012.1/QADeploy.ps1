﻿ ############################################################################
 #
 #  						SAO Environment Script        
 #	This ia a nightly patch script to reinstall Elviz ETRM on QA SAO machines
 #
 ############################################################################
cls
# ------- Script variables ---------------
."c:\ElvizPatchScripts\Functions.ps1"
$setup="setup.exe" 
$Version="2012.1"
$VersionFolder="2012.1"
$qainstall="qainstall"
$qainstallpwd="installqa"
$DEployServer="BERSVDEPLOY"
$Servers=@("QA121A","QA121CR01","QA121AR02","IS-TEST-MF")
$Clients=@("QA121C","","QA121CR02","CLIENTTESTXP")
$Deleting_folders=@("Documentation")
$Deleting_files=@("Bin\*.tlb")
$QASCRIPTS="\\$DEployServer\INSTALLATIONS\QASCRIPTS\$version"
$PatchToolFolder="C:\util\PatchTools"
$SetupPath="\\$DEployServer\INSTALLATIONS\DEP_TEMP\$VersionFolder\setup.exe"
$ConfigPathset="\\$DEployServer\INSTALLATIONS\DEP_TEMP\QAConfig\Viz.Priceboard.Server.dll.config"
$LicencePath="\\$DEployServer\INSTALLATIONS\DEP_TEMP\QALicenceReg\ElvizETRMLicenses.reg" 
$delete_file_list=@(".tlb",".dll")
$run_date= Get-Date
$Log_Date= Get-Date -format "yyyyMMdd_HHmm"
$output_file = "c:\Elviz\NightlyPatch_log.txt"
set-alias psexec "$PatchToolFolder\psexec.exe"
$log = "out-file -filepath $output_file -Append"
#delete the log file
if(Test-Path $output_file){
    Remove-Item $output_file
}
Function Echo_msg([String]$msg,[String]$color){
	trap {
		#Write-Host "error happened: $($_.Exception.Message)"; `
		Write-Host "     The log file is locked by another process"; `
		continue
	}
	& {#try
		if($color -eq ""){
			$color ="green"
		}
		Write-Host -ForegroundColor $color "$msg"
		out-file -filepath $output_file -InputObject $msg -Append -Force -ErrorAction SilentlyContinue -Encoding Unicode
	}
}
Echo_msg "
------------------------------------------------------------
-                                                          -
-           Nightly Patch For QA Environment               -
-           Copy Rights Viz Risk Management                -
-              Insalling version $Version                  -
-                                                          -
------------------------------------------------------------"

#find the computer name
$CompName="$Env:ComputerName"
#$computer=get-wmiobject win32_computersystem
#$CompName=$computer.name
$Log_Date= Get-Date
 out-file -filepath $output_file -inputobject "  " 
Start-Sleep -Seconds 10
# set the variables of the computer found
Echo_msg "[INFO]:Check if the $CompName is availabe in the servers list"
# REGRESSION MACHINES VARIABLES
$c=0
$exist=$false
WHILE($c -lt $Servers.Length){
    if($Servers[$c] -ne $null){
        IF($CompName -eq $Servers[$c]){
            $IS_NET_API_LOGON_USERNAME1="viz\$qainstall"
            $IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
            $SQL_SERVER1="BERSV-SQL8REG"
            $SQL_USER1="EcmDbUser"
            $SQL_PASSW1="EcmDbQaReg"
            $DB_SYSTEM1="QAREGSystem121"
            $DB_ECM1="QAREGECMOLD121"
            $DB_PRICES1="QAREGPRICES121"
            $DB_DWH1="QADataWarehouse121"
            $DB_FWC1="VizForwardCurve"
            $MQ_SERVER1="localhost"
            $MQ_PORT1="61616"
            $VIZ_REMOTING_PORT1="8005"
            $PB_SERVER1="localhost" 
            #------- Machines names -----------
            $QAServerR=$Servers[$c]
            $QAClientR=$Clients[$c]
            # ***** to UNC path 
            $QAServer="\\$QAServerR"
            $QAClient="\\$QAClientR"
            $exist=$true
        IF($Servers[$c] -eq "QA121AR02"){ 
                $IS_NET_API_LOGON_USERNAME1="Viz\$qainstall"
                $IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
                $SQL_SERVER1="BERSV-SQL8REG"
                $SQL_USER1="EcmDbUser"
                $SQL_PASSW1="EcmDbQaReg"
                $DB_SYSTEM1="QASystem_Reg121"
                $DB_ECM1="QAECM_REG121"
                $DB_PRICES1="QAPrices_Reg121"
                $DB_DWH1="QADatawareHouse_Reg121"
                $DB_FWC1="VizForwardCurve121"
                $MQ_SERVER1="localhost"
                $MQ_PORT1="61616"
                $VIZ_REMOTING_PORT1="8005"
                $PB_SERVER1="localhost" 
                #------- Machines names -----------
                $QAServerR=$Servers[$c]
                $QAClientR=$Clients[$c]
                # ***** to UNC path 
                $QAServer="\\$QAServerR"
                $QAClient="\\$QAClientR"
                $exist=$true
                Break
            }ELSEIF(($Servers[$c] -eq "QA121A")-or ($Servers[$c] -eq "IS-TEST-MF")){ 
                $IS_NET_API_LOGON_USERNAME1="Viz\$qainstall"
                $IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
                $SQL_SERVER1="BERSV-SQL8QA"
                $SQL_USER1="EcmDbUser"
                $SQL_PASSW1="EcmDbUser"
                $DB_SYSTEM1="QASystem121"
                $DB_ECM1="QAECM121"
                $DB_PRICES1="QAPRICES121"
                $DB_DWH1="VizDataWarehouse121"
                $DB_FWC1="VizForwardCurve121"
                $MQ_SERVER1="localhost"
                $MQ_PORT1="61616"
                $VIZ_REMOTING_PORT1="8005"
                $PB_SERVER1="localhost" 
                #------- Machines names -----------
                $QAServerR=$Servers[$c]
                $QAClientR=$Clients[$c]
				$QAClientR1="QA112C"
                # ***** to UNC path 
                $QAServer="\\$QAServerR"
                $QAClient="\\$QAClientR"
                $exist=$true
                Break
             }ELSEIF($Servers[$c] -eq "IS-TEST-MF"){ 
                $IS_NET_API_LOGON_USERNAME1="Viz\$qainstall"
                $IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
                $SQL_SERVER1="BERSV-SQL8QA"
                $SQL_USER1="EcmDbUser"
                $SQL_PASSW1="EcmDbUser"
                $DB_SYSTEM1="QASystem121"
                $DB_ECM1="QAECM121"
                $DB_PRICES1="QAPRICES121"
                $DB_DWH1="VizDataWarehouse121"
                $DB_FWC1="VizForwardCurve121"
                $MQ_SERVER1="localhost"
                $MQ_PORT1="61616"
                $VIZ_REMOTING_PORT1="8005"
                $PB_SERVER1="localhost" 
                #------- Machines names -----------
                $QAServerR=$Servers[$c]
                $QAClientR=$Clients[$c]
				$QAClientR1="QA112C"
                # ***** to UNC path 
                $QAServer="\\$QAServerR"
                $QAClient="\\$QAClientR"
                $exist=$true
                Break
            }
        Break
        }ELSE{
            $c++
        }	
    }

}
IF($exist -eq $true){
    Echo_msg "[SUCCESS]:Application server  $QAServerR is availabe "
}ELSE{
    Echo_msg "[ERROR]:$CompName is not exist in the Servers list"
    Send_Error_Message "[ERROR]:$CompName is not exist in the Servers list Check the Nightly patch Script"
    exit
}
#########################################################################################################################################################
 
 # sending a message to QA
 Send_Start_message 

######################################################################
#Delete logfiles
Echo_msg "[INFO]:Deleting old log files........."
DEL "$QAServer\elviz\VizInstallLog*"
DEL "$QAServer\elviz\VizUninstallLog*"
# copying setup file to teh C: drive
Echo_msg "[INFO]:copying setup.exe to Temp folder........."
Copy-Item "$SetupPath" "c:\"  | Out-Null
$copiedSetup="c:\setup.exe"
IF(Test-Path $SetupPath){

    Echo_msg "[SUCCESS]:$copiedSetup copied to c:\"
}ELSE{
    Echo_msg "[ERROR]:$SetupPath this path does not exist, check the network connection"
    Send_Error_Message "[ERROR]:THE SETUP FILE WAS NOT FOUND OR NOT COPIED IN C:\SETUP.EXE"
    exit
}
Echo_msg "[INFO]:Check the connection to the application server"
# test connection the the machines
$AppServerExist = Ping-Address ($QAServerR)

IF ( $AppServerExist ){
    Echo_msg "[SUCCESS]:Connected to the application server $QAServerR" 
}ELSE{ 
    Echo_msg "[ERROR]:Cannot connect to the application server $QAServerR"
    Send_Error_Message "[ERROR]:COULD NOT CONNECT TO THE APPLICATION SERVER"
    exit
}
$ClientExist = Ping-Address ($QAClientR)
Echo_msg "[INFO]:Check the connection to the client machine"
IF ( $ClientExist ){
    Echo_msg "[SUCCESS]:Connected to the Client machine $QAClientR"
}ELSE{ 
    Echo_msg "[ERROR]:Cannot connect to the Client machine $QAClientR"
    Send_Error_Message "[ERROR]:COULD NOT CONNECT TO THE CLEINT MACHINE $QAClientR"
    exit
}

# modifying the registry rights
$get_drive=Get-PSDrive -Name HKLM -ErrorAction SilentlyContinue
if(!($get_drive)){
	New-PSDrive -Name HKLM -PSProvider Registry -Root HKEY_LOCAL_MACHINE > $null
}
$InstallPath_test=Test-RegistryValue "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\InstallInfo" "InstallPath" 
if ($InstallPath_test -eq $true){ 
	Set-ItemProperty "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\InstallInfo\" -Name "InstallPath" -Value "C:\Elviz"
}else{
	$InstallPath = Get-RegistryValue  "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\InstallInfo" "InstallPath" 
	if ($InstallPath -notlike "C:\Elviz"){ 
		New-Item -Path "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services"
		New-Item -Path "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz"
		New-Item -Path "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\InstallInfo"
		New-ItemProperty "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\InstallInfo\" -Name "InstallPath" -Value "C:\Elviz" -PropertyType "String" 
	}
}

#########################################################################
###########################################################y##############

Echo_msg "[INFO]:Killing Elviz Processes on the application server..."
$processes_names=@("TestComplete.exe","TestExecute.exe","Viz.Integration.Core.MessageQueueListener.exe","Viz.Integration.Core.FileWatcher.exe","Viz.Integration.Core.WCFPublisher.exe","Viz.MiddleWare.ETRMWinService.exe","Viz.MiddleWare.Priceboard.WinService.exe","TestExecute.exe","ElvizETRMClient.exe","ElvizDM.exe","ElvizFM.exe","ElvizCM.exe","ElvizTM.exe","ElvizLM.exe")
$Processes_objects = @()
FOREACH($process_name in $processes_names){
    $process_name = get-wmiobject win32_process -computername $QAServerR | where {$_.Name -like "$process_name"}
    if($process_name -ne $null){
        $Processes_objects=$Processes_objects + $process_name
    }
}

FOREACH($Process_object in $Processes_objects){
    $obj_name=$Process_object.Name
    Echo_msg "[INFO]:Terminating $obj_name "
    $terminate =$Process_object.Terminate()
}
FOREACH ($Process_object in $Processes_objects)
{
    $s= Get-Process -computername $QAServerR -Id $Process_object.ProcessId -ErrorAction SilentlyContinue
    if($s)
    {			
       $obj_name=$Process_object.Name
       $Obj_PID=$Process_object.ProcessId
       Echo_msg "[INFO]:Killing  $obj_name which has PID:$Obj_PID"
       & taskkill -PID $Process_object.ProcessId | Out-Null
  }
}

FOREACH ($Process_object in $Processes_objects)
{
$PID_Exist= Get-Process -computername $QAServerR -Id $Process_object.ProcessId -ErrorAction SilentlyContinue
$PID_Exist_name= $Process_object.Name
    if($PID_Exist)
    {	
        Echo_msg "[WARN]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
    }
}
Echo_msg "[INFO]:Stopping Elviz Curve Server Services on the application server...."
$sNames =@("Elviz Message Queue Listener Service","Elviz File Watching Service","Elviz ETRM","Elviz Curve Server Services","Elviz Priceboard Service")
FOREACH($sName IN $sNames){
    $service = get-service -display $sName -ErrorAction SilentlyContinue 
    IF( ! $service ){ 
        $msg1= "[INFO]:" + $sName + " was not installed on this computer." 
        Echo_msg $msg1
        } 
    ELSE{ 
        Stop-Service -Name $sName
        Start-Sleep -Seconds 10
        $msg1 = "[INFO]:" + $sName + "'s status is: " + $service.Status 
        Echo_msg $msg1
    }
}




IF($QAClientR -ne ""){
    Echo_msg "[INFO]:Kill all Elviz applications on the Client machine"
    $Processes_objects = @()
		FOREACH($process_name in $processes_names){
		    $process_name = get-wmiobject win32_process -computername $QAServerR | where {$_.Name -like "$process_name"}
		    if($process_name -ne $null){
		        $Processes_objects=$Processes_objects + $process_name
		    }
		}

		FOREACH($Process_object in $Processes_objects){
		    $obj_name=$Process_object.Name
		    Echo_msg "[INFO]:Terminating $obj_name "
		    $Process_object.Terminate()
		}
		FOREACH ($Process_object in $Processes_objects)
		{
		    $s= Get-Process -computername $QAServerR -Id $Process_object.ProcessId 
		    if($s)
		    {			
		       $obj_name=$Process_object.Name
		       $Obj_PID=$Process_object.ProcessId
		       Echo_msg "[INFO]:Killing  $obj_name which has PID:$Obj_PID"
		       taskkill -PID $Process_object.ProcessId
		  }
		}

		FOREACH ($Process_object in $Processes_objects)
		{
		$PID_Exist= Get-Process -computername $QAServerR -Id $Process_object.ProcessId 
		$PID_Exist_name= $Process_object.Name
		    if($PID_Exist)
		    {	
		        Echo_msg "[ERROR]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
		        Send_Error_Message "[ERROR]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
		        exit
		    }
		}
}
#logoff users in Client machine 
Echo_msg "[INFO]:Logging off all users on the client machine $QAClientR"
#psexec /accepteula -u viz\$qainstall /P $qainstallpwd  $QAClient "$QASCRIPTS\logoff.bat" 
logoff $QAClientR | Out-Null
#Start-sleep 15
#########################################################################

Echo_msg "[INFO]:Uninstallation of Elviz ETRM on $QAServerR "

# satrt sharing service if it is stopped
$Sharing_Services = get-service -display "Server" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
}ELSE{
    Start-Service -name "Server"  > $null

}

$product=Get-WmiObject -Class Win32_Product -Filter "name='Elviz ETRM (Server)'" -ComputerName "$QAServerR"
IF( $product -eq $null){
# if elviz not installed but still have junk files to delete
    Echo_msg "[INFO]:Deleting Elviz Subfolders "
        Foreach($Deleting_folder in $Deleting_folders){
            if (Test-Path "$QAServer\elviz\$Deleting_folder"){
                Del "$QAServer\elviz\$Deleting_folder" -Force -Recurse
                Echo_msg "[INFO]:Deleting $QAServer\elviz\$Deleting_folder"
				Start-Sleep -Seconds 10
            }
        }
        Foreach($Deleting_file in $Deleting_files){
            if (Test-Path "$QAServer\elviz\$Deleting_file"){
                Del "$QAServer\elviz\$Deleting_file" -Force -Recurse 
                Echo_msg "[INFO]:Deleting $QAServer\elviz\$Deleting_folder"
            }
        }
    #test if apps were removed
    FOREACH($Deleting_folder IN $Deleting_folders){
        IF(!(test-path "$QAServer\Elviz\$Deleting_folder")){
            $path_exist=$true
        }ELSE{
            Echo_msg "[ERROR]:this Folder was not removed: $QAServer\Elviz\$Deleting_folder"
            $path_exist=$false
            $Error_msg="[ERROR]:this Folder was not removed: $QAServer\Elviz\$Deleting_folder"
            break
        }
    }
	Echo_msg "[INFO]:Deleting Elviz tlb files "
	FOREACH($d in $delete_file_list){
		get-childitem "$QAServer\Elviz\Bin"  -include "*$d" -recurse | foreach ($_) {remove-item $_.fullname -Force  }
		Echo_msg "[INFO]:Delete $QAServer\Elviz\Bin\$_.fullname" 
	}
	FOREACH($d in $delete_file_list){
		$verify_Dbg_files = get-childitem "$QAServer\Elviz\Bin"  -include "*$d" -recurse
	}
	IF ($verify_Dbg_files -eq $null){
		Echo_msg "[INFO]:FINISHED Removing tlb files" 
	}
	
	
	
	
    If ($path_exist -eq $true){
            Echo_msg "[SUCCESS]:Elviz ETRM was removed successfully"
    }ELSE{
        Echo_msg "[ERROR]:Elviz ETRM was not removed successfully " 
        Send_Error_Message "$Error_msg"
        exit
    }
    Echo_msg "[WARN]:Elviz was not installed on this machine "
}ELSE{
    Echo_msg "[INFO]:Uninstalling Elviz........ (Min 2 minutes) "
    # Remove Elviz ETRM clsTransaction to Elviz ETRM
    # $product.Uninstall() 
    $app = Get-WmiObject -Class Win32_Product | Where-Object { 
    $_.Name -like "Elviz ETRM (Server)" 
    }
    $app.Uninstall() | Out-Null
#Start-Sleep -Seconds 180
    $Check_uninstall = Get-WmiObject -Class Win32_Product -Filter "name='Elviz ETRM (Server)'" -ComputerName "$QAServerR"
    IF( $Check_uninstall -eq $null){
        Echo_msg "[SUCCESS]:Elviz ETRM was removed Form Add\Remove programs"
    }Else{
        Echo_msg "[ERROR]:Elviz ETRM was not removed successfully" 
        Send_Error_Message "[ERROR]:ELVIZ WAS NOT REMOVED OF THE ADD\REMOVE PROGRAM, THE INSTALLATION PROCESS HAS BEEN STOPPED,"
        exit
    
    }
    Echo_msg "[INFO]:Deleting Elviz Subfolders "
        Foreach($Deleting_folder in $Deleting_folders){
            if (Test-Path "$QAServer\elviz\$Deleting_folder"){
                Del "$QAServer\elviz\$Deleting_folder" -Force -Recurse 
                Echo_msg "[INFO]:Deleting $QAServer\elviz\$Deleting_folder"
            }
        }
        Foreach($Deleting_file in $Deleting_files){
            if (Test-Path "$QAServer\elviz\$Deleting_file"){
                Del "$QAServer\elviz\$Deleting_file" -Force -Recurse 
                Echo_msg "[INFO]:Deleting $QAServer\elviz\$Deleting_folder"
            }
        }
    #test if apps were removed
    FOREACH($Deleting_folder IN $Deleting_folders){
        IF(!(test-path "$QAServer\Elviz\$Deleting_folder")){
            $path_exist=$true
        }ELSE{
            Echo_msg "[ERROR]:this Folder was not removed: $QAServer\Elviz\$Deleting_folder"
            $path_exist=$false
            $Error_msg="[ERROR]:this Folder was not removed: $QAServer\Elviz\$Deleting_folder"
            break
        }
    }
    
    If ($path_exist -eq $true){
            Echo_msg "[SUCCESS]:Elviz ETRM was removed successfully"
    }ELSE{
        Echo_msg "[ERROR]:Elviz ETRM was not removed successfully" 
        Send_Error_Message "$Error_msg"
        exit
    }
}
 # satrt sharing service if it is stopped
 $Sharing_Services = get-service -display "Server" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
}ELSE{
    Start-Service -name "Server"  > $null

}
#######################################################################
Echo_msg "[INFO]:Installing Elviz Application server on $QAServerR "
# run the setup file
& c:\Setup.exe /s /v" /qn IS_NET_API_LOGON_USERNAME=$IS_NET_API_LOGON_USERNAME1 IS_NET_API_LOGON_PASSWORD=$IS_NET_API_LOGON_PASSWORD1 SQL_SERVER=$SQL_SERVER1  SQL_USER=$SQL_USER1 SQL_PASSW=$SQL_PASSW1 DB_SYSTEM=$DB_SYSTEM1 DB_ECM=$DB_ECM1 DB_PRICES=$DB_PRICES1 DB_DWH=$DB_DWH1 DB_FWC=$DB_FWC1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=1 ELVIZ_DEAL_EVENTS=0 ELVIZ_WCF_PUBLISHER=1  ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=\\BERSV-SQL8QA\ECMimport\  BULKINSERTCLIENT=\\BERSV-SQL8QA\ECMimport\ " | Out-Null
# Start-Sleep -Seconds 540
 # satrt sharing service if it is stopped
 $Sharing_Services = get-service -display "Server" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
}ELSE{
    Start-Service -name "Server" > $null

}
 
 #Copying config files from Support to their locations
IF ($QAServerR -eq "QA121A"){
    ECHO ""| out-file -filepath $output_file -Append 
    Echo_msg "[INFO]:Copying config files from \\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\ Folder to their locations..." 
    
    IF(Test-Path "\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\Bin\Viz.Priceboard.Server.dll.config"){
        Echo_msg "[INFO]:Replacing  Vizlibs\Viz.Priceboard.Server.dll.config"
        Copy-Item  "\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\Bin\Viz.Priceboard.Server.dll.config" "C:\Elviz\Bin\" -Force 
    }ELSE{
        ECHO "[ERROR]:\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\Bin\Viz.Priceboard.Server.dll.config IS NOT EXIST"
    }
    IF(Test-Path "\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\PriceboardServerConfig\MontelApiFeeder.xml"){
        Echo_msg "[INFO]:Replacing  Configuration\PriceboardServerConfig\Adapters\MontelApiFeeder.xml"
        Copy-Item  "\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\PriceboardServerConfig\MontelApiFeeder.xml" "C:\Elviz\Bin\Configuration\PriceboardServerConfig\Adapters\" -Force 
    }ELSE{
        Echo_msg "[ERROR]:C:\ElvizPatchScripts\Config_files\Configuration\PriceboardServerConfig\MontelApiFeeder.xml IS NOT EXIST"
    }
	
	
	
	 IF(Test-Path "\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\AppSettings.config" ){
        Echo_msg "[INFO]:Replacing  Bin\Configuration\ClientConfig\AppSettings.config"
        Copy-Item  "\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\AppSettings.config" "C:\Elviz\Bin\Configuration\ClientConfig\" -Force 
    }ELSE{
        Echo_msg "[ERROR]:\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\AppSettings.config IS NOT EXIST"
    }
}
IF(Test-Path $LicencePath ){
    Echo_msg "[INFO]:Copying the License Reg to Elviz ETRM folder"
    Copy-Item  $LicencePath "C:\elviz\elviz ETRM\"
}ELSE{
    Echo_msg "[ERROR]:Elviz ETRM License was not copied, Check it is path $LicencePath"
}


$EUT_Info= "\\$DeployServer\INSTALLATIONS\DEP_TEMP\QALicenceReg\EUT_usernames_and_passwords.reg"
IF(Test-Path $EUT_Info ){
    Echo_msg "[INFO]:Copying the EUT Info to Elviz ETRM folder"
    Copy-Item  $EUT_Info "C:\elviz\elviz ETRM\"
}ELSE{
    Echo_msg "[ERROR]:EUT info file was not copied, Check it is path $LicencePath"
}
$Check_install = Get-WmiObject -Class Win32_Product -Filter "name='Elviz ETRM (Server)'" -ComputerName "$QAServerR"
Start-Sleep -Seconds 10
$installed=$false
IF( $Check_install -eq $null){
    Echo_msg "[ERROR]:Elviz ETRM was not installed successfully"
    Send_Error_Message "[ERROR]:ELVIZ WAS NOT INSTALLED ON THE APPLICATION SERVER, THE INSTALLATION PROCESS HAS BEEN STOPPED,"
    exit	
}ELSE{
    Echo_msg "[SUCCESS]:Elviz ETRM was installed successfully on the application server"
	$installed=$true
}
Remove-Item $copiedSetup -Force
Echo_msg "[INFO]:Setup file was deleted from the temp folder"

#########################################################################
Echo_msg "[INFO]:Starting Elviz Curve Server Services...."
Start-Service -name "Elviz Curve Server Services" > $null
Start-Sleep 5
$Viz_ETRM_Services = get-service -display "Elviz Curve Server Services" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
    Echo_msg "[INFO]:Elviz Curve Server Services status is : $ServiceStatus "
}ELSE{

    Echo_msg "[INFO]:Elviz Curve Server Services status is : $ServiceStatus "

}
Start-Service -name "Elviz Message Queue Listener Service" > $null
Start-Sleep 5
$Viz_ETRM_Services = get-service -display "Elviz Curve Server Services" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
    Echo_msg "[INFO]:Elviz Message Queue Listener Service status is : $ServiceStatus "
}ELSE{

    Echo_msg "[INFO]:Elviz Message Queue Listener Service status is : $ServiceStatus "

}

IF (($QAClientR -ne "") -and ($installed -eq $true)){
    Echo_msg "[INFO]:Running the Client install on $QAClientR "

    Echo_msg "[INFO]:Kill all Elviz applications on the Client machine"
    $processes_names=@("TestComplete.exe","TestExecute.exe","Viz.Windows.ETRM.exe","ElvizDM.exe","ElvizRM.exe","ElvizFM.exe","ElvizCM.exe","ElvizTM.exe","ElvizLM.exe","Viz.MiddleWare.ETRMWinService.exe","Viz.MiddleWare.Priceboard.WinService.exe")
 		$Processes_objects = @()
		FOREACH($process_name in $processes_names){
		    $process_name = get-wmiobject win32_process -computername $QAClientR | where {$_.Name -like "$process_name"}
		    if($process_name -ne $null){
		        $Processes_objects=$Processes_objects + $process_name
		    }
		}

		FOREACH($Process_object in $Processes_objects){
		    $obj_name=$Process_object.Name
		    Echo_msg "  [INFO]:Terminating $obj_name "
		    $Process_object.Terminate()
		}
		FOREACH ($Process_object in $Processes_objects)
		{
		    $s= Get-Process -computername $QAClientR -Id $Process_object.ProcessId 
		    if($s)
		    {			
		       $obj_name=$Process_object.Name
		       $Obj_PID=$Process_object.ProcessId
		       Echo_msg "  [INFO]:Killing  $obj_name which has PID:$Obj_PID"
		       taskkill -PID $Process_object.ProcessId
		  }
		}

		FOREACH ($Process_object in $Processes_objects)
		{
		$PID_Exist= Get-Process -computername $QAClientR -Id $Process_object.ProcessId 
		$PID_Exist_name= $Process_object.Name
		    if($PID_Exist)
		    {	
		        Echo_msg "[WARN]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
		        Send_Error_Message "[WARN]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
		        exit
		    }
		}

    # Check the app server installation to run the clientInstall.bat
    $Check_install = Get-WmiObject -Class Win32_Product -Filter "name='Elviz ETRM (Server)'" -ComputerName "$QAServerR"
    IF($Check_install -ne $null){
        Echo_msg "[INFO]:Starting the client install on $QAClient   "
		& schtasks /run /s $QAClientR  /tn ClientInstall
		Echo_msg "[WARN]:An Error message will be sent by mail if the client Instalaltion failed"
		Start-Sleep -s 240
    }ELSE{
        Echo_msg "[ERROR]:You cant run the client installation, check if the application server is installed and the client is available   [ERROR_MSG]"
    }
   Echo_msg "[INFO]:DISABLE The QA_DEPLOY scheduled tasks to prevent it to run after a normal reboot" 
   & schtasks /Change /ru viz\$qainstall /rp $qainstallpwd  /tn Run_QADEPLOY /DISABLE
   $end_date= Get-Date
    $msg2="
-------------------------------------------------------------------                
                 Installation is complete                   	
                 $end_date                                   
-------------------------------------------------------------------
        " 
  Echo_msg $msg2
  Send_End_Message
        
 #   }
}ELSE{
    Echo_msg "[INFO]:DISABLE The QA_DEPLOY scheduled tasks to prevent it to run after a normal reboot" 
    & schtasks /Change /ru viz\$qainstall /rp $qainstallpwd  /tn Run_QADEPLOY /DISABLE
	Echo_msg "  [SUCCESS]:Elviz ETRM was installed successfully on the application server $QAServerR"
    $end_date= Get-Date
    $msg2="
-------------------------------------------------------------------                
                 Installation is complete                   	
                 $end_date                                   
-------------------------------------------------------------------
    " 
    Echo_msg $msg2
    Send_End_Message
}

# sometimes Server service is stopped, which prevent file sharing.
$Sharing_Services = get-service -display "Server" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
}ELSE{
    Start-Service -name "Server"  > $null

}

IF (($QAServerR -eq "QA121Akkkkk")){
	"copy tests from the elviz release output"
	[System.Reflection.Assembly]::LoadFrom("C:\util\tfs\Microsoft.TeamFoundation.Client.dll")
	[System.Reflection.Assembly]::LoadFrom("C:\util\tfs\Microsoft.TeamFoundation.Build.Client.dll")	
	
	$elvizPath = "C:\Elviz"
	$serverName = "http://BERVS-TFS05:8080/tfs/BradyEnergy"
	$buildDefinition = "Dev_IntegrationTests"
	$teamProject = "Elviz"
	
	$collection = [Microsoft.TeamFoundation.Client.TfsTeamProjectCollectionFactory]::GetTeamProjectCollection($serverName)
	$buildServer = $collection.GetService([Microsoft.TeamFoundation.Build.Client.IBuildServer])
	$defenition = $buildServer.GetBuildDefinition($teamProject, $buildDefinition)

	$latestBuildDetail = $buildServer.QueryBuilds("Elviz", "Dev_Elviz_Release")[-1]	
	$matches = select-string -Path $latestBuildDetail.LogLocation -pattern 'BuildDirectory\s?=\s?\"(.*?)\"' -List
	$latestTestPath = $matches.matches[0].groups[0].groups[1].Value
	$latestTestPath = $latestTestPath -replace ":", "$"
	$latestTestPath
	
	$matches = select-string -Path $latestBuildDetail.LogLocation -pattern 'initiated by VIZ\\tfsservice from (.*?)\"' -List
	$buildAgentMachineName = $matches.matches[0].groups[0].groups[1].Value	
	
	$latestTestPath = "\\" + $buildAgentMachineName + "\" + $latestTestPath + "\Sources\Tests"
	
	
	"Path to copy test files from is $latestTestPath"
			
	if (test-path $latestTestPath)
	{		
		Copy-Item  $latestTestPath $elvizPath -Force -recurse                                                                             
		$buildServer.QueueBuild($defenition)
		break;
	}
}

IF (($QAServerR -eq "QA121AR02kkkk")){
	"copy tests from the elviz release output"
	[System.Reflection.Assembly]::LoadFrom("C:\util\tfs\Microsoft.TeamFoundation.Client.dll")
	[System.Reflection.Assembly]::LoadFrom("C:\util\tfs\Microsoft.TeamFoundation.Build.Client.dll")	
	
	$elvizPath = "C:\Elviz"
	$serverName = "http://BERVS-TFS05:8080/tfs/BradyEnergy"
	$buildDefinition = "Internal_Regression_11.2"
	$teamProject = "Elviz"
	
	$collection = [Microsoft.TeamFoundation.Client.TfsTeamProjectCollectionFactory]::GetTeamProjectCollection($serverName)
	$buildServer = $collection.GetService([Microsoft.TeamFoundation.Build.Client.IBuildServer])
	$defenition = $buildServer.GetBuildDefinition($teamProject, $buildDefinition)

	$latestBuildDetail = $buildServer.QueryBuilds("Elviz", "Dev_Elviz_Release")[-1]	
	$matches = select-string -Path $latestBuildDetail.LogLocation -pattern 'BuildDirectory\s?=\s?\"(.*?)\"' -List
	$latestTestPath = $matches.matches[0].groups[0].groups[1].Value
	$latestTestPath = $latestTestPath -replace ":", "$"
	$latestTestPath
	
	$matches = select-string -Path $latestBuildDetail.LogLocation -pattern 'initiated by VIZ\\tfsservice from (.*?)\"' -List
	$buildAgentMachineName = $matches.matches[0].groups[0].groups[1].Value	
	
	$latestTestPath = "\\" + $buildAgentMachineName + "\" + $latestTestPath + "\Sources\Tests"
	
	
	"Path to copy test files from is $latestTestPath"
			
	if (test-path $latestTestPath)
	{		
		Copy-Item  $latestTestPath $elvizPath -Force -recurse                                                                             
		$buildServer.QueueBuild($defenition)
		break;
	}
}
& schtasks /Change /ru viz\$qainstall /rp $qainstallpwd  /tn Run_QADEPLOY /DISABLE
