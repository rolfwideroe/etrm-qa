CREATE DATABASE QAECM_Reg153_Snapshot ON
( NAME = QAECM_Reg153_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAECM_Reg153_Snapshot.ss' )
AS SNAPSHOT OF QAECM_Reg153;
GO

CREATE DATABASE QAPrices_Reg153_Snapshot ON
( NAME = QAPrices_Reg153_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAPrices_Reg153_Snapshot.ss' )
AS SNAPSHOT OF QAPrices_Reg153;
GO

CREATE DATABASE QADatawareHouse_Reg153_Snapshot ON
( NAME = QADatawareHouse_Reg153_Data, FILENAME = 
'F:\MSSQL\Snapshots\QADatawareHouse_Reg153_Snapshot.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg153;
GO

CREATE DATABASE QASystem_Reg153_Snapshot ON
( NAME = QASystem_Reg153_Data, FILENAME = 
'F:\MSSQL\Snapshots\QASystem_Reg153_Snapshot.ss' )
AS SNAPSHOT OF QASystem_Reg153;
GO
