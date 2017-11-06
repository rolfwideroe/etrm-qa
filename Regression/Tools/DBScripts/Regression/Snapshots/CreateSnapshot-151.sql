CREATE DATABASE QAECM_Reg151_Snapshot ON
( NAME = QAECM_Reg151_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAECM_Reg151_Snapshot_2014-12-11.ss' )
AS SNAPSHOT OF QAECM_Reg151;
GO

CREATE DATABASE QAPrices_Reg151_Snapshot ON
( NAME = QAPrices_Reg151_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAPrices_Reg151_Snapshot_2014-12-11.ss' )
AS SNAPSHOT OF QAPrices_Reg151;
GO

CREATE DATABASE QADatawareHouse_Reg151_Snapshot ON
( NAME = QADatawareHouse_Reg151_Data, FILENAME = 
'F:\MSSQL\Snapshots\QADatawareHouse_Reg151_Snapshot_2014-12-11.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg151;
GO
