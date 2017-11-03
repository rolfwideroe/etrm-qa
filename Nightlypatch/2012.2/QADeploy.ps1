 ############################################################################
 #
 #  						SAO Environment Script        
 #	This ia a nightly patch script to reinstall Elviz ETRM on QA SAO machines
 #
 ############################################################################
cls
# ------- Script variables ---------------
."c:\ElvizPatchScripts\Functions.ps1"
$setup="setup.exe" 
$Version="2012.2"
$VersionFolder="2012.2"
$qainstall="Bradyplc\qainstall"
$qainstallpwd="Isetitup2"
$DEployServer="BERSVDEPLOY"
$Servers=@("BERVS-QA122A","BERVM-QA122C01","QA131AR01","BERVS-QA122A02","IS-TEST-MF","CCTESTNET")
$Clients=@("BERVS-QA122C","","QA131CR01","BERVM-QA122C02","CLIENTTESTXP","")
$Deleting_folders=@("Documentation")
$Deleting_files=@("Bin\*.tlb")
$QASCRIPTS="\\$DEployServer\INSTALLATIONS\QASCRIPTS\$version"
$PatchToolFolder="C:\util\PatchTools"
$WSMPath="\\$DEployServer\INSTALLATIONS\DEP_TEMP\WSM\2012.1\WSM2012.1.msi"
$msiFilesource="\\BERSVDEPLOY\INSTALLATIONS\DEP_TEMP\CCNET\2013.1\CRV2012.3.msi"
$WSMLocalpath="C:\WSM2012.1.msi"

$SetupPath="\\$DEployServer\INSTALLATIONS\DEP_TEMP\$VersionFolder\setup.exe"
$ConfigPathset="\\$DEployServer\INSTALLATIONS\DEP_TEMP\QAConfig\Viz.Priceboard.Server.dll.config"
$LicencePath="\\$DEployServer\INSTALLATIONS\DEP_TEMP\QALicenceReg\ElvizETRMLicenses.reg" 
$delete_file_list=@(".tlb",".dll")
$run_date= Get-Date
$Log_Date= Get-Date -format "yyyyMMdd_HHmm"
$output_file = "c:\Elviz\NightlyPatch_log.txt"
set-alias psexec "$PatchToolFolder\psexec.exe"
#delete the log file
if(Test-Path $output_file){
    Remove-Item $output_file
}



#find the computer name
$CompName="$Env:ElvizapplServer"
$Log_Date= Get-Date
 out-file -filepath $output_file -inputobject "  " 
Start-Sleep -Seconds 10
# set the variables of the computer found
Echo_msg "$output_file" "------------------------------------------------------------"
Echo_msg "$output_file" "-                                                          -"
Echo_msg "$output_file" "-           Nightly Patch For QA Environment               -"
Echo_msg "$output_file" "-           Copy Rights Viz Risk Management                -"
Echo_msg "$output_file" "-              Insalling version $Version                  -"
Echo_msg "$output_file" "-                                                          -"
Echo_msg "$output_file" "------------------------------------------------------------"

