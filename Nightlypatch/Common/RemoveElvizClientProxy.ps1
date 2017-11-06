$PATCHTOOLFOLDER="\\bersvdeploy\INSTALLATIONS\QASCRIPTS\util"
$product=Get-WmiObject -Class Win32_Product -Filter "name='Elviz ETRM (Application proxy)'" -ComputerName "localhost"
$UninstallGUID = $product.IdentifyingNumber
IF( $UninstallGUID -ne $null){
	Echo "[INFO] Removing elviz ETRM (Application proxy)"
	$MsiExitCode= (Start-Process -FilePath "msiexec.exe" -ArgumentList "/X$UninstallGUID /qn" -Wait -Passthru).ExitCode
	IF ($MsiExitCode -eq 0){
		Echo "[SUCCESS] Removed elviz ETRM (Application proxy)"
	}ElSEIF($MsiExitCode -eq 1605){
		Echo  "[INFO] elviz ETRM (Application proxy) is not installed on this machine"
	}ELSE{
		Echo  "[ERROR] This Command msiexec.exe /X$UninstallGUID /qn"
		Echo  "[ERROR] Exited with ExitCode $MsiExitCode"
		Echo "[INFO] Please try to remove elviz ETRM (Application proxy) then run the script again"
				#Start-Sleep -Seconds 10
	}
}ELSE{
	Echo "[INFO] elviz ETRM (Application proxy) is not installed on this machine" 
}
