 ############################################################################
 #
 #  						QA Client Environment Script        
 #	This ia a nightly patch script to reinstall Elviz ETRM on QA Clients machines
 #
 ############################################################################
cls

."c:\ElvizPatchScripts\Functions.ps1"
$run_date= Get-Date
$Log_Date= Get-Date -format "yyyyMMdd_HHmm"
$guid ="{8DE97840-2D53-4EF4-8BE3-645685E3FDCD}"
$installDir="C:\ElvizClient"
$LicencePath="\\NETVS-DEPLOY.nortest.bradyplc.com\INSTALLATIONS\DEP_TEMP\QALicenceReg\ElvizETRMLicenses.reg" 
$PATCHTOOLFOLDER="C:\util"
$LogFileName="NightlyPatch_ClientLog.txt"
$LogFileSpecified="$installDir\$LogFileName"
if(!(test-path "$installDir")){mkdir $installDir -force}
if(Test-Path "$LogFileSpecified"){ DEL "$LogFileSpecified" -Force}

Echo_msg "$LogFileSpecified" "
------------------------------------------------------------
-                                                          -
-           Nightly Patch For QA Environment               -
-           Copy Rights Viz Risk Management                -
-           Installing Elviz ETRM (client)                 -
-                                                          -
------------------------------------------------------------"
# ------- Script variables ---------------
$setup="ElvizETRM(Client).exe"

$App_Server="$Env:ElvizApplServer"
$Client="$Env:ElvizApplClient"
Echo_msg "$LogFileSpecified" "[INFO] Application server: $App_Server"
Echo_msg "$LogFileSpecified" "[INFO] Client Machine: $Client"
#$App_Servers=@("NETVS-QA153A","NETVS-QA153A01","NETVS-QA153A02","NETVS-QA152A","NETVS-QA152A01","NETVS-QA152A02","NETVS-QA142A01","NETVS-QA142A02","NETVS-QA151A","NETVS-QA151A01","NETVS-QA151A02","BERVS-QA143A","BERVS-QA143A01","BERVS-QA143A02","BERVS-QA142A","BERVS-QA142A02","BERVS-QA141A","BERVS-QA141A01","BERVS-QA141A02","BERVS-QA133A","BERVS-QA133A01","BERVS-QA133A02","BERVS-QA132A02","BERVS-QA132A","BERVS-QA122A","BERVS-QA131A","BERVS-QA131A01","BERVS-QA122A02","BERPC-PERFA","BERVS-QA112A")
#$Clients=@("NETVS-QA153C","NETVM-QA153C01","NETVM-QA153C02","NETVS-QA152C","NETVM-QA152C01","NETVM-QA152C02","NETVM-QA142C01","NETVM-QA142C02","NETVS-QA151C","NETVM-QA151C01","NETVM-QA151C02","BERVS-QA143C","BERVM-QA143C01","BERVM-QA143C02","BERVS-QA142C","BERVM-QA142C02","BERVS-QA141C","BERVM-QA141C01","BERVM-QA141C02","BERVS-QA133C","BERVM-QA133C01","BERVM-QA133C02","BERVM-QA132C02","BERVS-QA132C","BERVS-QA122C","BERVS-QA131C","BERVM-QA131C01","BERVM-QA122C02","BERPC-PERFC","BERVS-QA112C")
#$c=0
#$exist=$false
#WHILE($c -lt $App_Servers.Length){
#	IF($App_Servers[$c] -ne $null){
#		IF($Server_Inv_var -eq $App_Servers[$c]){
#			$exist=$true
#			$App_Server=$App_Servers[$c]
#			$Client=$Clients[$c]
#			Echo_msg "$LogFileSpecified" "[INFO] Application server: $App_Server"
#			Echo_msg "$LogFileSpecified" "[INFO] Client Machine: $Client"
#			Break
#		}ELSE{
#			$c++
#		}
#	}
#} 

