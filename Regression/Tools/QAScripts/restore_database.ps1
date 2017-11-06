Param(
    [string] $sqlServerName,
 	[String]$restoringDatabaseName,
	[String]$snapshotName,
	[String]$UserName,
	[String]$Password

)
$error.clear()
$Error
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null
[Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.ConnectionInfo") | Out-Null
[Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoEnum") | Out-Null
 # please specify the parameters when run the scripts
 # Ex: ./restore_database.ps1  "SQLServer" "Databasename" "Snapshot Name" "username" "password"
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

$commandText = "USE master; 

ALTER DATABASE "+ $restoringDatabaseName +" SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
 
ALTER DATABASE " + $restoringDatabaseName + " SET MULTI_USER; 
 
restore database " + $restoringDatabaseName + "  from database_snapshot = '" + $snapshotName + "'"

$command.CommandText=$commandText

$command.ExecuteNonQuery() > $null
$conn.close()
$Error_number= $Error.Count
IF ( $Error_number -gt 0){
return 0
}Else{
	return 1
}