Echo_msg "$output_file" "[INFO]:Check if the $CompName is availabe in the servers list"
# REGRESSION MACHINES VARIABLES
$c=0
$exist=$false
WHILE($c -lt $Servers.Length){
    if($Servers[$c] -ne $null){
        IF($CompName -eq $Servers[$c]){
            $IS_NET_API_LOGON_USERNAME1="$qainstall"
            $IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
            $SQL_SERVER1="BERSV-SQL8REG"
            $SQL_USER1="EcmDbUser"
            $SQL_PASSW1="EcmDbQaReg"
            $DB_SYSTEM1="QAREGSystem122"
            $DB_ECM1="QAREGECMOLD122"
            $DB_PRICES1="QAREGPRICES122"
            $DB_DWH1="QADataWarehouse122"
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
       IF($Servers[$c] -eq "BERVS-QA122A02"){ 
				$IS_NET_API_LOGON_USERNAME1="$qainstall"
                $IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
                $SQL_SERVER1="BERSV-SQL8REG"
                $SQL_USER1="EcmDbUser"
                $SQL_PASSW1="EcmDbQaReg"
                $DB_SYSTEM1="QASystem_Reg122"
                $DB_ECM1="QAECM_REG122"
                $DB_PRICES1="QAPrices_Reg122"
                $DB_DWH1="QADatawareHouse_Reg122"
                $DB_FWC1="VizForwardCurve122"
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
            }ELSEIF(($Servers[$c] -eq "BERVS-QA122A") -or ($Servers[$c] -eq "IS-TEST-MF")){ 
                $IS_NET_API_LOGON_USERNAME1="$qainstall"
                $IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
                $SQL_SERVER1="BERSV-SQL8QA"
                $SQL_USER1="EcmDbUser"
                $SQL_PASSW1="EcmDbUser"
                $DB_SYSTEM1="QASystem122"
                $DB_ECM1="QAECM122"
                $DB_PRICES1="QAPRICES122"
                $DB_DWH1="VizDataWarehouse122"
                $DB_FWC1="VizForwardCurve122"
                $MQ_SERVER1="localhost"
                $MQ_PORT1="61616"
                $VIZ_REMOTING_PORT1="8005"
                $PB_SERVER1="localhost" 
                #------- Machines names -----------
                $QAServerR=$Servers[$c]
                $QAClientR=$Clients[$c]
				$QAClientR1="BERVS-QA122C"
                # ***** to UNC path 
                $QAServer="\\$QAServerR"
                $QAClient="\\$QAClientR"
                $exist=$true
                Break
          }ELSEIF(($Servers[$c] -eq "CCTESTNET")){ 
                $IS_NET_API_LOGON_USERNAME1="$qainstall"
                $IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
                $SQL_SERVER1="BERSV-SQL8QA"
                $SQL_USER1="EcmDbUser"
                $SQL_PASSW1="EcmDbUser"
                $DB_SYSTEM1="QASystem_CCNET"
                $DB_ECM1="QAECM_CCNET"
                $DB_PRICES1="QAPRICES_CCNET"
                $DB_DWH1="QADatawareHouse_CCNET"
                $DB_FWC1="QAForwardCurve_CCNET"
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
			}
        Break
        }ELSE{
            $c++
        }	
    }

}
IF($exist -eq $true){
    Echo_msg "$output_file" "[SUCCESS]:Application server  $QAServerR is availabe "
}ELSE{
    Echo_msg "$output_file" "[ERROR]:$CompName is not exist in the Servers list"
    Send_Error_Message "[ERROR]:$CompName is not exist in the Servers list Check the Nightly patch Script"
    exit
}
#########################################################################################################################################################
 
 # sending a message to QA
 Send_Start_message 

######################################################################
#Delete logfiles
Echo_msg "$output_file" "[INFO]:Deleting old log files........."
DEL "$QAServer\elviz\VizInstallLog*"
DEL "$QAServer\elviz\VizUninstallLog*"
# copying setup file to teh C: drive
Echo_msg "$output_file" "[INFO]:copying setup.exe to Temp folder........."
Copy-Item "$SetupPath" "c:\"  | Out-Null
$copiedSetup="c:\setup.exe"
IF(Test-Path $SetupPath){

    Echo_msg "$output_file" "[SUCCESS]:$copiedSetup copied to c:\"
}ELSE{
    Echo_msg "$output_file" "[ERROR]:$SetupPath this path does not exist, check the network connection"
    Send_Error_Message "[ERROR]:THE SETUP FILE WAS NOT FOUND OR NOT COPIED IN C:\SETUP.EXE"
    exit
}
Echo_msg "$output_file" "[INFO]:Check the connection to the application server"
# test connection the the machines
$AppServerExist = Ping-Address ($QAServerR)

