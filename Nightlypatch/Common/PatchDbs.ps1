$Release = $args[0]
$SERVER = $args[1]
$USERNAME = $args[2]
$PASSWORD = $args[3]
$DBSYS = $args[4]
$DBECM = $args[5]
$DBPRC = $args[6]
$DBDWH = $args[7]


cls
#$Release="2014.1"
if(($Release -eq $null) -or ($SERVER -eq $null) -or ($USERNAME -eq $null) -or ($PASSWORD -eq $null) -or ($DBSYS -eq $null) -or ($DBECM -eq $null) -or ($DBPRC -eq $null)){
	echo " ************ Powershell script to patch Elviz Datbases on certain SQL server ********"
	echo "	Description: PatchDbs.ps1 should be called with parameters to work correctly"
	echo "	Usage: .\PatchDbs.ps1  releaseNumber SQLServer username password VizSystem VizECM VizPrices VizDatawarehouse"
	echo "	VizDatawareHouse param is optional"
}else{
	$guid ="{E957CFF0-DB0B-43B9-ACF3-BF11EAF2EB59}"
	$DbPatch_Path="\\NETVS-DEPLOY.nortest.bradyplc.com\Installations\DB_Patches\$Release\ElvizDbPatch.msi"
	#	$SERVER="BERPC-MF8"
	#    $USERNAME="EcmDbUser"
	#    $PASSWORD="EcmDbUser"
	#    $DBSYS="SYS132"
	#    $DBECM="ECM132"
	#	$DBPRC="PRC132"
	#	$DBDWH=""

	IF(Test-Path $DbPatch_Path){
		
	}ELSE{
		Echo "[ERROR] $DbPatch_Path this path does not exist, check the network connection"
	}
	&cmd /c "msiexec /X$guid /qn" 
	echo "& msiexec /X$guid /qn"

	if($DBDWH -eq $null){
		&cmd /c "msiexec /i $DbPatch_Path  /qn ALLUSERS=1 IS_SQLSERVER_SERVER=$SERVER IS_SQLSERVER_USERNAME=$USERNAME IS_SQLSERVER_PASSWORD=$PASSWORD IS_SQLSERVER_DBSYS=$DBSYS IS_SQLSERVER_DBECM=$DBECM IS_SQLSERVER_DBPRC=$DBPRC"  | Out-Null
		echo "msiexec /i $DbPatch_Path  /qn /lv D:\log.txt ALLUSERS=1 IS_SQLSERVER_SERVER=$SERVER IS_SQLSERVER_USERNAME=$USERNAME IS_SQLSERVER_PASSWORD=$PASSWORD IS_SQLSERVER_DBSYS=$DBSYS IS_SQLSERVER_DBECM=$DBECM IS_SQLSERVER_DBPRC=$DBPRC"
	}Elseif ($DBDWH -ne $null){
		&cmd /c "msiexec /i $DbPatch_Path  /qn ALLUSERS=1 IS_SQLSERVER_SERVER=$SERVER IS_SQLSERVER_USERNAME=$USERNAME IS_SQLSERVER_PASSWORD=$PASSWORD IS_SQLSERVER_DBSYS=$DBSYS IS_SQLSERVER_DBECM=$DBECM IS_SQLSERVER_DBPRC=$DBPRC IS_SQLSERVER_DBDWH=$DBDWH" | Out-Null
		echo "msiexec /i $DbPatch_Path  /qn /lv D:\log.txt ALLUSERS=1 IS_SQLSERVER_SERVER=$SERVER IS_SQLSERVER_USERNAME=$USERNAME IS_SQLSERVER_PASSWORD=$PASSWORD IS_SQLSERVER_DBSYS=$DBSYS IS_SQLSERVER_DBECM=$DBECM IS_SQLSERVER_DBPRC=$DBPRC IS_SQLSERVER_DBDWH=$DBDWH"
	}
}