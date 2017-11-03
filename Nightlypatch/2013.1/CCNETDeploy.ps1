$error.clear()
$nl = [Environment]::NewLine
$version="2013.1"
$CCserver="BERVS-APP8CC01"
$LogFileSpecified="C:\Elviz\CCNET\LOGS\Nightly_patch.txt"
Del $LogFileSpecified -Force
."c:\ElvizPatchScripts\Functions.ps1"
$LatestBinaries="\\Bersvdeploy\installations\DEP_TEMP\CCNET\2013.1\LatestBinaries"
$localpath= "C:\Elviz\CCNET\"
$run_date= Get-Date
$Log_Date= Get-Date -format "yyyyMMdd_HHmm"
Echo_msg "$LogFileSpecified" "--------------------------------------------------------------- $nl"
Echo_msg "$LogFileSpecified" "- Installing Elviz Contract Clearer package on $CCserver. Date: $run_date"
Echo_msg "$LogFileSpecified" "- $msiFilesource "
Echo_msg "$LogFileSpecified" "--------------------------------------------------------------- $nl"
	& c:\util\pskill.exe "NordpoolAdapter.exe"
	Echo_msg "$LogFileSpecified" "[INFO]-Stopping CC Services on the application server...."
	$sNames =@("ElvizContractClearer","ElvizNasdaqOMXadapter","ElvizTrayportadapter")
	FOREACH($sName IN $sNames){
	    $service = get-service -Name $sName -ErrorAction SilentlyContinue 
	    IF( ! $service ){ 
	        $msg1= "[INFO]-" + $sName + " was not installed on this computer." 
	        Echo_msg "$LogFileSpecified" $msg1
	        } 
	    ELSE{ 
	        Stop-Service -Name $sName -Force
	        Start-Sleep -Seconds 10
			$service = get-service -Name $sName -ErrorAction SilentlyContinue 
	        $msg1 = "[INFO]-" + $sName + "'s status is: " + $service.Status 
	        Echo_msg "$LogFileSpecified" $msg1
	    }
	}

Echo_msg "$LogFileSpecified" "[INFO]-Reinstalling Elviz Contract Clearer"
del "C:\Elviz\CCNET\*.dll" -Force
del "C:\Elviz\CCNET\*.xml" -Force
del "C:\Elviz\CCNET\*.config" -Force
del "C:\Elviz\CCNET\*.exe" -Force
del "C:\Elviz\CCNET\*.txt" -Force
Echo_msg "$LogFileSpecified" "[INFO]-Copying latest binaries  from $LatestBinaries"
Copy-Item  "$LatestBinaries\*" "C:\Elviz\CCNET\" -recurse | Out-Null
	Echo_msg "$LogFileSpecified" "[INFO]Starting ElvizContractClearer..."	
	Start-Service -name "ElvizContractClearer" -Verbose > $null
	Start-Sleep 5
	$CCService = get-service -Name "ElvizContractClearer" -ErrorAction SilentlyContinue 
	$ServiceStatus = $CCService.Status
	IF($ServiceStatus -eq "Running"){
	    Echo_msg "$LogFileSpecified" "[INFO]ElvizContractClearer status is : $ServiceStatus "
	}ELSE{

	    Echo_msg "$LogFileSpecified" "[ERROR]ElvizContractClearer status is : $ServiceStatus "
		$Error.Add("[ERROR]ElvizContractClearer status is : $ServiceStatus")
	}
	Echo_msg "$LogFileSpecified" "[INFO]Starting ElvizTrayportadapter..."	
	Start-Service -name "ElvizTrayportadapter" -Verbose > $null
	Start-Sleep 5
	$TrayPort = get-service -Name "ElvizTrayportadapter" -ErrorAction SilentlyContinue 
	$ServiceStatus = $TrayPort.Status
	IF($ServiceStatus -eq "Running"){
	    Echo_msg "$LogFileSpecified" "[INFO]Elviz PriceBoard Service status is : $ServiceStatus "
	}ELSE{

	    Echo_msg "$LogFileSpecified" "[ERROR]Elviz PriceBoard Service status is : $ServiceStatus "
		$Error.Add("[ERROR]Elviz PriceBoard Service status is : $ServiceStatus")
	}			

	$end_date= Get-Date