IF ( $AppServerExist ){
    Echo_msg "$output_file" "[SUCCESS]:Connected to the application server $QAServerR" 
}ELSE{ 
    Echo_msg "$output_file" "[ERROR]:Cannot connect to the application server $QAServerR"
    Send_Error_Message "[ERROR]:COULD NOT CONNECT TO THE APPLICATION SERVER"
    exit
}
$ClientExist = Ping-Address ($QAClientR)
Echo_msg "$output_file" "[INFO]:Check the connection to the client machine"
IF ( $ClientExist ){
    Echo_msg "$output_file" "[SUCCESS]:Connected to the Client machine $QAClientR"
}ELSE{ 
    Echo_msg "$output_file" "[ERROR]:Cannot connect to the Client machine $QAClientR"
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

Echo_msg "$output_file" "[INFO]:Killing Elviz Processes on the application server..."
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
    Echo_msg "$output_file" "[INFO]:Terminating $obj_name "
    $terminate =$Process_object.Terminate()
}
FOREACH ($Process_object in $Processes_objects)
{
    $s= Get-Process -computername $QAServerR -Id $Process_object.ProcessId -ErrorAction SilentlyContinue
    if($s)
    {			
       $obj_name=$Process_object.Name
       $Obj_PID=$Process_object.ProcessId
       Echo_msg "$output_file" "[INFO]:Killing  $obj_name which has PID:$Obj_PID"
       & taskkill -PID $Process_object.ProcessId | Out-Null
  }
}

FOREACH ($Process_object in $Processes_objects)
{
$PID_Exist= Get-Process -computername $QAServerR -Id $Process_object.ProcessId -ErrorAction SilentlyContinue
$PID_Exist_name= $Process_object.Name
    if($PID_Exist)
    {	
        Echo_msg "$output_file" "[WARN]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
    }
}
Echo_msg "$output_file" "[INFO]:Stopping Elviz Curve Server Services on the application server...."
$sNames =@("Elviz Message Queue Listener Service","Elviz File Watching Service","Elviz ETRM","Elviz Curve Server Services","Elviz Priceboard Service")
FOREACH($sName IN $sNames){
    $service = get-service -display $sName -ErrorAction SilentlyContinue 
    IF( ! $service ){ 
        $msg1= "[INFO]:" + $sName + " was not installed on this computer." 
        Echo_msg "$output_file" $msg1
        } 
    ELSE{ 
        Stop-Service -Name $sName
        Start-Sleep -Seconds 10
        $msg1 = "[INFO]:" + $sName + "'s status is: " + $service.Status 
        Echo_msg "$output_file" $msg1
    }
}




IF($QAClientR -ne ""){
    Echo_msg "$output_file" "[INFO]:Kill all Elviz applications on the Client machine"
    $Processes_objects = @()
		FOREACH($process_name in $processes_names){
		    $process_name = get-wmiobject win32_process -computername $QAServerR | where {$_.Name -like "$process_name"}
		    if($process_name -ne $null){
		        $Processes_objects=$Processes_objects + $process_name
		    }
		}

		FOREACH($Process_object in $Processes_objects){
		    $obj_name=$Process_object.Name
		    Echo_msg "$output_file" "[INFO]:Terminating $obj_name "
		    $Process_object.Terminate()
		}
		FOREACH ($Process_object in $Processes_objects)
		{
		    $s= Get-Process -computername $QAServerR -Id $Process_object.ProcessId 
		    if($s)
		    {			
		       $obj_name=$Process_object.Name
		       $Obj_PID=$Process_object.ProcessId
		       Echo_msg "$output_file" "[INFO]:Killing  $obj_name which has PID:$Obj_PID"
		       taskkill -PID $Process_object.ProcessId
		  }
		}

		FOREACH ($Process_object in $Processes_objects)
		{
		$PID_Exist= Get-Process -computername $QAServerR -Id $Process_object.ProcessId 
		$PID_Exist_name= $Process_object.Name
		    if($PID_Exist)
		    {	
		        Echo_msg "$output_file" "[ERROR]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
		        Send_Error_Message "[ERROR]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
		        exit
		    }
		}
}
#logoff users in Client machine 
IF($QAClientR -ne ""){
	Echo_msg "$output_file" "[INFO]:Logging off all users on the client machine $QAClientR"
	#psexec /accepteula -u viz\$qainstall /P $qainstallpwd  $QAClient "$QASCRIPTS\logoff.bat" 
	logoff $QAClientR | Out-Null
	#Start-sleep 15
}
#########################################################################