$InstallProxy=$Client+ "_InstallProxy.txt"
$SetupPath="\\$App_Server\Elviz\ClientInstall\$setup"
if(!(Test-Path "$SetupPath")){
	$setup="BradyETRM(Client).exe"
	$SetupPath="\\$App_Server\Elviz\ClientInstall\$setup"
}
Echo_msg "$LogFileSpecified" "[INFO] Check the connection to the application server"
# test connection the the machines
$AppServerExist = Ping-Address ($App_Server)

IF ( $AppServerExist ){
	Echo_msg "$LogFileSpecified" "[SUCCESS] Connected to the application server $App_Server" 
}ELSE{ 
	Echo_msg "$LogFileSpecified" "[ERROR] Cannot connect to the application server $App_Server"
	Client_Error_Message "[ERROR] COULD NOT CONNECT TO THE APPLICATION SERVER $App_Server"
	exit
}





IF(Test-Path $SetupPath){
	
}ELSE{
	Echo_msg "$LogFileSpecified" "[ERROR] $SetupPath this path does not exist, check the network connection"
	exit
}
Echo_msg "$LogFileSpecified" "[INFO] Kill all Elviz applications on the Client machine"
$processes_ISBEW=@("ISBEW64.exe","ISBEW64.exe","ISBEW64.exe","ISBEW64.exe","msiexec.exe","msiexec.exe","msiexec.exe")
foreach($a in $processes_ISBEW){
	$process = get-wmiobject win32_process | where {$_.Name -like "$a"}
	if($process -ne $null){
		$process.Terminate() > $null 
		Echo_msg "$LogFileSpecified" "[SUCCESS] $a was terminated on $Client "
	}ELSE{

		Echo_msg "$LogFileSpecified" "[INFO] $a was not running on $Client"
	}
}
$processes_list=@("ISBEW64.exe","notepad.exe","TestComplete.exe","TestExecute.exe","ElvizETRMClient.exe","ElvizDM.exe","ElvizRM.exe","ElvizFM.exe","ElvizCM.exe","ElvizTM.exe","ElvizLM.exe")
foreach($a in $processes_list){
	$process = get-wmiobject win32_process | where {$_.Name -like "$a"}
	if($process -ne $null){
		$process.Terminate() > $null 
		Echo_msg "$LogFileSpecified" "[SUCCESS] $a was terminated on $Client "
	}ELSE{

		Echo_msg "$LogFileSpecified" "[INFO] $a was not running on $Client"
	}
}
 Echo_msg "$LogFileSpecified" "[INFO] Searching for previous Elviz client installation..."
do {
	if (test-path "$PATCHTOOLFOLDER\GetUninstallGUID.exe"){
		$APP_GUID=	& $PATCHTOOLFOLDER\GetUninstallGUID.exe "elviz ETRM (Application proxy)"
		Echo_msg "$LogFileSpecified" "[INFO] Application proxy GUID $APP_GUID"
		if ($APP_GUID -ne $null){
		Echo_msg "$LogFileSpecified" "[INFO] Removing elviz ETRM (Application proxy)"
			$MsiExitCode= (Start-Process -FilePath "msiexec.exe" -ArgumentList "/X$APP_GUID /qn" -Wait -Passthru).ExitCode
			IF ($MsiExitCode -eq 0){
				Echo_msg "$LogFileSpecified" "[SUCCESS] Removed elviz ETRM (Application proxy)"
			}ElSEIF($MsiExitCode -eq 1605){
				Echo_msg "$LogFileSpecified" "[INFO] elviz ETRM (Application proxy) is not installed on this machine"
			}ELSE{
				Echo_msg "$LogFileSpecified" "[ERROR] This Command msiexec.exe /X$APP_GUID /qn"
				Echo_msg "$LogFileSpecified" "[ERROR] Exited with ExitCode $MsiExitCode"
				Echo_msg "$LogFileSpecified" "[INFO] Please try to remove elviz ETRM (Application proxy) then run the script again"
				#Start-Sleep -Seconds 10
			}
					
		}ELSE{
			Echo_msg "$LogFileSpecified" "[INFO] elviz ETRM (Application proxy) is not installed on this machine" 
		}
	}ELSE {
		Echo_msg "$LogFileSpecified" "[ERROR] $PATCHTOOLFOLDER\GetUninstallGUID.exe does not exist"
		Echo_msg "$LogFileSpecified""[ERROR] You cannot processs the process while this file is missing"
		#Start-Sleep -Seconds 10
	}
$i++
}
while ($i -le 3)
 
 Echo_msg "$LogFileSpecified" "[INFO] Uninstallation of Brady ETRM on $Client "