$Error_number= $Error.Count
IF ( $Error_number -gt 0){
	$smtpServer = “smtp.songnetworks.no”
	$msg = new-object Net.Mail.MailMessage
	$smtp = new-object Net.Mail.SmtpClient($smtpServer)
	$msg.From = "qainstall@bradyplc.com"
	$msg.To.Add("ElvizDeploy@bradyplc.com") # Change it to deployment 
	$msg.To.Add("ilya.shavlyuk@bradyplc.com")# Change it to deployment 
	$msg.To.Add("andrew.konovalov@bradyplc.com")
	$msg.To.Add("irina.yanovskaya@bradyplc.com")
	$msg.To.Add("bjarne.johansen@bradyplc.com")
	$msg.To.Add("kristoffer.stangnes@bradyplc.com")
	$msg.Subject = "ERRORS WHILE INSTALLING CCNET $version on $CCserver "
	$msg.Body = "- The following errors occur on $CCserver $nl"
	$msg.Body +="- Date:$end_date $nl"
	$msg.Body +="----------------------------------------------------------------------------------- $nl" 
	Echo_msg "$LogFileSpecified" " --List of ERRORS " 
	Echo_msg "$LogFileSpecified" "--------------------------------------" 
	$msg.Body +=" --List of ERRORS ($Error_number Errors) $nl  " 
	Foreach($gh in $Error){
		IF ($gh -ne "0"){
			Echo_msg "$LogFileSpecified" "$gh  $nl " 
			$msg.Body += "- $gh  $nl"
			$msg.Body += "$nl"
		}
	}
	Echo_msg "$LogFileSpecified" "--------------------------------------" 
	Echo_msg "$LogFileSpecified" "Copy script finished with $Error_number ERRORS" 
	Echo_msg "$LogFileSpecified" "--------------------------------------" 
	#smtp mailer
	$msg.Body += "
	---------------------------------------------------------------------------------------------
	- The package was not installed
	- Login to $CCserver nad check the source of the issues.
	----------------------------------------------------------------------------------------------"
	$smtp.Send($msg)
	Copy-Item -re
	Start-Sleep 30
}ELSE{ # if there is no errors in the copy script 
	Echo_msg "$LogFileSpecified" "--------------------------------------$nl" 
	Echo_msg "$LogFileSpecified" "  Installing the latest CCNET was finished SUCCESSFULLY on $CCserver $nl  "
	Echo_msg "$LogFileSpecified" "--------------------------------------$nl"
	$smtpServer = “smtp.songnetworks.no”
	$msg1 = new-object Net.Mail.MailMessage
	$smtp = new-object Net.Mail.SmtpClient($smtpServer)
	$msg1.From = "qainstall@bradyplc.com"
	$msg1.To.Add("ElvizDeploy@bradyplc.com")
	$msg1.To.Add("ilya.shavlyuk@bradyplc.com")# Change it to deployment 
	$msg1.To.Add("andrew.konovalov@bradyplc.com")
		$msg1.To.Add("irina.yanovskaya@bradyplc.com")
	$msg1.To.Add("bjarne.johansen@bradyplc.com")
	$msg1.To.Add("kristoffer.stangnes@bradyplc.com")
	$msg1.Subject = "NightlyPatch for CCNET $version ended on $CCserver  "
	$msg1.Body ="----------------------------------------------------------------------------------- $nl" 
	$msg1.Body += "- Installing the latest CCNET was finished SUCCESSFULLY on $CCserver $nl "
	$msg1.Body +="- Date:$end_date $nl"
	$msg1.Body +="----------------------------------------------------------------------------------- $nl" 
	$smtp.Send($msg1)

}
$Error.Clear()