Echo_msg "$output_file" "[INFO]:Uninstallation of Elviz ETRM on $QAServerR "

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
    Echo_msg "$output_file" "[INFO]:Deleting Elviz Subfolders "
        Foreach($Deleting_folder in $Deleting_folders){
            if (Test-Path "$QAServer\elviz\$Deleting_folder"){
                Del "$QAServer\elviz\$Deleting_folder" -Force -Recurse
                Echo_msg "$output_file" "[INFO]:Deleting $QAServer\elviz\$Deleting_folder"
				Start-Sleep -Seconds 10
            }
        }
        Foreach($Deleting_file in $Deleting_files){
            if (Test-Path "$QAServer\elviz\$Deleting_file"){
                Del "$QAServer\elviz\$Deleting_file" -Force -Recurse 
                Echo_msg "$output_file" "[INFO]:Deleting $QAServer\elviz\$Deleting_folder"
            }
        }
    #test if apps were removed
    FOREACH($Deleting_folder IN $Deleting_folders){
        IF(!(test-path "$QAServer\Elviz\$Deleting_folder")){
            $path_exist=$true
        }ELSE{
            Echo_msg "$output_file" "[ERROR]:this Folder was not removed: $QAServer\Elviz\$Deleting_folder"
            $path_exist=$false
            $Error_msg="[ERROR]:this Folder was not removed: $QAServer\Elviz\$Deleting_folder"
            break
        }
    }
	Echo_msg "$output_file" "[INFO]:Deleting Elviz tlb files "
	FOREACH($d in $delete_file_list){
		get-childitem "$QAServer\Elviz\Bin"  -include "*$d" -recurse | foreach ($_) {remove-item $_.fullname -Force  }
		Echo_msg "$output_file" "[INFO]:Delete $QAServer\Elviz\Bin\$_.fullname" 
	}
	FOREACH($d in $delete_file_list){
		$verify_Dbg_files = get-childitem "$QAServer\Elviz\Bin"  -include "*$d" -recurse
	}
	IF ($verify_Dbg_files -eq $null){
		Echo_msg "$output_file" "[INFO]:FINISHED Removing tlb files" 
	}
	
	
	
	
    If ($path_exist -eq $true){
            Echo_msg "$output_file" "[SUCCESS]:Elviz ETRM was removed successfully"
    }ELSE{
        Echo_msg "$output_file" "[ERROR]:Elviz ETRM was not removed successfully " 
        Send_Error_Message "$Error_msg"
        exit
    }
    Echo_msg "$output_file" "[WARN]:Elviz was not installed on this machine "
}ELSE{
    Echo_msg "$output_file" "[INFO]:Uninstalling Elviz........ (Min 2 minutes) "
    # Remove Elviz ETRM clsTransaction to Elviz ETRM
    # $product.Uninstall() 
    $app = Get-WmiObject -Class Win32_Product | Where-Object { 
    $_.Name -like "Elviz ETRM (Server)" 
    }
    $app.Uninstall() | Out-Null
#Start-Sleep -Seconds 180
    $Check_uninstall = Get-WmiObject -Class Win32_Product -Filter "name='Elviz ETRM (Server)'" -ComputerName "$QAServerR"
    IF( $Check_uninstall -eq $null){
        Echo_msg "$output_file" "[SUCCESS]:Elviz ETRM was removed Form Add\Remove programs"
    }Else{
        Echo_msg "$output_file" "[ERROR]:Elviz ETRM was not removed successfully" 
        Send_Error_Message "[ERROR]:ELVIZ WAS NOT REMOVED OF THE ADD\REMOVE PROGRAM, THE INSTALLATION PROCESS HAS BEEN STOPPED,"
        exit
    
    }
    Echo_msg "$output_file" "[INFO]:Deleting Elviz Subfolders "
        Foreach($Deleting_folder in $Deleting_folders){
            if (Test-Path "$QAServer\elviz\$Deleting_folder"){
                Del "$QAServer\elviz\$Deleting_folder" -Force -Recurse 
                Echo_msg "$output_file" "[INFO]:Deleting $QAServer\elviz\$Deleting_folder"
            }
        }
        Foreach($Deleting_file in $Deleting_files){
            if (Test-Path "$QAServer\elviz\$Deleting_file"){
                Del "$QAServer\elviz\$Deleting_file" -Force -Recurse 
                Echo_msg "$output_file" "[INFO]:Deleting $QAServer\elviz\$Deleting_folder"
            }
        }
		get-childitem c:\Elviz\Integration\ -include *.dll -recurse | foreach ($_) {remove-item $_.fullname -force}
		get-childitem c:\Elviz\Integration\ -include *.exe -recurse | foreach ($_) {remove-item $_.fullname -force}
    #test if apps were removed
    FOREACH($Deleting_folder IN $Deleting_folders){
        IF(!(test-path "$QAServer\Elviz\$Deleting_folder")){
            $path_exist=$true
        }ELSE{
            Echo_msg "$output_file" "[ERROR]:this Folder was not removed: $QAServer\Elviz\$Deleting_folder"
            $path_exist=$false
            $Error_msg="[ERROR]:this Folder was not removed: $QAServer\Elviz\$Deleting_folder"
            break
        }
    }
    
    If ($path_exist -eq $true){
            Echo_msg "$output_file" "[SUCCESS]:Elviz ETRM was removed successfully"
    }ELSE{
        Echo_msg "$output_file" "[ERROR]:Elviz ETRM was not removed successfully" 
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
Echo_msg "$output_file" "[INFO]:Installing Elviz Application server on $QAServerR "
# run the setup file
IF (($QAServerR -eq "QA122AR02")){
	& c:\Setup.exe /s /v" /qn ELVIZ_ETRM_IDENTITY_ACCOUNTTYPE_SELECTION=UserAccount IS_NET_API_LOGON_USERNAME=$IS_NET_API_LOGON_USERNAME1 IS_NET_API_LOGON_PASSWORD=$IS_NET_API_LOGON_PASSWORD1 SQL_SERVER=$SQL_SERVER1  SQL_USER=$SQL_USER1 SQL_PASSW=$SQL_PASSW1 DB_SYSTEM=$DB_SYSTEM1 DB_ECM=$DB_ECM1 DB_PRICES=$DB_PRICES1 DB_DWH=$DB_DWH1 DB_FWC=$DB_FWC1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=1 ELVIZ_DEAL_EVENTS=0 ELVIZ_WCF_PUBLISHER=1  ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=\\BERSV-SQL8QA\ECMimport\  BULKINSERTCLIENT=\\BERSV-SQL8QA\ECMimport\ " | Out-Null
}ELSE{
	& c:\Setup.exe /s /v" /qn ELVIZ_ETRM_IDENTITY_ACCOUNTTYPE_SELECTION=UserAccount IS_NET_API_LOGON_USERNAME=$IS_NET_API_LOGON_USERNAME1 IS_NET_API_LOGON_PASSWORD=$IS_NET_API_LOGON_PASSWORD1 SQL_SERVER=$SQL_SERVER1  SQL_USER=$SQL_USER1 SQL_PASSW=$SQL_PASSW1 DB_SYSTEM=$DB_SYSTEM1 DB_ECM=$DB_ECM1 DB_PRICES=$DB_PRICES1 DB_DWH=$DB_DWH1 DB_FWC=$DB_FWC1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=1 ELVIZ_DEAL_EVENTS=1 ELVIZ_WCF_PUBLISHER=1  ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=\\BERSV-SQL8QA\ECMimport\  BULKINSERTCLIENT=\\BERSV-SQL8QA\ECMimport\ " | Out-Null
}
# Start-Sleep -Seconds 540
 # satrt sharing service if it is stopped
 $Sharing_Services = get-service -display "Server" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
}ELSE{
    Start-Service -name "Server" > $null

}
 
 #Copying config files from Support to their locations
IF ($QAServerR -eq "QA122A"){
    ECHO ""| out-file -filepath $output_file -Append 
    Echo_msg "$output_file" "[INFO]:Copying config files from \\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\ Folder to their locations..." 

    IF(Test-Path "\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\PriceboardServerConfig\MontelApiFeeder.xml"){
        Echo_msg "$output_file" "[INFO]:Replacing  Configuration\PriceboardServerConfig\Adapters\MontelApiFeeder.xml"
        Copy-Item  "\\$DeployServer\INSTALLATIONS\QASCRIPTS\$Version\Config_Files\PriceboardServerConfig\MontelApiFeeder.xml" "C:\Elviz\Bin\Configuration\PriceboardServerConfig\Adapters\" -Force 
    }ELSE{
        Echo_msg "$output_file" "[ERROR]:C:\ElvizPatchScripts\Config_files\Configuration\PriceboardServerConfig\MontelApiFeeder.xml IS NOT EXIST"
    }
	

}
IF (($QAServerR -eq "QA122AR02")){
	    IF(Test-Path "$QASCRIPTS\Config_Files\Bin\AppSettings.config"){
		  Echo_msg "$output_file" "[INFO]:Replacing  $QASCRIPTS\Config_Files\Bin\AppSettings.config"
		  Copy-Item  "$QASCRIPTS\Config_Files\Bin\AppSettings.config" "C:\Elviz\Bin\Configuration\ServerConfig\" -Force 
		 }ELSE{
		  ECHO "[ERROR]:$QASCRIPTS\Config_Files\Bin\AppSettings.config IS NOT EXIST"
		 }
		 Set-ItemProperty "C:\Elviz\Bin\Configuration\ServerConfig\AppSettings.config"  -Name IsReadOnly -Value $false
	}

IF(Test-Path $LicencePath ){
    Echo_msg "$output_file" "[INFO]:Copying the License Reg to Elviz ETRM folder"
    Copy-Item  $LicencePath "C:\elviz\elviz ETRM\"
}ELSE{
    Echo_msg "$output_file" "[ERROR]:Elviz ETRM License was not copied, Check it is path $LicencePath"
}


$EUT_Info= "\\$DeployServer\INSTALLATIONS\DEP_TEMP\QALicenceReg\EUT_usernames_and_passwords.reg"
IF(Test-Path $EUT_Info ){
    Echo_msg "$output_file" "[INFO]:Copying the EUT Info to Elviz ETRM folder"
    Copy-Item  $EUT_Info "C:\elviz\elviz ETRM\"
}ELSE{
    Echo_msg "$output_file" "[ERROR]:EUT info file was not copied, Check it is path $LicencePath"
}

IF ($QAServerR -eq "QA122A") {

	Echo_msg "$output_file" "[INFO]-Reinstalling Web sales Manager"
	if(test-path "$WSMLocalpath"){DEL "$WSMLocalpath"}
	Copy-Item  $WSMPath "C:\" | Out-Null
	$product=Get-WmiObject -Class Win32_Product -Filter "name='Elviz Web Sales Manager'" -ComputerName "$QAServerR"
	IF( $product -eq $null){
	# if elviz not installed but still have junk files to delete

	}ELSE{
		Echo_msg "$output_file" "[INFO]-removing Web sales Manager"

		start-sleep 5
		& MsiExec /X  "{2BDDACBE-5326-458F-88B6-EBD7FBBD987D}" /qn | Out-Null
	}
	Echo_msg "$output_file" "[INFO]-Installing Web sales Manager"
	&cmd /c "msiexec /i `"$WSMLocalpath`" /quiet INSTALLDIR=C:\Elviz APP_SERVER=$QAServerR IS_SQLSERVER_SERVER=BERSV-SQL8QA IS_SQLSERVER_USERNAME=EcmDbUser IS_SQLSERVER_PASSWORD=EcmDbUser IS_SQLSERVER_DATABASE=QAWSM122 PORT_NUMBER=8009 BUYATC=1 AUTOREFRESH=1 BUYEC=0 USEFAILEDCURVES=1 USENONSYNCEDCURVES=1" | Out-Null


	Echo_msg "$output_file" "[INFO]-Reinstalling Elviz Clearing Reports Viewer"
	if(test-path "$CRVLocalpath"){DEL "$CRVLocalpath"}
	$product=Get-WmiObject -Class Win32_Product -Filter "name='Elviz Clearing Reports Viewer'" -ComputerName "$QAServerR"
	IF( $product -eq $null){
	# if elviz not installed but still have junk files to delete

	}ELSE{
		Echo_msg "$output_file" "[INFO]-removing Elviz Clearing Reports Viewer"

		Copy-Item  $CRVPath "C:\elviz\" | Out-Null
		start-sleep 5
		& MsiExec /X  "{DDE7CBB6-97CB-41D6-A1C7-D8EDE8AA99FF}" /qn | Out-Null
	}
	Echo_msg "$output_file" "[INFO]-Installing Elviz Clearing Reports Viewer"
	&cmd /c "msiexec /i `"$CRVLocalpath`" /quiet INSTALLDIR=C:\Elviz  IS_SQLSERVER_SERVER=BERSV-SQL8QA IS_SQLSERVER_USERNAME=EcmDbUser IS_SQLSERVER_PASSWORD=EcmDbUser IS_SQLSERVER_DATABASE=QACRV122 APP_SERVER=$QAServerR PORT_NUMBER=8009 NOMXHOSTIP=192.176.3.151 NOMXPORTNO=6124 NOMXUSERNAME=NPVIZTS_14 NOMXPASSWORD=Nasdaq12 CLEARONCONFIRM=1 IGNOREFEE=1 " | Out-Null



}


$Check_install = Get-WmiObject -Class Win32_Product -Filter "name='Elviz ETRM (Server)'" -ComputerName "$QAServerR"
Start-Sleep -Seconds 10
$installed=$false
IF( $Check_install -eq $null){
    Echo_msg "$output_file" "[ERROR]:Elviz ETRM was not installed successfully"
    Send_Error_Message "[ERROR]:ELVIZ WAS NOT INSTALLED ON THE APPLICATION SERVER, THE INSTALLATION PROCESS HAS BEEN STOPPED,"
    exit	
}ELSE{
    Echo_msg "$output_file" "[SUCCESS]:Elviz ETRM was installed successfully on the application server"
	$installed=$true
}
Remove-Item $copiedSetup -Force
Echo_msg "$output_file" "[INFO]:Setup file was deleted from the temp folder"

#########################################################################
Echo_msg "$output_file" "[INFO]:Starting Elviz Curve Server Services...."
Start-Service -name "Elviz Curve Server Services" > $null
Start-Sleep 5
$Viz_ETRM_Services = get-service -display "Elviz Curve Server Services" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
    Echo_msg "$output_file" "[INFO]:Elviz Curve Server Services status is : $ServiceStatus "
}ELSE{

    Echo_msg "$output_file" "[INFO]:Elviz Curve Server Services status is : $ServiceStatus "

}
Start-Service -name "Elviz Message Queue Listener Service" > $null
Start-Sleep 5
$Viz_ETRM_Services = get-service -display "Elviz Curve Server Services" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
    Echo_msg "$output_file" "[INFO]:Elviz Message Queue Listener Service status is : $ServiceStatus "
}ELSE{

    Echo_msg "$output_file" "[INFO]:Elviz Message Queue Listener Service status is : $ServiceStatus "

}

