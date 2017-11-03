 ############################################################################
 #
 #  						SAO Environment Script        
 #	This ia a nightly patch script to reinstall Web Sales Manager on QA SAO machines
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
$Servers=@("BERVS-QA122A")
$WSMPath="\\$DEployServer\INSTALLATIONS\DEP_TEMP\WSM\2012.1\WSM2012.1.msi"
$WSMLocalpath="C:\WSM2012.1.msi"
$run_date= Get-Date
$Log_Date= Get-Date -format "yyyyMMdd_HHmm"
$output_file = "c:\Elviz\WSM\NightlyPatch_log.txt"
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
$c=0
$exist=$false
WHILE($c -lt $Servers.Length){
    if($Servers[$c] -ne $null){
        IF($Servers[$c] -eq "BERVS-QA122A"){ 
				$IS_NET_API_LOGON_USERNAME1="$qainstall"
                $IS_NET_API_LOGON_PASSWORD1="$qainstallpwd"
                $SQL_SERVER1="BERSV-SQL8QA"
                $SQL_USER1="EcmDbUser"
                $SQL_PASSW1="EcmDbUser"
                $DB_WSM="QAWSM122"
               $PORT_NUMBER="8009" 
			   $BUYATC="1" 
			   $AUTOREFRESH="1" 
			   $BUYEC="0" 
			   $USEFAILEDCURVES="1" 
			   $USENONSYNCEDCURVES="1"
                #------- Machines names -----------
                $QAServerR=$Servers[$c]
                # ***** to UNC path 
                $QAServer="\\$QAServerR"
                $exist=$true
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
&cmd /c "msiexec /i `"$WSMLocalpath`" /quiet INSTALLDIR=C:\Elviz APP_SERVER=$QAServerR IS_SQLSERVER_SERVER=$SQL_SERVER1 IS_SQLSERVER_USERNAME=$SQL_USER1 IS_SQLSERVER_PASSWORD=$SQL_PASSW1 IS_SQLSERVER_DATABASE=$DB_WSM PORT_NUMBER=$PORT_NUMBER BUYATC=$BUYATC AUTOREFRESH=$BUYATC BUYEC=$BUYEC USEFAILEDCURVES=$USEFAILEDCURVES USENONSYNCEDCURVES=$USENONSYNCEDCURVES" | Out-Null

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
$product=Get-WmiObject -Class Win32_Product -Filter "name='Elviz Clearing Reports Viewer'" -ComputerName "$QAServerR"
IF( $product -eq $null){
	Echo_msg "$output_file" "[ERROR]-Elviz Clearing Reports Viewer wzas not installed"
}ELSE{
	Echo_msg "$output_file" "[SUCCESS]-Elviz Clearing Reports Viewer wzas installed Successfully"
}