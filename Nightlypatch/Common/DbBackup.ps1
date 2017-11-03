$DBNAME = $args[0]
$DAILYCOPY = $args[1]

$SERVER = "NETSV-dbs12reg.NORTEST.BRADYPLC.COM"
$date = Get-Date -Format yyyyMMddHHmmss
$DestFolder= "\\NETSV-dbs12reg.NORTEST.BRADYPLC.COM\DBBackup_AfterPatch"
#$dbname = 'QASystem_Reg151'
if (!(Test-Path "C:\Windows\System32\WindowsPowerShell\v1.0\Modules\SQLPS")){
	MKDIR "C:\Windows\System32\WindowsPowerShell\v1.0\Modules\SQLPS" -force
	&xcopy /Y /s "\\NETVS-DEPLOY\Installations\QASCRIPTS\Common\PSModule\SQLPS\*"  "C:\Windows\System32\WindowsPowerShell\v1.0\Modules\SQLPS\"
	import-module sqlps 
}

Backup-SqlDatabase -ServerInstance $SERVER -Database $DBNAME -BackupFile "$DestFolder\$($DBNAME)_db_$($date).bak"

if($DAILYCOPY -eq "True")
{
	#$GUISERVER = "NETSV-DBS12REG"
	#$GUIDBNAME = "VizECM_151"
	$DbPrefix = "DAILY_"
	$DbBcakupName=$DbPrefix + $DBNAME +".Bak"

	$GUIDestFolder= "\\NETSV-dbs12reg.NORTEST.BRADYPLC.COM\Daily_QAREG"
	if(test-path "$GUIDestFolder\$DbBcakupName"){
		DEL "$GUIDestFolder\$DbBcakupName" -force
	}

	Backup-SqlDatabase -ServerInstance $SERVER -Database $DBNAME -BackupFile "$GUIDestFolder\$DbBcakupName" -Initialize
}