IF (($QAClientR -ne "") -and ($installed -eq $true)){
    Echo_msg "$output_file" "[INFO]:Running the Client install on $QAClientR "

    Echo_msg "$output_file" "[INFO]:Kill all Elviz applications on the Client machine"
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
		    Echo_msg "$output_file" "  [INFO]:Terminating $obj_name "
		    $Process_object.Terminate()
		}
		FOREACH ($Process_object in $Processes_objects)
		{
		    $s= Get-Process -computername $QAClientR -Id $Process_object.ProcessId 
		    if($s)
		    {			
		       $obj_name=$Process_object.Name
		       $Obj_PID=$Process_object.ProcessId
		       Echo_msg "$output_file" "  [INFO]:Killing  $obj_name which has PID:$Obj_PID"
		       taskkill -PID $Process_object.ProcessId
		  }
		}

		FOREACH ($Process_object in $Processes_objects)
		{
		$PID_Exist= Get-Process -computername $QAClientR -Id $Process_object.ProcessId 
		$PID_Exist_name= $Process_object.Name
		    if($PID_Exist)
		    {	
		        Echo_msg "$output_file" "[WARN]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
		        Send_Error_Message "[WARN]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
		        exit
		    }
		}

    # Check the app server installation to run the clientInstall.bat
    $Check_install = Get-WmiObject -Class Win32_Product -Filter "name='Elviz ETRM (Server)'" -ComputerName "$QAServerR"
    IF($Check_install -ne $null){
        Echo_msg "$output_file" "[INFO]:Starting the client install on $QAClient   "
		& schtasks /run /s $QAClientR  /tn ClientInstall
		Echo_msg "$output_file" "[WARN]:An Error message will be sent by mail if the client Instalaltion failed"
		Start-Sleep -s 240
    }ELSE{
        Echo_msg "$output_file" "[ERROR]:You cant run the client installation, check if the application server is installed and the client is available   [ERROR_MSG]"
    }
   Echo_msg "$output_file" "[INFO]:DISABLE The QA_DEPLOY scheduled tasks to prevent it to run after a normal reboot" 
   & schtasks /Change /ru viz\$qainstall /rp $qainstallpwd  /tn Run_QADEPLOY /DISABLE
   $end_date= Get-Date
    $msg2="
