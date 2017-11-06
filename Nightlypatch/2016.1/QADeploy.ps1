 ############################################################################
 #
 #  						SAO Environment Script        
 #	This ia a nightly patch script to reinstall Elviz ETRM on QA machines
 #
 ############################################################################
cls
# ------- Script variables ---------------
."c:\ElvizPatchScripts\Functions.ps1"
$ElvizServerGuid ="{F03EAA70-F8A9-4BA2-8287-A7BF71DD5E9C}"
$setup="setup.exe" 
$Version="2016.1"
$VersionFolder="2016.1"
$DEployServer="NETVS-DEPLOY"
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


#find the computer name
$QAServerR="$Env:ElvizApplServer"
$QAClientR="$Env:ElvizApplClient"
$Log_Date= Get-Date
$DB_DRD1=""
 out-file -filepath $output_file -inputobject "  " 
Start-Sleep -Seconds 10
Echo_msg "$output_file" "------------------------------------------------------------"
Echo_msg "$output_file" "-                                                          "
Echo_msg "$output_file" "-           Nightly Patch For QA Environment               "
Echo_msg "$output_file" "-           Copy Rights Viz Risk Management                "
Echo_msg "$output_file" "-              Insalling version $Version on $QAServerR "
Echo_msg "$output_file" "-                                                          "
Echo_msg "$output_file" "------------------------------------------------------------"
Echo_msg "$output_file" "[INFO] Application server: $QAServerR"
Echo_msg "$output_file" "[INFO] Client Machine: $QAClientR"
Echo_msg "$output_file" "[INFO]:Check the connection to the application server $QAServerR"
# test connection the the machines
$AppServerExist = Ping-Address ($QAServerR)

IF ( $AppServerExist ){
	Echo_msg "$output_file" "[SUCCESS]:Connected to the application server $QAServerR" 
}ELSE{ 
	Echo_msg "$output_file" "[ERROR]:Cannot connect to the application server $QAServerR"
	Send_Error_Message "[ERROR]:COULD NOT CONNECT TO THE APPLICATION SERVER"
	exit
}
if ($QAClientR -ne ""){
	$ClientExist = Ping-Address ($QAClientR)
	Echo_msg "$output_file" "[INFO]:Check the connection to the client machine"
	IF ( $ClientExist ){
		Echo_msg "$output_file" "[SUCCESS]:Connected to the Client machine $QAClientR"
	}ELSE{ 
		Echo_msg "$output_file" "[ERROR]:Cannot connect to the Client machine $QAClientR"
		Send_Error_Message "[ERROR]:COULD NOT CONNECT TO THE CLEINT MACHINE $QAClientR"
		exit
	}
}


$exist=$false
IF(($QAServerR -eq "NETVS-QA161A") -or ($QAServerR -eq "NETVS-ISDEV") ){ 
	$IS_NET_API_LOGON_USERNAME1="$qainstall"
	$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
	$SQL_SERVER1="NETSV-DBS12QA"
	$DB_SYSTEM1="QASystem161"
	$DB_ECM1="QAECM161"
	$DB_PRICES1="QAPRICES161"
	$DB_DWH1="QADatawareHouse161"
	$DB_DRD1="QAReporting161"
	
	# ***** to UNC path 
	$QAServer="\\$QAServerR"
	$QAClient="\\$QAClientR"
	$BulkInsertPath="\\NETSV-DBS12QA\ECMimport\161\"
	$exist=$true
}
IF(($QAServerR -eq "NETVS-QA161A03")){ 
	$IS_NET_API_LOGON_USERNAME1="$qainstall"
	$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
	$SQL_SERVER1="NETSV-DBS12REG"
	$DB_SYSTEM1="QASystem_Reg161"
	$DB_ECM1="QAECM_Reg161"
	$DB_PRICES1="QAPrices_Reg161"
	$DB_DWH1="QADatawareHouse_Reg161"
	$DB_DRD1="QAReporting_Reg161"
	# ***** to UNC path 
	$QAServer="\\$QAServerR"
	$QAClient="\\$QAClientR"
	$BulkInsertPath="\\NETSV-DBS12REG\ECMimport\161\"
	$exist=$true
}

