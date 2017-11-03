
# Functions to use
$setup="setup.exe" 
$qainstall="bradyplc\qainstall"
$qainstallpwd="Isetitup2"
$IS_NET_API_LOGON_USERNAME1="$qainstall"
$IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
$DEployServer="BERSVDEPLOY"
$Deleting_folders=@("Documentation")
$Deleting_files=@("Bin\*.tlb")
$PatchToolFolder="C:\util\PatchTools"
$ConfigPathset="\\$DEployServer\INSTALLATIONS\DEP_TEMP\QAConfig\Viz.Priceboard.Server.dll.config"
$LicencePath="\\$DEployServer\INSTALLATIONS\DEP_TEMP\QALicenceReg\ElvizETRMLicenses.reg" 
$delete_file_list=@(".tlb",".dll")
$run_date= Get-Date
$Log_Date= Get-Date -format "yyyyMMdd_HHmm"
$output_file = "c:\Elviz\NightlyPatch_log.txt"
set-alias psexec "$PatchToolFolder\psexec.exe"

$date =Get-Date
$server="$Env:ElvizapplServer" 
$client= "$Env:ElvizApplClient" 
$Reg="False"
$QARecievers=@("proddev@bradyplc.com","daniel.watanabe@bradyplc.com","irina.yanovskaya@bradyplc.com","bjarne.johansen@bradyplc.com","grzegorz.jedynak@bradyplc.com","iryna.nikolaienko@bradyplc.com")
$QARegRecievers=@("elvizdeploy@bradyplc.com" ,"daniel.watanabe@bradyplc.com","iryna.nikolaienko@bradyplc.com","bjarne.johansen@bradyplc.com")
FUNCTION Send_Mail {
	param($From,$To,$Subject,$body)
	#$smtpServer = “smtp.songnetworks.no”
	$smtpServer = “Camvs-re8-2”
	
	$msg = new-object Net.Mail.MailMessage
	$smtp = new-object Net.Mail.SmtpClient($smtpServer)
	$msg.From = "$From"
	$msg.To.Add("$To")
	$msg.Subject = "$Subject"
	$msg.Body = "$body"
	$smtp.Send($msg)
	
}
$FromMail="qainstall@bradyplc.com"


IF($server -eq "BERVS-QA141A"){
	$client= "BERVS-QA141C" 
	$Reg="False"
}ELSEIF($server -eq "BERVS-QA141A01"){
	$client="BERVM-QA141C01"
	$Reg="True"
}ELSEIF($server -eq "BERVS-QA141A02"){
	$client="BERVM-QA141C02"
	$Reg="True"
}


IF($server -eq "BERVS-QA133A"){
	$client= "BERVS-QA133C" 
	$Reg="False"
}ELSEIF($server -eq "BERVS-QA133A01"){
	$client="BERVM-QA133C01"
	$Reg="True"
}ELSEIF($server -eq "BERVS-QA133A02"){
	$client="BERVM-QA133C02"
	$Reg="True"
}

IF($server -eq "BERVS-QA132A"){
	$client= "BERVS-QA132C" 
	$Reg="False"
}ELSEIF($server -eq "BERVS-QA132A01"){
	$client="BERVM-QA132C01"
	$Reg="True"
}ELSEIF($server -eq "BERVS-QA132A02"){
	$client="BERVM-QA132C02"
	$Reg="True"
}


IF($server -eq "BERVS-QA131A"){
	$client= "BERVM-QA131C" 
	$Reg="False"
}ELSEIF($server -eq "BERVS-QA131A01"){
	$client="QA131CR01"
	$Reg="True"
}ELSEIF($server -eq "QA131AR02"){
	$client="QA131CR02"
	$Reg="True"
}ELSEIF($server -eq"DEVPERFAPP"){
	$client=""
	$Reg="False"
}
IF($server -eq "BERVS-QA122A"){
	$client= "BERVS-QA122C"
	$Reg="False"
}ELSEIF($server -eq "BER-VS-QA122C01"){
	$client=""
	$Reg="True"
}ELSEIF($server -eq "BERVS-QA122A02"){
	$client="BERVM-QA122C02"
	$Reg="True"
}
IF($server -eq"BERVS-APP8CC"){
	$client=""
	$Reg="False"
}

