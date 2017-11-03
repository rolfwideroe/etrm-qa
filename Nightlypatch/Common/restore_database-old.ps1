$error.clear()
$Error
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null
[Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.ConnectionInfo") | Out-Null
[Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoEnum") | Out-Null
 # please specify the parameters when run the scripts
 # Ex: ./restore_database.ps1  "SQLServer" "Databasename" "Snapshot Name" "username" "password"
$sqlServerName = $args[0]
$restoringDatabaseName = $args[1]
$snapshotName = $args[2]
$UserName = $args[3]
$Password = $args[4]

$sqlServerName="BERSV-SQL8REG"
$restoringDatabaseName= "QADatawarehouse_Reg132"
$snapshotName= "QADatawarehouse_Reg132_Snapshot"
$UserName = "EcmDbUser"
$Password = "EcmDbQaReg"
#kill all processes that don't allow restoring the database from snapshot

$mySrvConn = new-object Microsoft.SqlServer.Management.Common.ServerConnection
$mySrvConn.ServerInstance=$sqlServerName
$mySrvConn.LoginSecure = $false
$mySrvConn.Login = $UserName
$mySrvConn.Password = $Password
$srv = new-object Microsoft.SqlServer.Management.SMO.Server($mySrvConn)
$srv.KillAllProcesses($restoringDatabaseName)
$srv.KillAllProcesses($restoringDatabaseName)
$conn = new-object ('System.Data.SqlClient.SqlConnection')
$conn.ConnectionString = "Server=" + $sqlServerName  + ";User ID=" + $UserName  + ";Password=" + $Password  + ";Database=master"
$command = new-object("System.Data.SqlClient.SqlCommand")
$command.Connection = $conn

$conn.open()
$command.CommandText = "restore database " + $restoringDatabaseName + "  from database_snapshot = '" + $snapshotName + "'"
 $command.ExecuteNonQuery() > $null
$conn.close()
$Error_number= $Error.Count
IF ( $Error_number -gt 0){
return "Error restoring Snapshot"
}Else{
	return "Snapshot was restored successfully"
}

