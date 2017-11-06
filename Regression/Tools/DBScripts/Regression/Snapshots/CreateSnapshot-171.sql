CREATE DATABASE QAECM_Reg171_Snapshot ON
( NAME = QAECM_Reg171_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAECM_Reg171_Snapshot.ss' )
AS SNAPSHOT OF QAECM_Reg171;
GO

CREATE DATABASE QAPrices_Reg171_Snapshot ON
( NAME = QAPrices_Reg171_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAPrices_Reg171_Snapshot.ss' )
AS SNAPSHOT OF QAPrices_Reg171;
GO

CREATE DATABASE QADatawareHouse_Reg171_Snapshot ON
( NAME = QADatawareHouse_Reg171_Data, FILENAME = 
'F:\MSSQL\Snapshots\QADatawareHouse_Reg171_Snapshot.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg171;
GO

CREATE DATABASE QASystem_Reg171_Snapshot ON
( NAME = QASystem_Reg171_Data, FILENAME = 
'F:\MSSQL\Snapshots\QASystem_Reg171_Snapshot.ss' )
AS SNAPSHOT OF QASystem_Reg171;
GO

CREATE DATABASE QAReporting_Reg171_Snapshot ON
( NAME = QAReporting_Reg171, FILENAME = 
'F:\MSSQL\Snapshots\QAReporting_Reg171_Snapshot.ss' )
AS SNAPSHOT OF QAReporting_Reg171;
GO
