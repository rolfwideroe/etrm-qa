
USE master;
GO
ALTER DATABASE QAECM_Reg162
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO
ALTER DATABASE QAECM_Reg162
SET MULTI_USER;
GO

RESTORE DATABASE QAECM_Reg162 FROM DATABASE_SNAPSHOT = 'QAECM_Reg162_Snapshot';
GO

USE master;
GO
ALTER DATABASE QAReporting_Reg162
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO
ALTER DATABASE QAReporting_Reg162
SET MULTI_USER;
GO

RESTORE DATABASE QAReporting_Reg162 FROM DATABASE_SNAPSHOT = 'QAReporting_Reg162_Snapshot';
GO 




USE master;
GO
ALTER DATABASE QAPrices_Reg162
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO
ALTER DATABASE QAPrices_Reg162
SET MULTI_USER;
GO

RESTORE DATABASE QAPrices_Reg162 FROM DATABASE_SNAPSHOT = 'QAPrices_Reg162_Snapshot';
GO 

USE master;
GO
ALTER DATABASE QADatawareHouse_Reg162
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO
ALTER DATABASE QADatawareHouse_Reg162
SET MULTI_USER;
GO

RESTORE DATABASE QADatawareHouse_Reg162 FROM DATABASE_SNAPSHOT = 'QADatawareHouse_Reg162_Snapshot';
GO 

USE master;
GO
ALTER DATABASE QASystem_Reg162
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO
ALTER DATABASE QASystem_Reg162
SET MULTI_USER;
GO

RESTORE DATABASE QASystem_Reg162 FROM DATABASE_SNAPSHOT = 'QASystem_Reg162_Snapshot';
GO 

