CREATE DATABASE QAECM_Reg161_Snapshot ON
( NAME = QAECM_Reg161_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAECM_Reg161_Snapshot.ss' )
AS SNAPSHOT OF QAECM_Reg161;
GO

CREATE DATABASE QAPrices_Reg161_Snapshot ON
( NAME = QAPrices_Reg161_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAPrices_Reg161_Snapshot.ss' )
AS SNAPSHOT OF QAPrices_Reg161;
GO

CREATE DATABASE QADatawareHouse_Reg161_Snapshot ON
( NAME = QADatawareHouse_Reg161_Data, FILENAME = 
'F:\MSSQL\Snapshots\QADatawareHouse_Reg161_Snapshot.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg161;
GO

CREATE DATABASE QASystem_Reg161_Snapshot ON
( NAME = QASystem_Reg161_Data, FILENAME = 
'F:\MSSQL\Snapshots\QASystem_Reg161_Snapshot.ss' )
AS SNAPSHOT OF QASystem_Reg161;
GO

CREATE DATABASE QAReporting_Reg161_Snapshot ON
( NAME = QAReporting_Reg161, FILENAME = 
'F:\MSSQL\Snapshots\QAReporting_Reg161_Snapshot.ss' )
AS SNAPSHOT OF QAReporting_Reg161;
GO
