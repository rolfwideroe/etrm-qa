CREATE DATABASE QAECM_Reg163_Snapshot ON
( NAME = QAECM_Reg163_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAECM_Reg163_Snapshot.ss' )
AS SNAPSHOT OF QAECM_Reg163;
GO

CREATE DATABASE QAPrices_Reg163_Snapshot ON
( NAME = QAPrices_Reg163_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAPrices_Reg163_Snapshot.ss' )
AS SNAPSHOT OF QAPrices_Reg163;
GO

CREATE DATABASE QADatawareHouse_Reg163_Snapshot ON
( NAME = QADatawareHouse_Reg163_Data, FILENAME = 
'F:\MSSQL\Snapshots\QADatawareHouse_Reg163_Snapshot.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg163;
GO

CREATE DATABASE QASystem_Reg163_Snapshot ON
( NAME = QASystem_Reg163_Data, FILENAME = 
'F:\MSSQL\Snapshots\QASystem_Reg163_Snapshot.ss' )
AS SNAPSHOT OF QASystem_Reg163;
GO

CREATE DATABASE QAReporting_Reg163_Snapshot ON
( NAME = QAReporting_Reg163, FILENAME = 
'F:\MSSQL\Snapshots\QAReporting_Reg163_Snapshot.ss' )
AS SNAPSHOT OF QAReporting_Reg163;
GO
