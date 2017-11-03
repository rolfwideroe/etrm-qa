$error.clear()
$Error
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null
[Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.ConnectionInfo") | Out-Null
[Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoEnum") | Out-Null
 # please specify the parameters when run the scripts
 # Ex: ./AddColumnsReportingDB.ps1  "SQLServer" "Databasename" "username" "password"
$sqlServerName = $args[0]
$reportingDatabaseName = $args[1]
$UserName = $args[2]
$Password = $args[3]
#$sqlServerName="BERPC-MF"
#$restoringDatabaseName= "VizECM1"
#$snapshotName= "VizECM_132_Snapshot"
#$UserName = "EcmDbUser"
#$Password = "EcmDbUser"

$mySrvConn = new-object Microsoft.SqlServer.Management.Common.ServerConnection
$mySrvConn.ServerInstance=$sqlServerName
$mySrvConn.LoginSecure = $false
$mySrvConn.Login = $UserName
$mySrvConn.Password = $Password
$conn = new-object ('System.Data.SqlClient.SqlConnection')
$conn.ConnectionString = "Server=" + $sqlServerName  + ";User ID=" + $UserName  + ";Password=" + $Password  + ";Database=master"
$command = new-object("System.Data.SqlClient.SqlCommand")
$command.Connection = $conn

$conn.open()

$commandText = "
USE "+$reportingDatabaseName +"; 
ALTER TABLE ContractExportsCustomFields
ADD PropertyCustom1 nvarchar(255)
ALTER TABLE ContractExportsCustomFields
	ADD PropertyCustom2 nvarchar(255)
ALTER TABLE ContractExportsCustomFields
	ADD PropertyCustom3 nvarchar(255)
ALTER TABLE ContractExportsCustomFields 
	ADD PropertyCustom4 nvarchar(255)
ALTER TABLE ContractExportsCustomFields
	ADD PropertyCustom5 nvarchar(255)
ALTER TABLE ContractExportsCustomFields
	ADD PropertyCustom6 nvarchar(255)
ALTER TABLE ContractExportsCustomFields 
	ADD PropertyCustom7 nvarchar(255)
ALTER TABLE ContractExportsCustomFields
	ADD PropertyCustom8 nvarchar(255)
ALTER TABLE ContractExportsCustomFields
	ADD PropertyCustom9 nvarchar(255)
ALTER TABLE ContractExportsCustomFields
ADD PropertyCustom10 nvarchar(255)"


$command.CommandText=$commandText

$command.ExecuteNonQuery() > $null
$conn.close()
$Error_number= $Error.Count
IF ( $Error_number -gt 0){
return 0
}Else{
	return 1
}