<# $product=Get-WmiObject -Class Win32_Product -Filter "name='ElvizETRM(Client)'" -ComputerName "$Client"
IF( $product -eq $null){
# if elviz not installed but still have junk files to delete
	
}ELSE{
	Echo_msg "$LogFileSpecified" "[INFO] Uninstalling Elviz client........ (Min 2 minutes) "

	$comAdmin = New-Object -comobject COMAdmin.COMAdminCatalog
	$apps = $comAdmin.GetCollection("Applications")
	$apps.populate()
	$appIndex = 0
	foreach ($a in $apps)
	{		
		if ($a.Name -eq "Elviz ETRM")
		{
			Echo_msg "$LogFileSpecified" "[INFO] Removing ETRM COM+ application"
			$apps.remove($appIndex)
			$apps.savechanges()
			break
		}
		$appIndex++
	}
	Echo_msg "$LogFileName" "************** Uninstallation errors log start  here ************** "
	& msiexec /qn /lwme+ "$installDir\$LogFileName /Uninstall $guid /norestart | Out-Null
	#Start-Sleep -Seconds 120
	Echo_msg "$LogFileName" "************** End of Uninstallation errors log  ************** "
	#$Check_uninstall = Get-WmiObject -Class Win32_Product -Filter "name='ElvizETRM(Client)'" -ComputerName "$Client"
	$Check_uninstall =Get-WmiObject -query 'select * from win32_product' | where {$_.name -like "ElvizETRM(Client)"} | foreach {$_}
	Echo_msg "$LogFileSpecified" "check uninstall $Check_uninstall "
	IF( $Check_uninstall -eq $null){
		Echo_msg "$LogFileSpecified" "[SUCCESS] ElvizETRM(Client) was removed Form Add\Remove programs"
	}Else{
		Echo_msg "$LogFileSpecified" "[ERROR]Elviz ETRM was not removed successfully on $Client"
		Client_Error_Message "[ERROR] Elviz ETRM (client) was not removed successfully on $Client"
	exit
	}

}
#>
$regkey64= "HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\$guid"
$regkey32= "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$guid"
if((Test-Path $regkey64) -or (Test-Path $regkey32) ) {
	Echo_msg "$LogFileSpecified" "[INFO] Uninstalling Brady client........ (Min 2 minutes) "

	$comAdmin = New-Object -comobject COMAdmin.COMAdminCatalog
	$apps = $comAdmin.GetCollection("Applications")
	$apps.populate()
	$appIndex = 0
	foreach ($a in $apps)
	{		
		if ($a.Name -eq "Elviz ETRM")
		{
			Echo_msg "$LogFileSpecified" "[INFO] Removing ETRM COM+ application"
			$apps.remove($appIndex)
			$apps.savechanges()
			break
		}
		$appIndex++
	}
	Echo_msg "$LogFileSpecified" "************** Uninstallation errors log start  here ************** "
	& msiexec /qn /lwme+ "$installDir\$LogFileName" /Uninstall $guid /norestart | Out-Null
	Echo_msg "$LogFileSpecified" "************** End of Uninstallation errors log  ************** "
	if(!(Test-Path $regkey64) -or !(Test-Path $regkey32) ) {
		Echo_msg "$LogFileSpecified" "[SUCCESS] BradyETRM(Client) was removed Form Add\Remove programs"
	}Else{
		Echo_msg "$LogFileSpecified" "[ERROR]Brady ETRM was not removed successfully on $Client"
		Client_Error_Message "[ERROR] Brady ETRM (client) was not removed successfully on $Client"
	exit
	}
}




