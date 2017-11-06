$error.clear()
$nl = [Environment]::NewLine
$LogFileSpecified="C:\Elviz\CCNET\LOGS\Nightly_patch.txt"
#delete the log file
if(Test-Path $LogFileSpecified){
    Remove-Item $LogFileSpecified
}

."c:\ElvizPatchScripts\Functions.ps1"
$msiFilesource="\\BERSVDEPLOY\INSTALLATIONS\DEP_TEMP\CCNET\2012.1\CCNET2012.1.msi"
$msiFileapth= "C:\CCNET2012.1.msi"
$ElvizServer="BERVS-APP8CC"
$CCserver="BERVS-APP8CC"
$INSTALLDIR="C:\Elviz\"
$SQL_SERVER="BERSV-SQL8QA"
$SQL_USER="EcmDbUser"
$SQL_PASSW="EcmDbUser"
$DB_CC="QACCNET"
$MSGQPORT="61616"
$PRIMARYIP="194.110.101.27"
$SECONDARYIP="194.110.101.27"
$FAILOVERIP="194.110.101.27"
$SOCKETCONNECTPORT="37126"
$SOCKETUSER="PIXML_VIZ"
$SOCKETPASSW="Nasdaq12"
$SENDERSUBID="XVIZ"
$SENDERCOMPID="NCVIZ"
$TARGETCOMPID="GENIUM_TEST"
$ELVIZRESPQUEUE="dealinsert.response.net.ish"
$now_date=Get-Date

Echo_msg "$LogFileSpecified" "--------------------------------------------------------------- $nl"
Echo_msg "$LogFileSpecified" "- Installing Elviz Contract Clearer package on $CCserver FOR $CODENAME. Date: $now_date"
Echo_msg "$LogFileSpecified" "- Release Number is : $ELVIZRELEASE"
Echo_msg "$LogFileSpecified" "STARTING $CODENAME COPY SCRIPT $now"
Echo_msg "$LogFileSpecified" "--------------------------------------------------------------- $nl"
	& c:\util\pskill.exe "NordpoolAdapter.exe"
	& c:\util\pskill.exe "Elviz.CCFExport.WindowsService.exe"
	& c:\util\pskill.exe "Viz.ContractClearer.Server.WinService.exe"
	Echo_msg "$LogFileSpecified" "[INFO]-Stopping CC Services on the application server...."
	$sNames =@("Elviz CCF Export Service","Brady ETRM RegReporting Service","Elviz Message Queue Listener Service","ElvizContractClearer","Elviz PriceBoard Service","ElvizNasdaqOMXadapter","ActiveMQ")
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
if(test-path "$msiFileapth"){DEL "$msiFileapth"}
$product=Get-WmiObject -Class Win32_Product -Filter "name='Elviz Contract Clearer'" -ComputerName "$CCserver"
IF( $product -eq $null){
	}ELSE{
	Echo_msg "$LogFileSpecified" "[INFO]-removing Elviz Contract Clearer"
	start-sleep 5
	& MsiExec /X  "{9F3505DD-F782-4486-A92E-CAB274ADBC36}" /qn | Out-Null
}
	Echo_msg "$LogFileSpecified" "[INFO]Starting ActiveMQ..."
	Start-Service -name "ActiveMQ" -Verbose  > $null
	Start-Sleep 5
	$Viz_ETRM_Services = get-service -display "ActiveMQ" -ErrorAction SilentlyContinue 
	$ServiceStatus = $Viz_ETRM_Services.Status
	IF($ServiceStatus -eq "Running"){
	    Echo_msg "$LogFileSpecified" "[INFO]ActiveMQ status is : $ServiceStatus "
	}ELSE{
		Echo_msg "$LogFileSpecified" "[INFO]ActiveMQ status is : $ServiceStatus "
		$Error.Add("[ERROR] ActiveMQ status is : $ServiceStatus ")
	}
	Echo_msg "$LogFileSpecified" "[INFO]Starting Elviz Message Queue Listener Service..."
	Start-Service -name "Elviz Message Queue Listener Service" -Verbose > $null
	Start-Sleep 5
	$Viz_ETRM_Services = get-service -Name "Elviz Message Queue Listener Service" -ErrorAction SilentlyContinue 
	$ServiceStatus = $Viz_ETRM_Services.Status
	IF($ServiceStatus -eq "Running"){
	    Echo_msg "$LogFileSpecified" "[INFO]Elviz Message Queue Listener Service status is : $ServiceStatus "
	}ELSE{

	    Echo_msg "$LogFileSpecified" "[ERROR]Elviz Message Queue Listener Service status is : $ServiceStatus "
		$Error.Add("[ERROR] Elviz Message Queue Listener Service status is : $ServiceStatus  ")
	}

