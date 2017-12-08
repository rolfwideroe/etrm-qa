CREATE DATABASE VizECM_171_Snapshot ON
( NAME = VizECM_171_Data, FILENAME = 
'D:\MSSQL13\Snapshots\VizECM_171_Snapshot.ss' )
AS SNAPSHOT OF VizECM_171;
GO

CREATE DATABASE VizReporting_171_Snapshot ON
( NAME = VizReporting_171, FILENAME = 
'D:\MSSQL13\Snapshots\VizReporting_171_Snapshot.ss' )
AS SNAPSHOT OF VizReporting_171;
GO

