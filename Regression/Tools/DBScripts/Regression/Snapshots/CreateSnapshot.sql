CREATE DATABASE QAECM_Reg112_Snapshot ON
( NAME = QAECM_Reg112_Data, FILENAME = 
'e:\qaregs\112\QAECM_Reg112_Snapshot_2012-09-25a.ss' )
AS SNAPSHOT OF QAECM_Reg112;
GO

CREATE DATABASE QADatawareHouse_Reg112_Snapshot ON
( NAME = QADatawareHouse_Reg112_Data, FILENAME = 
'e:\qaregs\112\QADatawareHouse_Reg112_Snapshot_2012-09-03.ss' )
AS SNAPSHOT OF QADatawareHouse_Reg112;
GO
