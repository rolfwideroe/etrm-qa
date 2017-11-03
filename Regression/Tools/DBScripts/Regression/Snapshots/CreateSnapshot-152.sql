CREATE DATABASE QAECM_Reg152_Snapshot ON
( NAME = QAECM_Reg152_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAECM_Reg152_Snapshot.ss' )
AS SNAPSHOT OF QAECM_Reg152;
GO

CREATE DATABASE QAPrices_Reg152_Snapshot ON
( NAME = QAPrices_Reg152_Data, FILENAME = 
'F:\MSSQL\Snapshots\QAPrices_Reg152_Snapshot.ss' )
AS SNAPSHOT OF QAPrices_Reg152;
GO

CREATE DATABASE QADatawareHouse_Reg152_Snapshot ON
( NAME = QADatawareHouse_Reg152_Data, FILENAME = 
'F:\MSSQL\Snapshots\QADatawareHouse_Reg152_Snapshot.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg152;
GO