IF(($QAServerR -eq "NETVS-QADEV01")){ 
	$IS_NET_API_LOGON_USERNAME1="$qainstall"
	$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
	$SQL_SERVER1="NETSV-DBS12DEV\DEVQA"
	$SQL_USER1="EcmDbUser"
	$SQL_PASSW1="EcmDbUser"
	$DB_SYSTEM1="DAILY_QASystem_Reg161"
	$DB_ECM1="DAILY_QAECM_Reg161"
	$DB_PRICES1="DAILY_QAPrices_Reg161"
	$DB_DWH1="DAILY_QADatawareHouse_Reg161"
	# ***** to UNC path 
	$QAServer="\\$QAServerR"
	$QAClient="\\$QAClientR"
	$BulkInsertPath="\\NETSV-DBS12DEV\BulkInsert\"
	$exist=$true
}
IF(($QAServerR -eq "NETVS-QADEV02")){ 
	$IS_NET_API_LOGON_USERNAME1="$qainstall"
	$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
	$SQL_SERVER1="NETSV-DBS12DEV\DEVQA"
	$SQL_USER1="EcmDbUser"
	$SQL_PASSW1="EcmDbUser"
	$DB_SYSTEM1="DAILY_QASystem161"
	$DB_ECM1="DAILY_QAECM161"
	$DB_PRICES1="DAILY_QAPrices161"
	$DB_DWH1="DAILY_QADatawareHouse161"
	# ***** to UNC path 
	$QAServer="\\$QAServerR"
	$QAClient="\\$QAClientR"
	$BulkInsertPath="\\NETSV-DBS12DEV\BulkInsert\"
	$exist=$true
}
IF(($QAServerR -eq "NETVS-QADEV03")){ 
	$IS_NET_API_LOGON_USERNAME1="$qainstall"
	$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
	$SQL_SERVER1="NETSV-DBS12DEV\DEVQA"
	$SQL_USER1="EcmDbUser"
	$SQL_PASSW1="EcmDbUser"
	$DB_SYSTEM1="DAILY_VizSystem_161"
	$DB_ECM1="DAILY_VizECM_161"
	$DB_PRICES1="DAILY_VizPrices_161" 
	$DB_DWH1="DAILY_VizDatawareHouse_161"
	$DB_DRD1=""
	# ***** to UNC path 
	$QAServer="\\$QAServerR"
	$QAClient="\\$QAClientR"
	$BulkInsertPath="\\NETSV-DBS12DEV\BulkInsert\"
	$exist=$true
}
IF(($QAServerR -eq "NETVS-QA161A02")){ 
	$IS_NET_API_LOGON_USERNAME1="$qainstall"
	$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
	$SQL_SERVER1="NETSV-DBS12REG"
	$DB_SYSTEM1="VizSystem_161"
	$DB_ECM1="VizECM_161"
	$DB_PRICES1="VizPrices_161"
	$DB_DWH1="VizDatawareHouse_161"
	$DB_DRD1="VizReporting_161"
	# ***** to UNC path 
	$QAServer="\\$QAServerR"
	$QAClient="\\$QAClientR"
	$BulkInsertPath="\\NETSV-DBS12REG\ECMimport\161\"
	$exist=$true
}

