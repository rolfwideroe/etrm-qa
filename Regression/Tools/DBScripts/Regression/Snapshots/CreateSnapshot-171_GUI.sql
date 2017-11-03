CREATE DATABASE VizECM_171_Snapshot ON
( NAME = VizECM_171_Data, FILENAME = 
'F:\MSSQL\Snapshots\VizECM_171_Snapshot_2017-01-20.ss' )
AS SNAPSHOT OF VizECM_171;
GO

CREATE DATABASE VizReporting_171_Snapshot ON
( NAME = VizReporting_171, FILENAME = 
'F:\MSSQL\Snapshots\VizReporting_171_Snapshot_2017-01-20.ss' )
AS SNAPSHOT OF VizReporting_171;
GO

