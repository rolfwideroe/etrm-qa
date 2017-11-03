$SERVER = "NETSV-DBS12REG"
$DBNAME = "VizECM_151"
$DbBcakupName="DAILY_VizECM_151.Bak"
$date = Get-Date -Format yyyyMMddHHmmss
$DestFolder= "\\NETSV-dbs12reg.NORTEST.BRADYPLC.COM\Daily_QAREG"
if(test-path "\\NETSV-dbs12reg.NORTEST.BRADYPLC.COM\Daily_QAREG\$DbBcakupName"){
	DEL "\\NETSV-dbs12reg.NORTEST.BRADYPLC.COM\Dailt_QAREG\$DbBcakupName" -force
}
#$dbname = 'QASystem_Reg151' 
if (!(Test-Path "C:\Windows\System32\WindowsPowerShell\v1.0\Modules\SQLPS")){
	MKDIR "C:\Windows\System32\WindowsPowerShell\v1.0\Modules\SQLPS" -force
	&xcopy /Y /s "\\BERSVDEPLOY\Installations\QASCRIPTS\Common\PSModule\SQLPS\*"  "C:\Windows\System32\WindowsPowerShell\v1.0\Modules\SQLPS\"
	import-module sqlps 
}

Backup-SqlDatabase -ServerInstance $SERVER -Database $DBNAME -BackupFile "$DestFolder\$DbBcakupName" -Initialize
