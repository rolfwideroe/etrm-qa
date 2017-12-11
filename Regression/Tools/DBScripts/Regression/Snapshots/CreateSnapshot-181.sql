CREATE DATABASE QAECM_Reg181_Snapshot ON
( NAME = QAECM_Reg181_Data, FILENAME = 
'D:\MSSQL13\Snapshots\QAECM_Reg181_Snapshot.ss' )
AS SNAPSHOT OF QAECM_Reg181;
GO

CREATE DATABASE QAPrices_Reg181_Snapshot ON
( NAME = QAPrices_Reg181_Data, FILENAME = 
'D:\MSSQL13\Snapshots\QAPrices_Reg181_Snapshot.ss' )
AS SNAPSHOT OF QAPrices_Reg181;
GO

CREATE DATABASE QADatawareHouse_Reg181_Snapshot ON
( NAME = QADatawareHouse_Reg181_Data, FILENAME = 
'D:\MSSQL13\Snapshots\QADatawareHouse_Reg181_Snapshot.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg181;
GO

CREATE DATABASE QASystem_Reg181_Snapshot ON
( NAME = QASystem_Reg181_Data, FILENAME = 
'D:\MSSQL13\Snapshots\QASystem_Reg181_Snapshot.ss' )
AS SNAPSHOT OF QASystem_Reg181;
GO

CREATE DATABASE QAReporting_Reg181_Snapshot ON
( NAME = QAReporting_Reg181, FILENAME = 
'D:\MSSQL13\Snapshots\QAReporting_Reg181_Snapshot.ss' )
AS SNAPSHOT OF QAReporting_Reg181;
GO

