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
$Version="2015.2"
$VersionFolder="2015.2"

$DEployServer="NETVS-DEPLOY"
$Servers=@("NETVS-QA152A","NETVS-QA152A01D","NETVS-QA152A01","NETVS-QA152A02","BERPC-PERFA","NETVS-ISDEV")
$Clients=@("NETVS-QA152C","","NETVM-QA152C01","NETVM-QA152C02","","")
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
$CompName="$Env:ElvizapplServer"
$Log_Date= Get-Date
 out-file -filepath $output_file -inputobject "  " 
Start-Sleep -Seconds 10
Echo_msg "$output_file" "------------------------------------------------------------"
Echo_msg "$output_file" "-                                                          -"
Echo_msg "$output_file" "-           Nightly Patch For QA Environment               -"
Echo_msg "$output_file" "-           Copy Rights Viz Risk Management                -"
Echo_msg "$output_file" "-              Insalling version $Version                  -"
Echo_msg "$output_file" "-                                                          -"
Echo_msg "$output_file" "------------------------------------------------------------"
# set the variables of the computer found
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
			$DB_SYSTEM1="QAREGSystem152"
			$DB_ECM1="QAREGECMOLD152"
			$DB_PRICES1="QAREGPRICES152"
			$DB_DWH1="QADataWarehouse152"
			#------- Machines names -----------
			$QAServerR=$Servers[$c]
			$QAClientR=$Clients[$c]
			# ***** to UNC path 
			$QAServer="\\$QAServerR"
			$QAClient="\\$QAClientR"
			$BulkInsertPath="\\BERSV-SQL8QA\ECMimport\142\"
			$exist=$true
		IF(($Servers[$c] -eq "NETVS-QA152A")){ 
				$IS_NET_API_LOGON_USERNAME1="$qainstall"
				$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
				$SQL_SERVER1="NETSV-DBS12QA"
				$DB_SYSTEM1="QASystem152"
				$DB_ECM1="QAECM152"
				$DB_PRICES1="QAPRICES152"
				$DB_DWH1="QADatawareHouse152"

				#------- Machines names -----------
				$QAServerR=$Servers[$c]
				$QAClientR=$Clients[$c]
				# ***** to UNC path 
				$QAServer="\\$QAServerR"
				$QAClient="\\$QAClientR"
				$BulkInsertPath="\\NETSV-DBS12QA\ECMimport\152\"
				$exist=$true
				Break
		}ELSEIF(($Servers[$c] -eq "NETVS-ISDEV")){ 
			$IS_NET_API_LOGON_USERNAME1="$qainstall"
				$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
				$SQL_SERVER1="NETSV-DBS12QA"

				$DB_SYSTEM1="ISDEV_System"
				$DB_ECM1="ISDEV_ECM"
				$DB_PRICES1="ISDEV_Prices"
				$DB_DWH1="ISDEV_DWH"

				#------- Machines names -----------
				$QAServerR=$Servers[$c]
				$QAClientR=$Clients[$c]
				# ***** to UNC path 
				$QAServer="\\$QAServerR"
				$QAClient="\\$QAClientR"
				$BulkInsertPath="\\NETSV-DBS12QA\ECMimport\152\"
				$exist=$true
				Break
		}ELSEIF(($Servers[$c] -eq "NETVS-QA152A01")){ 
			 $IS_NET_API_LOGON_USERNAME1="$qainstall"
				$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
				$SQL_SERVER1="NETSV-DBS12REG"
				$SQL_USER1="EcmDbUser"
				$SQL_PASSW1="EcmDbQaReg"
				$DB_SYSTEM1="QASystem_Reg152"
				$DB_ECM1="QAECM_Reg152"
				$DB_PRICES1="QAPrices_Reg152"
				$DB_DWH1="QADatawareHouse_Reg152"
				#------- Machines names -----------
				$QAServerR=$Servers[$c]
				$QAClientR=$Clients[$c]
				# ***** to UNC path 
				$QAServer="\\$QAServerR"
				$QAClient="\\$QAClientR"
				$BulkInsertPath="\\NETSV-DBS12REG\ECMimport\152\"
				$exist=$true
				Break
		}ELSEIF(($Servers[$c] -eq "NETVS-QA152A01D")){ 
			 $IS_NET_API_LOGON_USERNAME1="$qainstall"
				$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
				$SQL_SERVER1="NETSV-DBS12DEV\DEVQA"
				$SQL_USER1="EcmDbUser"
				$SQL_PASSW1="EcmDbUser"
				$DB_SYSTEM1="DAILY_QASystem_Reg152"
				$DB_ECM1="DAILY_QAECM_Reg152"
				$DB_PRICES1="DAILY_QAPrices_Reg152"
				$DB_DWH1="DAILY_QADatawareHouse_Reg152"
				#------- Machines names -----------
				$QAServerR=$Servers[$c]
				$QAClientR=$Clients[$c]
				# ***** to UNC path 
				$QAServer="\\$QAServerR"
				$QAClient="\\$QAClientR"
				$BulkInsertPath="\\NETSV-DBS12DEV\BulkInsert\"
				$exist=$true
				Break
		}ELSEIF(($Servers[$c] -eq "NETVS-QA152A02")){ 
			 $IS_NET_API_LOGON_USERNAME1="$qainstall"
				$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
				$SQL_SERVER1="NETSV-DBS12REG"
				$DB_SYSTEM1="VizSystem_152"
				$DB_ECM1="VizECM_152"
				$DB_PRICES1="VizPrices_152"
				$DB_DWH1="VizDatawareHouse_152"
				#------- Machines names -----------
				$QAServerR=$Servers[$c]
				$QAClientR=$Clients[$c]
				# ***** to UNC path 
				$QAServer="\\$QAServerR"
				$QAClient="\\$QAClientR"
				$BulkInsertPath="\\NETSV-DBS12REG\ECMimport\152\"
				$exist=$true
				Break
		}
		ELSEIF(($Servers[$c] -eq "BERPC-PERFA")){ 
			 $IS_NET_API_LOGON_USERNAME1="$qainstall"
				$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
				$SQL_SERVER1="NETSV-DBS12TST"
				$SQL_USER1="EcmDbUser"
				$SQL_PASSW1="EcmDbUser"
				$DB_SYSTEM1="PERFASystem"
				$DB_ECM1="PERFAECM"
				$DB_PRICES1="PERFAPrices"
				$DB_DWH1="PERFADwh"
				#------- Machines names -----------
				$QAServerR=$Servers[$c]
				$QAClientR=$Clients[$c]
				# ***** to UNC path 
				$QAServer="\\$QAServerR"
				$QAClient="\\$QAClientR"
				$BulkInsertPath="\\NETSV-DBS12TST\BulkInsert\"
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
$sNames =@("Elviz Message Queue Listener Service","Elviz File Watching Service","Elviz Curve Server Services","Elviz Priceboard Service","ActiveMQ")
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
				Echo_msg "$output_file"		"[ERROR]:Cannot kill the process $PID_Exist_name with PID number $PID_Exist"
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
Echo_msg "$output_file" "[INFO]:Installing Elviz Application server on $QAServerR "
Echo_msg "$output_file" "************** Installation errors log start  here ************** "
IF ($QAServerR -eq "BERPC-PERFA"){
	& c:\Setup.exe /s /v"/lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1  IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616 ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath BULKINSERTCLIENT=$BulkInsertPath " | Out-Null
	Echo_msg "$output_file"  "& c:\Setup.exe /s /v/lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1  IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616 ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath BULKINSERTCLIENT=$BulkInsertPath "
 }ELSEIF ($QAServerR -eq "NETVS-QA152A"){
	& c:\Setup.exe /s /v"/lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1  IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616 ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath BULKINSERTCLIENT=$BulkInsertPath " | Out-Null
	Echo_msg "$output_file"  "& c:\Setup.exe /s /v/lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1  IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616 ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath BULKINSERTCLIENT=$BulkInsertPath "
 }
 Else{
	 & c:\Setup.exe /s /v" /lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1 IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616  ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath  BULKINSERTCLIENT=$BulkInsertPath " | Out-Null
	 Echo_msg "$output_file"  "& c:\Setup.exe /s /v /lwme! $output_file /qn IS_SQLSERVER_SERVER=$SQL_SERVER1 IS_SQLSERVER_AUTHENTICATION=0 IS_NET_API_LOGON_USERNAME=$qainstall IS_NET_API_LOGON_PASSWORD=$qainstallpwd  IS_SQLSERVER_DBSYS=$DB_SYSTEM1 IS_SQLSERVER_DBECM=$DB_ECM1 IS_SQLSERVER_DBPRC=$DB_PRICES1 IS_SQLSERVER_DBDWH=$DB_DWH1 EDM_USER=EcmDbReader EDM_PASSW=EcmDbReader ELVIZ_FILE_WATCHING=1 ELVIZ_MESSAGE_QUEUE=0 ACTIVEMQPORT=61616  ELVIZ_WCF_PUBLISHER=1  WCFPORTHTTP=8009 WCFPORTHTTPS=8010 WCFSENDTIMEOUT=10 ELVIZUSER=DealImport ELVIZPASSW=elviz BULKINSERTSERVER=$BulkInsertPath  BULKINSERTCLIENT=$BulkInsertPath "
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
	Echo_msg "$output_file" "[INFO]:Stopping Elviz Message Queue Listener Service on the application server...."
	$sNames =@("Elviz Message Queue Listener Service")
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
$installed=$false


$regkey= "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{F03EAA70-F8A9-4BA2-8287-A7BF71DD5E9C}" 
if(Test-Path $regkey) {
	Echo_msg "$output_file" "[SUCCESS]:Elviz ETRM was installed successfully on the application server"
	$installed=$true
}else{
	Echo_msg "$output_file" "[ERROR]:Elviz ETRM was not installed successfully"
	Send_Error_Message "[ERROR]:ELVIZ WAS NOT INSTALLED ON THE APPLICATION SERVER, THE INSTALLATION PROCESS HAS BEEN STOPPED,"
	exit
}


Echo_msg "$output_file" "[INFO]:Create Elviz Curve Server Services if not exist"
$CurveServer = "Elviz Curve Server Services"
$binaryPath="C:\Elviz\Bin\Viz.MiddleWare.ETRMWinService.exe"
$ECSExist = get-service -display $CurveServer -ErrorAction SilentlyContinue 
if($ECSExist -eq ""){
	 New-Service -name $CurveServer -binaryPathName $binaryPath -displayName $CurveServer -DependsOn "LanmanServer" -Description $CurveServer  -startupType Automatic 
}else{
	Echo_msg "$output_file" "[INFO]:Elviz Curve Server Services is already Exist "
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
IF ($QAServerR -ne "BERPC-PERFA"){
	Start-Service -name "Elviz Message Queue Listener Service" > $null

	Start-Sleep 5
	$Viz_ETRM_Services = get-service -display "Elviz Curve Server Services" -ErrorAction SilentlyContinue 
	$ServiceStatus = $Viz_ETRM_Services.Status
	IF($ServiceStatus -eq "Running"){
	  Echo_msg "$output_file" "	[INFO]:Elviz Message Queue Listener Service status is : $ServiceStatus "
	}ELSE{
		Echo_msg "$output_file" "[INFO]:Elviz Message Queue Listener Service status is : $ServiceStatus "

	}
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
	$Check_install = Get-WmiObject -Class Win32_Product -Filter "name='Elviz ETRM (Server)'" -ComputerName "$QAServerR"
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
	$msg2="
-------------------------------------------------------------------                
				 Installation is complete                   	
				 $end_date                                   
-------------------------------------------------------------------
		" 
  Echo_msg "$output_file" $msg2

}ELSE{
	Echo_msg "$output_file" "[INFO]:DISABLE The QA_DEPLOY scheduled tasks to prevent it to run after a normal reboot" 
	& schtasks /Change /ru $qainstall /rp $qainstallpwd  /tn Run_QADEPLOY /DISABLE
	Echo_msg "$output_file" "[SUCCESS]:Elviz ETRM was installed successfully on the application server $QAServerR"
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