IF($server -eq "QA112A"){
	$client= "QA112C1"
	$Reg="False"
}
FUNCTION Ping-Address {
# to ping a machine
param([string]$ExistComputer);
	PROCESS {
	$ping = $false
	  $results = Get-WmiObject -query `
	  "SELECT * FROM Win32_PingStatus WHERE Address = '$ExistComputer'"
	  foreach ($result in $results) {
		if ($results.StatusCode -eq 0) {
		  $ping = $true
		  
		}
	  }
	  if ($ping -eq $true) {
		Write-Output $ping
	  }
	}
 }

 # Function to restart a computer 
FUNCTION Restart-Computer {
param([string]$RestarComputer);
	PROCESS {
	  $computer = Get-WmiObject Win32_OperatingSystem -computer $RestarComputer
	 $computer.Reboot()
	}
}

function isNumeric ($x) {
	try {
		0 + $x | Out-Null
		return $true
	} catch {
		return $false
	}
}

# test reg key if exist
Function Test-RegistryValue($regkey, $name) {
	$exists = Get-ItemProperty -Path "$regkey" -Name "$name" -ErrorAction SilentlyContinue
	If (($exists -ne $null) -and ($exists.Length -ne 0)) {
		Return $true
	}
	Return $false
}
# Gets the specified registry value or $null if it is missing
function Get-RegistryValue($path, $name)
{
	$key = Get-Item -LiteralPath $path -ErrorAction SilentlyContinue
	if ($key) {
		$key.GetValue($name, $null)
	}
}
#logoff all users in a machine
Function Logoff{
param(
[String]$ComputerName# used if logoff should be forced
)
#if windows is XP
$colItems= Get-WmiObject Win32_OperatingSystem  -Namespace root\CIMV2 -ComputerName $ComputerName
	Foreach($objItem in $colItems){
		$version= $objItem.Version
		if($version.Contains("5.1")){
			#logoff xp machine
			(Get-WmiObject -Class Win32_OperatingSystem -ComputerName $ComputerName).InvokeMethod("Win32Shutdown",0)
		}Else{
			#if not windows XP
			$list=@();
			$q = (qwinsta /SERVER:$ComputerName )
			foreach ($s in $q){
				if($s.Contains("Active") -or $s.Contains("Disc") ){
					$r= (($s.Trim() -replace "\s+",",") )
					$list+=$r.Split(",")
				}	
			}
			Foreach ($t in $list){
				if(isNumeric($t)){
					([WMICLASS]"win32_process").Create("cmd.exe /C  logoff $t /server:$ComputerName ")	
				}
			}
				
		}
	}
}

Function Send_Not_Patching_Message {
		$Subject= "WARNING: NightlyPatch DID NOT run on $server (Fail on TFS Last Build)"
		$body= "
----------------------------------------------------------------------------------------------
- Date:$date
- Application server: $server
- Client machine(s):     $client 
- WARNING: Nightly patch did not run on $server because of a failed in the last build server.
----------------------------------------------------------------------------------------------"
		if($Reg -eq "True"){
			FOREACH ($QARegReciever in $QARegRecievers){
				Send_mail "$FromMail" "$QARegReciever" "$Subject" "$body"

			}

		}else{
			
			FOREACH ($QAReciever in $QARecievers){
				Send_mail "$FromMail" "$QAReciever" "$Subject" "$body"

			}

		}
	
}

# Send mail to qa 
Function Send_Warning_Message {
		$Subject= "WARNING:NightlyPatch will reboot $server in 2 Minutes "
		$body= "
----------------------------------------------------------------------------------------------
- Date:$date
- Application server: $server
- Client machine(s):     $client
- WARNING: Please Close all Elviz applications and logoff these machines
----------------------------------------------------------------------------------------------"
		if($Reg -eq "True"){
			FOREACH ($QARegReciever in $QARegRecievers){
				Send_mail "$FromMail" "$QARegReciever" "$Subject" "$body"

			}
		}else{
			
			FOREACH ($QAReciever in $QARecievers){
				Send_mail "$FromMail" "$QAReciever" "$Subject" "$body"

			}

		}
}

# Send mail to qa 
Function Send_Start_Message {
		$Subject= "NightlyPatch is starting on $server "
		$body= "
----------------------------------------------------------------------------------------------
- Date:$date
- Application server: $server
- Client machine(s):     $client
----------------------------------------------------------------------------------------------"
		if($Reg -eq "True"){
			FOREACH ($QARegReciever in $QARegRecievers){
				Send_mail "$FromMail" "$QARegReciever" "$Subject" "$body"

			}
		}else{
			
			FOREACH ($QAReciever in $QARecievers){
				Send_mail "$FromMail" "$QAReciever" "$Subject" "$body"

			}

		}

}





Function Send_Error_Message {
		param([string]$Errmsg);
		$Subject= "[ERROR] NightlyPatch Installation error on $server "
		$body= "
----------------------------------------------------------------------------------------------
- Date:$date
- Nightly Patch has finished patching ($server)
- Application server: $server
- Client machine(s):     $client
- $Errmsg, Please check the log file
- Log File: \\$server\Elviz\NightlyPatch_log.txt
----------------------------------------------------------------------------------------------"
		if($Reg -eq "True"){
			FOREACH ($QARegReciever in $QARegRecievers){
				Send_mail "NightlyPatch@bradyplc.com" "$QARegReciever" "$Subject" "$body"

			}
		}else{
			
			FOREACH ($QAReciever in $QARecievers){
				Send_mail "$FromMail" "$QAReciever" "$Subject" "$body"

			}

		}
}



FUNCTION Send_End_Message {

		$Subject= "NightlyPatch ended on $server "
		$body= "
----------------------------------------------------------------------------------------------
- Date:$date
- Nightly Patch has finished patching ($server)
- Application server: $server
- Client machine:     $client
- Log File: \\$server\Elviz\NightlyPatch_log.txt
- Please check the log file before testing Elviz 
				----------------------------------------------------------------------------------------------"
		if($Reg -eq "True"){
			FOREACH ($QARegReciever in $QARegRecievers){
				Send_mail "$FromMail" "$QARegReciever" "$Subject" "$body"

			}
		}else{
			
			FOREACH ($QAReciever in $QARecievers){
				Send_mail "$FromMail" "$QAReciever" "$Subject" "$body"

			}

		}
}
Function Client_Error_Message {
		param([string]$Errmsg);
		$Subject= "[ERROR] NightlyPatch Installation error on $client "
		$body= "
----------------------------------------------------------------------------------------------
- Date:$date
- Nightly Patch has finished patching ($server and $client)
- Application server: $server
- Client machine(s):     $client
- $Errmsg, Please check the log file
- Log File: \\$server\Elviz\NightlyPatch_ClientLog.txt
----------------------------------------------------------------------------------------------"
		if($Reg -eq "True"){
			FOREACH ($QARegReciever in $QARegRecievers){
				Send_mail "$FromMail" "$QARegReciever" "$Subject" "$body"

			}
		}else{
			
			FOREACH ($QAReciever in $QARecievers){
				Send_mail "$FromMail" "$QAReciever" "$Subject" "$body"

			}

		}
}
FUNCTION Client_End_Message {

		$Subject= "NightlyPatch ended on $client "
		$body= "
----------------------------------------------------------------------------------------------
- Date:$date
- Nightly Patch has finished patching ($server and $client)
- Application server: $server
- Client machine:     $client
- Log File: \\$server\Elviz\NightlyPatch_ClientLog.txt
- Please check the log file before testing Elviz 
				----------------------------------------------------------------------------------------------"
		if($Reg -eq "True"){
			FOREACH ($QARegReciever in $QARegRecievers){
				Send_mail "$FromMail" "$QARegReciever" "$Subject" "$body"

			}
		}else{
			
			FOREACH ($QAReciever in $QARecievers){
				Send_mail "$FromMail" "$QAReciever" "$Subject" "$body"

			}

		}
}


Function Echo_msg {
	Param (
	 [string]$filePath,
	 [string]$msg
	)
   Add-content $filePath -value $msg -Force -Encoding Unicode
   echo $msg
}

FUNCTION Remove-ScriptVariables($path) {
	$result = Get-Content $path |
	ForEach { 
		if ( $_ -match '(\$.*?)\s*=') {
		 $matches[1]  | ? { $_ -notlike '*.*' -and $_ -notmatch 'result' -and $_ -notmatch 'env:'}
		}
	}
	FOREACH ($v IN ($result | Sort-Object | Get-Unique)){
		 Write-Host "Removing" $v.replace("$","")
		 Remove-Variable ($v.replace("$","")) -ErrorAction SilentlyContinue
	  }
}