IF(($QAServerR -eq "BERPC-PERFA")){ 
 $IS_NET_API_LOGON_USERNAME1="$qainstall"
	$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
	$SQL_SERVER1="NETSV-DBS12TST"
	$SQL_USER1="EcmDbUser"
	$SQL_PASSW1="EcmDbUser"
	$DB_SYSTEM1="PERFASystem"
	$DB_ECM1="PERFAECM"
	$DB_PRICES1="PERFAPrices"
	$DB_DWH1="PERFADwh"
	# ***** to UNC path 
	$QAServer="\\$QAServerR"
	$QAClient="\\$QAClientR"
	$BulkInsertPath="\\NETSV-DBS12TST\BulkInsert\"
	$exist=$true
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
# copying setup file to teh C: drive
Echo_msg "$output_file" "[INFO]:copying setup.exe to Temp folder........."
Copy-Item "$SetupPath" "c:\"  | Out-Null
$copiedSetup="c:\setup.exe"
IF(Test-Path $copiedSetup){

	Echo_msg "$output_file" "[SUCCESS]:$copiedSetup copied to c:\"
}ELSE{
	Echo_msg "$output_file" "[ERROR]:$SetupPath this path does not exist, check the network connection"
	Send_Error_Message "[ERROR]:THE SETUP FILE WAS NOT FOUND OR NOT COPIED IN C:\SETUP.EXE"
	exit
}


Echo_msg "$output_file" "[INFO]:Killing Elviz Processes on the application server..."
$processes_names=@("TestComplete.exe","TestExecute.exe","Viz.Integration.Core.MessageQueueListener.exe","Viz.Integration.Core.FileWatcher.exe","Viz.Integration.Core.WCFPublisher.exe","Brady.Etrm.MiddleWare.ETRMWinService.exe","Brady.Etrm.MiddleWare.Priceboard.WinService.exe","TestExecute.exe","ElvizETRMClient.exe","ElvizDM.exe","ElvizFM.exe","ElvizCM.exe","ElvizTM.exe","ElvizLM.exe")
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
$sNames =@("Brady.ETRM.Message.Queue.Listener","Elviz Message Queue Listener Service","Brady.ETRM.File.Watching","Elviz File Watching Service","Brady.ETRM.WCF.Publishing","Brady.ETRM","Brady.ETRM.Priceboard","Elviz Curve Server Services","Elviz Priceboard Service","ActiveMQ")
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
if (Test-Path "C:\activemq\data"){
	Remove-Item "C:\activemq\data" -Force -Recurse
	Start-Service -name "ActiveMQ"  > $null
}




IF($QAClientR -ne ""){
	Echo_msg "$output_file" "[INFO]:Kill all Brady applications on the Client machine"
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
		if($s){			
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
			Echo_msg "$output_file"		"[ERROR]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
			Send_Error_Message "[ERROR]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
			exit
		}
	}
	#logoff users in Client machine 
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

$regkey32= "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$ElvizServerGuid" 
$regkey64= "HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\$ElvizServerGuid"
if(!(Test-Path $regkey32) -and !(Test-Path $regkey64) ) {
	Echo_msg "$output_file" "[WARN]:Elviz was not installed on this machine "
}else{
	Echo_msg "$output_file" "[INFO]:Uninstalling Elviz ETRM Server........  "
	Echo_msg "$output_file" "************** Uninstallation errors log start  here ************** "
	& msiexec /qn /lwme! "$output_file" /Uninstall $ElvizServerGuid /norestart | Out-Null

	Echo_msg "$output_file" "************** End of Uninstallation errors log  ************** "
	if(!(Test-Path $regkey32) -and !(Test-Path $regkey64) ) {
		 Echo_msg "$output_file" "[SUCCESS]:Elviz ETRM was removed Form Add\Remove programs"
	}else{
		Echo_msg "$output_file" "[ERROR]:Elviz ETRM was not removed successfully" 
		Send_Error_Message "[ERROR]:ELVIZ WAS NOT REMOVED OF THE ADD\REMOVE PROGRAM, THE INSTALLATION PROCESS HAS BEEN STOPPED,"
		exit
	}
}

 # start sharing service if it is stopped
$Sharing_Services = get-service -display "Server" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
}ELSE{
	Start-Service -name "Server"  > $null

}
#######################################################################
Echo_msg "$output_file" "[INFO]:SQL Server infromation: "
Echo_msg "$output_file" "SQL server: $SQL_SERVER1"
Echo_msg "$output_file" "SQL user: $SQL_USER1"
Echo_msg "$output_file" "System Db: $DB_SYSTEM1"
Echo_msg "$output_file" "ECM Db: $DB_ECM1"
Echo_msg "$output_file" "PricesDB: $DB_PRICES1"
Echo_msg "$output_file" "Datawrehouse DB: $DB_DWH1"
Echo_msg "$output_file" "ForwardCurve DB: $DB_FWC1"
Echo_msg "$output_file" "Applicartion Server: $QAServer"
Echo_msg "$output_file" "Client machine: $QAClient"
Echo_msg "$output_file" "Bulk Insert path: $BulkInsertPath"
# run the setup file
Echo_msg "$output_file" "[INFO]:Installing Brady ETRM Application server on $QAServerR "
Echo_msg "$output_file" "************** Installation errors log start  here ************** "
IF ($QAServerR -eq "BERPC-PERFA"){
	& c:\Setup.exe /s /v"/lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1  IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616 ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath BULKINSERTCLIENT=$BulkInsertPath " | Out-Null
	Echo_msg "$output_file"  "& c:\Setup.exe /s /v/lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1  IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616 ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath BULKINSERTCLIENT=$BulkInsertPath "
 }ELSEIF (($QAServerR -eq "NETVS-QA161A")){
	& c:\Setup.exe /s /v"/lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1  IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 IS_SQLSERVER_DBRD=$DB_DRD1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616 ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath BULKINSERTCLIENT=$BulkInsertPath " | Out-Null
	Echo_msg "$output_file"  "& c:\Setup.exe /s /v/lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1  IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 IS_SQLSERVER_DBRD=$DB_DRD1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616 ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath BULKINSERTCLIENT=$BulkInsertPath "
 }
 Else{
	 if ($DB_DRD1 -eq ""){
		& c:\Setup.exe /s /v" /lwme!+ $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1 IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616  ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath  BULKINSERTCLIENT=$BulkInsertPath " | Out-Null
		 Echo_msg "$output_file"  "& c:\Setup.exe /s /v /lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1 IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1  IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616  ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath  BULKINSERTCLIENT=$BulkInsertPath "
		 Set-ItemProperty "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\NETSV-DBS12REG" -Name "DefaultUser" -Value "EcmDbUser"
		Set-ItemProperty "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\NETSV-DBS12REG" -Name "DefaultPw" -Value "EcmDbQaReg"
	}Else{
		& c:\Setup.exe /s /v" /lwme!+ $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1 IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 IS_SQLSERVER_DBRD=$DB_DRD1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616  ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath  BULKINSERTCLIENT=$BulkInsertPath " | Out-Null
		 Echo_msg "$output_file"  "& c:\Setup.exe /s /v /lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1 IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBRD=$DB_DRD1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616  ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath  BULKINSERTCLIENT=$BulkInsertPath "
		 Set-ItemProperty "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\NETSV-DBS12REG" -Name "DefaultUser" -Value "EcmDbUser"
		Set-ItemProperty "HKLM:\SOFTWARE\Wow6432Node\VIZ Risk Management Services\Elviz\Contract Manager\NETSV-DBS12REG" -Name "DefaultPw" -Value "EcmDbQaReg"
	}
}
Echo_msg "$output_file" "************** End of Installation errors log  ************** "
# Start-Sleep -Seconds 540
 # satrt sharing service if it is stopped
 $Sharing_Services = get-service -display "Server" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
}ELSE{
	Start-Service -name "Server" > $null

}
IF ($QAServerR -eq "BERPC-PERFA"){
	Echo_msg "$output_file" "[INFO]:Stopping Brady Message Queue Listener Service on the application server...."
	$sNames =@("Brady ETRM Message Queue Listener")
	FOREACH($sName IN $sNames){
		$service = get-service -display $sName -ErrorAction SilentlyContinue 
		IF( ! $service ){ 
			$msg1= "[INFO]:" + $sName + " was not installed on this computer." 
			Echo_msg "$output_file" $msg1
			} 
		ELSE{ 
			Stop-Service -Name $sName
			#Start-Sleep -Seconds 10
			$msg1 = "[INFO]:" + $sName + "'s status is: " + $service.Status 
			Echo_msg "$output_file" $msg1
		}
	}
}

