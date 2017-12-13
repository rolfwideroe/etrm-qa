CREATE DATABASE VizECM_172_Snapshot ON
( NAME = VizECM_172_Data, FILENAME = 
'D:\MSSQL13\Snapshots\VizECM_172_Snapshot.ss' )
AS SNAPSHOT OF VizECM_172;
GO

CREATE DATABASE VizReporting_172_Snapshot ON
( NAME = VizReporting_172, FILENAME = 
'D:\MSSQL13\Snapshots\VizReporting_172_Snapshot.ss' )
AS SNAPSHOT OF VizReporting_172;
GO

