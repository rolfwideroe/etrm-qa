

USE master;
GO
ALTER DATABASE QAECM_Reg112
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO
ALTER DATABASE QAECM_Reg112
SET MULTI_USER;
GO

USE master;
GO
ALTER DATABASE QADatawareHouse_Reg112
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO
ALTER DATABASE QADatawareHouse_Reg112
SET MULTI_USER;
GO

USE master
RESTORE DATABASE QAECM_Reg112 FROM DATABASE_SNAPSHOT = 'QAECM_Reg112_Snapshot';
GO

RESTORE DATABASE QADatawareHouse_Reg112 FROM DATABASE_SNAPSHOT = 'QADatawareHouse_Reg112_Snapshot';
GO 