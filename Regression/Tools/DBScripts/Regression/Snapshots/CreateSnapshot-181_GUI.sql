CREATE DATABASE VizECM_181_Snapshot ON
( NAME = VizECM_181_Data, FILENAME = 
'D:\MSSQL13\Snapshots\VizECM_181_Snapshot.ss' )
AS SNAPSHOT OF VizECM_181;
GO

CREATE DATABASE VizReporting_181_Snapshot ON
( NAME = VizReporting_181, FILENAME = 
'D:\MSSQL13\Snapshots\VizReporting_181_Snapshot.ss' )
AS SNAPSHOT OF VizReporting_181;
GO

