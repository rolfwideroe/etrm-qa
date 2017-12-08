CREATE DATABASE QAECM_Reg172_Snapshot ON
( NAME = QAECM_Reg172_Data, FILENAME = 
'D:\MSSQL13\Snapshots\QAECM_Reg172_Snapshot.ss' )
AS SNAPSHOT OF QAECM_Reg172;
GO

CREATE DATABASE QAPrices_Reg172_Snapshot ON
( NAME = QAPrices_Reg172_Data, FILENAME = 
'D:\MSSQL13\Snapshots\QAPrices_Reg172_Snapshot.ss' )
AS SNAPSHOT OF QAPrices_Reg172;
GO

CREATE DATABASE QADatawareHouse_Reg172_Snapshot ON
( NAME = QADatawareHouse_Reg172_Data, FILENAME = 
'D:\MSSQL13\Snapshots\QADatawareHouse_Reg172_Snapshot.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg172;
GO

CREATE DATABASE QASystem_Reg172_Snapshot ON
( NAME = QASystem_Reg172_Data, FILENAME = 
'D:\MSSQL13\Snapshots\QASystem_Reg172_Snapshot.ss' )
AS SNAPSHOT OF QASystem_Reg172;
GO

CREATE DATABASE QAReporting_Reg172_Snapshot ON
( NAME = QAReporting_Reg172, FILENAME = 
'D:\MSSQL13\Snapshots\QAReporting_Reg172_Snapshot.ss' )
AS SNAPSHOT OF QAReporting_Reg172;
GO

