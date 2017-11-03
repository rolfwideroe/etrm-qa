CREATE DATABASE QAECM_Reg162_Snapshot ON
( NAME = QAECM_Reg162_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAECM_Reg162_Snapshot.ss' )
AS SNAPSHOT OF QAECM_Reg162;
GO

CREATE DATABASE QAPrices_Reg162_Snapshot ON
( NAME = QAPrices_Reg162_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAPrices_Reg162_Snapshot.ss' )
AS SNAPSHOT OF QAPrices_Reg162;
GO

CREATE DATABASE QADatawareHouse_Reg162_Snapshot ON
( NAME = QADatawareHouse_Reg162_Data, FILENAME = 
'F:\MSSQL\Snapshots\QADatawareHouse_Reg162_Snapshot.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg162;
GO

CREATE DATABASE QASystem_Reg162_Snapshot ON
( NAME = QASystem_Reg162_Data, FILENAME = 
'F:\MSSQL\Snapshots\QASystem_Reg162_Snapshot.ss' )
AS SNAPSHOT OF QASystem_Reg162;
GO

CREATE DATABASE QAReporting_Reg162_Snapshot ON
( NAME = QAReporting_Reg162, FILENAME = 
'F:\MSSQL\Snapshots\QAReporting_Reg162_Snapshot.ss' )
AS SNAPSHOT OF QAReporting_Reg162;
GO