Echo_msg "$LogFileSpecified" "[INFO]-Copying msi file from $msiFilesource to $msiFileapth "
Copy-Item  $msiFilesource "$msiFileapth" | Out-Null
Echo_msg "$LogFileSpecified" "[INFO]-Installing Elviz Contract Clearer"
&cmd /c "msiexec /i `"$msiFileapth`" /quiet INSTALLDIR=$INSTALLDIR IS_SQLSERVER_SERVER=$SQL_SERVER IS_SQLSERVER_USERNAME=$SQL_USER IS_SQLSERVER_PASSWORD=$SQL_PASSW IS_SQLSERVER_DATABASE=$DB_CC MSGQUEUESERVER=$ElvizServer MSGQPORT=$MSGQPORT PRIMARYIP=$PRIMARYIP SECONDARYIP=$SECONDARYIP FAILOVERIP=$FAILOVERIP SOCKETCONNECTPORT=$SOCKETCONNECTPORT SOCKETUSER=$SOCKETUSER SOCKETPASSW=$SOCKETPASSW SENDERSUBID=$SENDERSUBID SENDERCOMPID=$SENDERCOMPID TARGETCOMPID=$TARGETCOMPID ELVIZRESPQUEUE=$ELVIZRESPQUEUE WCFPORT=8009" | Out-Null
$product=Get-WmiObject -Class Win32_Product -Filter "name='Elviz Contract Clearer'" -ComputerName "$CCserver"
IF( $product -eq $null){
	Echo_msg "$LogFileSpecified" "[ERROR]: Elviz Contract Clearer cannot be installed, please contact MOhammed to check the installation files" 
	$Error.Add("[ERROR]: Elviz Contract Clearer cannot be installed, please contact MOhammed to check the installation files")
}ELSE{

	

	$ServicesNames =@("ElvizNasdaqOMXadapter","ElvizContractClearer","Elviz PriceBoard Service","Brady ETRM RegReporting Service", "Elviz CCF Export Service")
	FOREACH($ServicesName IN $ServicesNames){
		Echo_msg "$LogFileSpecified" "[INFO]Starting $ServicesName ..."	
		Start-Service -name "$ServicesName" -Verbose > $null
	    Start-Sleep 5
		$Current_Service = get-service -Name "$ServicesName" -ErrorAction SilentlyContinue 
		IF( $Current_Service ){ 
			$ServiceStatus = $Current_Service.Status
			IF($ServiceStatus -eq "Running"){
			Echo_msg "$LogFileSpecified" "[INFO]$ServicesName status is : $ServiceStatus "
			}ELSE{
				 Echo_msg "$LogFileSpecified" "[ERROR]$ServicesName status is : $ServiceStatus "
				$Error.Add("[ERROR]$ServicesName status is : $ServiceStatus")
			}
		}
	}
}
$now_date=Get-Date
$Error_number= $Error.Count
IF ( $Error_number -gt 0){
	$smtpServer = “smtp.songnetworks.no”
	$msg = new-object Net.Mail.MailMessage
	$smtp = new-object Net.Mail.SmtpClient($smtpServer)
	$msg.From = "qainstall@bradyplc.com"
	$msg.To.Add("ElvizDeploy@bradyplc.com") # Change it to deployment 
	#$msg.To.Add("ilya.shavlyuk@bradyplc.com")# Change it to deployment 
	#$msg.To.Add("andrew.konovalov@bradyplc.com")
	#$msg.To.Add("irina.yanovskaya@bradyplc.com")
	#$msg.To.Add("bjarne.johansen@bradyplc.com")
	#$msg.To.Add("kristoffer.stangnes@bradyplc.com")
	$msg.Subject = "ERRORS WHILE INSTALLING CCNET on $CCserver "
	$msg.Body = "- The following errors occur on $CCserver $nl"
	$msg.Body +="- Date:$now_date $nl"
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
	$msg1.To.Add("andrew.konovalov@bradyplc.com")
	$msg1.To.Add("irina.yanovskaya@bradyplc.com")
	$msg1.To.Add("bjarne.johansen@bradyplc.com")
	$msg1.To.Add("kristoffer.stangnes@bradyplc.com")
	$msg1.Subject = "NightlyPatch for CCNET ended on $CCserver  "
	$msg1.Body ="----------------------------------------------------------------------------------- $nl" 
	$msg1.Body += "- Installing the latest CCNET was finished SUCCESSFULLY on $CCserver $nl "
	$msg1.Body +="- Date:$now_date $nl"
	$msg1.Body +="----------------------------------------------------------------------------------- $nl" 
	$smtp.Send($msg1)

}
$Error.Clear()