IF(Test-Path $LicencePath ){
	Echo_msg "$output_file" "[INFO]:Copying the License Reg to Brady ETRM folder"
	Copy-Item  $LicencePath "C:\elviz\Brady ETRM\"
}ELSE{
	Echo_msg "$output_file" "[ERROR]:Brady ETRM License was not copied, Check it is path $LicencePath"
}


$EUT_Info= "\\$DeployServer\INSTALLATIONS\DEP_TEMP\QALicenceReg\EUT_usernames_and_passwords.reg"
IF(Test-Path $EUT_Info ){
	Echo_msg "$output_file" "[INFO]:Copying the EUT Info to Elviz ETRM folder"
	Copy-Item  $EUT_Info "C:\elviz\Brady ETRM\"
}ELSE{
	Echo_msg "$output_file" "[ERROR]:EUT info file was not copied, Check it is path $LicencePath"
}

$installed=$false
$regkey= "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{F03EAA70-F8A9-4BA2-8287-A7BF71DD5E9C}" 
if(Test-Path $regkey) {
	Echo_msg "$output_file" "[SUCCESS]:Elviz ETRM was installed successfully on the application server"
	$installed=$true
}else{
	Echo_msg "$output_file" "[ERROR]:Brady ETRM was not installed successfully"
	Send_Error_Message "[ERROR]:ELVIZ WAS NOT INSTALLED ON THE APPLICATION SERVER, THE INSTALLATION PROCESS HAS BEEN STOPPED,"
	exit
}


Echo_msg "$output_file" "[INFO]:Create Brady ETRM if not exist"
$CurveServer = "Brady.ETRM"
$CurveServerDisplay = "Brady ETRM"
$binaryPath="C:\Elviz\Bin\Brady.Etrm.MiddleWare.ETRMWinService.exe"
$ECSExist = get-service -display $CurveServer -ErrorAction SilentlyContinue 
if($ECSExist -eq ""){
	 New-Service -name $CurveServer -binaryPathName $binaryPath -displayName $CurveServerDisplay -DependsOn "LanmanServer" -Description $CurveServerDisplay  -startupType Automatic 
}else{
	Echo_msg "$output_file" "[INFO]:Brady ETRM is already Exist "
}
Remove-Item $copiedSetup -Force
Echo_msg "$output_file" "[INFO]:Setup file was deleted from the temp folder"