-------------------------------------------------------------------                
                 Installation is complete                   	
                 $end_date                                   
-------------------------------------------------------------------
        " 
  Echo_msg "$output_file" $msg2
  Send_End_Message
        
 #   }
}ELSE{
    Echo_msg "$output_file" "[INFO]:DISABLE The QA_DEPLOY scheduled tasks to prevent it to run after a normal reboot" 
    & schtasks /Change /ru $qainstall /rp $qainstallpwd  /tn Run_QADEPLOY /DISABLE
	Echo_msg "$output_file" "  [SUCCESS]:Elviz ETRM was installed successfully on the application server $QAServerR"
    $end_date= Get-Date
    $msg2="
-------------------------------------------------------------------                
                 Installation is complete                   	
                 $end_date                                   
-------------------------------------------------------------------
    " 
    Echo_msg "$output_file" $msg2
    Send_End_Message
}

# sometimes Server service is stopped, which prevent file sharing.
$Sharing_Services = get-service -display "Server" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
}ELSE{
    Start-Service -name "Server"  > $null

}

IF (($QAServerR -eq "QA122A")){
	"copy tests from the elviz release output"
	[System.Reflection.Assembly]::LoadFrom("C:\util\tfs\Microsoft.TeamFoundation.Client.dll")
	[System.Reflection.Assembly]::LoadFrom("C:\util\tfs\Microsoft.TeamFoundation.Build.Client.dll")	
	
	$elvizPath = "C:\Elviz"
	$serverName = "http://BERVS-TFS05:8080/tfs/BradyEnergy"
	$buildDefinition = "2012.2_IntegrationTests"
	$teamProject = "Elviz"
	
	$collection = [Microsoft.TeamFoundation.Client.TfsTeamProjectCollectionFactory]::GetTeamProjectCollection($serverName)
	$buildServer = $collection.GetService([Microsoft.TeamFoundation.Build.Client.IBuildServer])
	$defenition = $buildServer.GetBuildDefinition($teamProject, $buildDefinition)

	$latestBuildDetail = $buildServer.QueryBuilds("Elviz", "2012.2a_Elviz_Release")[-1]	
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

IF (($QAServerR -eq "QA122AR02KKKKK")){
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
