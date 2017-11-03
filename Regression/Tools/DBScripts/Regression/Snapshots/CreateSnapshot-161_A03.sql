CREATE DATABASE QAECM_Reg161_Snapshot_A03 ON
( NAME = QAECM_Reg161_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAECM_Reg161_Snapshot_A03.ss' )
AS SNAPSHOT OF QAECM_Reg161_A03;
GO

CREATE DATABASE QAPrices_Reg161_Snapshot_A03 ON
( NAME = QAPrices_Reg161_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAPrices_Reg161_Snapshot_A03.ss' )
AS SNAPSHOT OF QAPrices_Reg161_A03;
GO

CREATE DATABASE QADatawareHouse_Reg161_Snapshot_A03 ON
( NAME = QADatawareHouse_Reg161_Data, FILENAME = 
'F:\MSSQL\Snapshots\QADatawareHouse_Reg161_Snapshot_A03.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg161_A03;
GO

CREATE DATABASE QASystem_Reg161_Snapshot_A03 ON
( NAME = QASystem_Reg161_Data, FILENAME = 
'F:\MSSQL\Snapshots\QASystem_Reg161_Snapshot_A03.ss' )
AS SNAPSHOT OF QASystem_Reg161_A03;
GO

CREATE DATABASE QAReporting_Reg161_Snapshot_A03 ON
( NAME = QAReporting_Reg161, FILENAME = 
'F:\MSSQL\Snapshots\QAReporting_Reg161_Snapshot_A03.ss' )
AS SNAPSHOT OF QAReporting_Reg161_A03;
GO