Echo_msg "$LogFileSpecified" "[INFO] Installing Brady ETRM Client on $Client "
# run the setup file
Echo_msg "$LogFileSpecified" "************** Installation errors log start  here ************** "
& "\\$App_Server\Elviz\ClientInstall\$setup" /s /v"/lwme+ $installDir\$LogFileName /qn APP_SERVER=\\$App_Server\Elviz APP_SERVER_NAME=$App_Server INSTALLDIR=$installDir " | Out-Null
#&cmd /c "msiexec /i `"\\$App_Server\Elviz\ClientInstall\$setup`" /l* `"$App_Server.msi.log`" /quiet APP_SERVER=\\$App_Server\Elviz APP_SERVER_NAME=$App_Server INSTALLDIR=`"$installDir`"" | Out-Null
Echo_msg "$LogFileSpecified" "************** End of Installation errors log  ************** "

# Start-Sleep -Seconds 180
#$Check_install = Get-WmiObject -Class Win32_Product -Filter "name='ElvizETRM(Client)'" -ComputerName "$Client"
# Start-Sleep -Seconds 10
if(!(Test-Path $regkey64) -and !(Test-Path $regkey32) ) {
	Echo_msg "$LogFileSpecified" "[ERROR] Elviz ETRM was not installed successfully"

	Echo_msg "$LogFileSpecified" "CMD: \\$App_Server\Elviz\ClientInstall\$setup /s /v/lwme+ $installDir\$LogFileName /qn APP_SERVER=\\$App_Server\Elviz APP_SERVER_NAME=$App_Server INSTALLDIR=$installDir " "YELLOW"
	Client_Error_Message "[ERROR] Elviz ETRM (client) was not installed successfully on $Client"
	exit	
}ELSE{
	Echo_msg "$LogFileSpecified" "[SUCCESS] ElvizETRM(client) was installed successfully"
	Echo_msg "$LogFileSpecified" "[INFO] Installing elviz ETRM (Application proxy)"
	IF(Test-Path "\\$App_Server\Elviz\ClientInstall\$App_Server.msi" ){
		& msiexec ALLUSERS=ALLUSERS ADDLOCAL=ALL REMOTESERVERNAME=$App_Server /i "\\$App_Server\Elviz\ClientInstall\$App_Server.msi" /norestart | out-null
		#Start-Sleep -Seconds 30
		if (test-path "$PATCHTOOLFOLDER\GetUninstallGUID.exe"){
			$APP_GUID=	& $PATCHTOOLFOLDER\GetUninstallGUID.exe "elviz ETRM (Application proxy)"
			Echo_msg "$LogFileSpecified" "[INFO] Application proxy GUID $APP_GUID"
			if ($APP_GUID -ne $null){
				 Echo_msg "$LogFileSpecified" "[SUCCESS] Installed elviz ETRM (Application proxy)"
			}else{
				Client_Error_Message "[ERROR]Elviz ETRM(application porxy) has not been install successfullly on $Client"
			}
		}
	}
}
IF(Test-Path $LicencePath ){
	Echo_msg "$LogFileSpecified"  "[INFO]:Copying the License Reg to Elviz ETRM folder"
	IF(Test-Path "$installDir\elviz ETRM\"){
		Copy-Item  $LicencePath "$installDir\elviz ETRM\"
	}
	IF(Test-Path "$installDir\Brady ETRM\"){
		Copy-Item  $LicencePath "$installDir\Brady ETRM\"
	}
}ELSE{
	Echo_msg "$LogFileSpecified"  "[ERROR]:Elviz ETRM License was not copied, Check it is path $LicencePath on $Client"
	Client_Error_Message "[ERROR]:Elviz ETRM License was not copied, Check it is path $LicencePath on $Client"
}

if((Test-Path $regkey64) -or (Test-Path $regkey32) ) {
	Echo_msg "$LogFileSpecified" "[SUCCESS]:Elviz ETRM was installed successfully on the application server $Client"
	$end_date= Get-Date
	$msg2="
-------------------------------------------------------------------                
				 Installation is complete                   	
				 $end_date                                   
-------------------------------------------------------------------
	" 
	Echo_msg "$LogFileSpecified" $msg2
	Copy-Item "$installDir\NightlyPatch_ClientLog.txt" "\\$App_Server\Elviz\" -Force 
	Client_End_Message
}