#########################################################################
Echo_msg "$output_file" "[INFO]:Starting Brady.ETRM...."
Start-Service -name "Brady.ETRM" > $null
Start-Sleep 5
$Viz_ETRM_Services = get-service -display "Brady ETRM" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
	Echo_msg "$output_file" "[INFO]:Brady ETRM status is : $ServiceStatus "
}ELSE{

	Echo_msg "$output_file" "[INFO]:Brady ETRM status is : $ServiceStatus "

}
IF ($QAServerR -ne "BERPC-PERFA"){
	Start-Service -name "Brady.ETRM.Message.Queue.Listener" > $null

	Start-Sleep 5
	$Viz_ETRM_Services = get-service -display "Brady ETRM Message Queue Listener" -ErrorAction SilentlyContinue 
	$ServiceStatus = $Viz_ETRM_Services.Status
	IF($ServiceStatus -eq "Running"){
	  Echo_msg "$output_file" "	[INFO]:Brady ETRM Message Queue Listener status is : $ServiceStatus "
	}ELSE{
		Echo_msg "$output_file" "[INFO]:Brady ETRM Message Queue Listenerstatus is : $ServiceStatus "

	}
}
IF (($QAClientR -ne "") -and ($installed -eq $true)){
	Echo_msg "$output_file" "[INFO]:Running the Client install on $QAClientR "

	Echo_msg "$output_file" "[INFO]:Kill all Brady applications on the Client machine"
	$processes_names=@("TestComplete.exe","TestExecute.exe","Viz.Windows.ETRM.exe","ElvizDM.exe","ElvizRM.exe","ElvizFM.exe","ElvizCM.exe","ElvizTM.exe","ElvizLM.exe","Brady.Etrm.MiddleWare.ETRMWinService.exe","Brady.Etrm.MiddleWare.Priceboard.WinService.exe")
	$Processes_objects = @()
	FOREACH($process_name in $processes_names){
		$process_name = get-wmiobject win32_process -computername $QAClientR | where {$_.Name -like "$process_name"}
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
	$Check_install = Get-WmiObject -Class Win32_Product -Filter "name='Brady ETRM (Server)'" -ComputerName "$QAServerR"
	IF($Check_install -ne $null){
		Echo_msg "$output_file" "[INFO]:Starting the client install on $QAClient   "
		& schtasks /run /s $QAClientR  /tn ClientInstall
		Echo_msg "$output_file" "[WARN]:An Error message will be sent by mail if the client Installation failed"
		Start-Sleep -s 120
	}ELSE{
		Echo_msg "$output_file" "[ERROR]:You cant run the client installation, check if the application server is installed and the client is available   [ERROR_MSG]"
	}
	Echo_msg "$output_file" "[INFO]:DISABLE The QA_DEPLOY scheduled tasks to prevent it to run after a normal reboot" 
	& schtasks /Change /ru viz\$qainstall /rp $qainstallpwd  /tn Run_QADEPLOY /DISABLE
	$end_date= Get-Date
	$msg2="-------------------------------------------------------------------                
				 Installation is complete                   	
				 $end_date                                   
-------------------------------------------------------------------
		" 
	Echo_msg "$output_file" $msg2
 }ELSE{
	Echo_msg "$output_file" "[INFO]:DISABLE The QA_DEPLOY scheduled tasks to prevent it to run after a normal reboot" 
	& schtasks /Change /ru $qainstall /rp $qainstallpwd  /tn Run_QADEPLOY /DISABLE
	Echo_msg "$output_file" "[SUCCESS]:Brady ETRM was installed successfully on the application server $QAServerR"
	$end_date= Get-Date
	$msg2="
-------------------------------------------------------------------                
				 Installation is complete                   	
				 $end_date                                   
-------------------------------------------------------------------
	" 
	Echo_msg "$output_file" $msg2
}
IF ($installed -eq $true){
	Send_End_Message
}
# sometimes Server service is stopped, which prevent file sharing.
$Sharing_Services = get-service -display "Server" -ErrorAction SilentlyContinue 
$ServiceStatus = $Viz_ETRM_Services.Status
IF($ServiceStatus -eq "Running"){
}ELSE{
	Start-Service -name "Server"  > $null

}
& schtasks /Change /ru $qainstall /rp $qainstallpwd  /tn Run_QADEPLOY /DISABLE
