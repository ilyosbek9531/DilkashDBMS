USE master
CREATE DATABASE Dilkash
	ON (FILENAME = 'C:\Users\orifj\Desktop\DilkashDBMS\DilkashDBMS\AppData\Dilkash.mdf'),
	   (FILENAME = 'C:\Users\orifj\Desktop\DilkashDBMS\DilkashDBMS\AppData\Dilkash.ldf')
	FOR ATTACH

--shrink DB log file
USE Dilkash
--select * FROM sys.database_files
ALTER DATABASE Dilkash SET RECOVERY SIMPLE
GO
DBCC SHRINKFILE (Dilkash_log, 7)