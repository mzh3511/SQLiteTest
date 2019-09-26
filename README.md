# SQLiteTest
A project used to test something about the SQLite database

经测试，数据库连接和文件流是冲突的

## 1 先打开数据库连接，再打开文件流，这时打开文件流出错
2019-09-26 09:21  State -> Open  
2019-09-26 09:21  Open file failed, FileMode=Open, FileAccess=Read, FileShare=Read,   
2019-09-26 09:21  The process cannot access the file 'C:\Users\ranplan-mzh\source\repos\SqliteTest\SqliteTest\bin\Debug\DeviceDB.db' because it is being used by another process.  
2019-09-26 09:21     at System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)  
   at System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)  
   at System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share)  
   at System.IO.File.Open(String path, FileMode mode, FileAccess access, FileShare share)  
   at SqliteTest.SQLiteOperationViewModel.OpenFile() in C:\Users\ranplan-mzh\source\repos\SqliteTest\SqliteTest\SQLiteOperationViewModel.cs:line 116  

## 2 先打开只读文件流，再打开数据库连接，这时可以建立数据库连接，可以 Select 不能 Update  
2019-09-26 09:23  Open file succeed, FileMode=Open, FileAccess=Read, FileShare=Read,   
2019-09-26 09:23  State -> Open  
2019-09-26 09:23  Execute SQL Select Type,Version From TDataBaseType  
2019-09-26 09:23  Query succeed row count =1  
2019-09-26 09:23  Execute SQL UPDATE TDataBaseType SET Version = '1.1.1.99' WHERE Type = 'Device'  
2019-09-26 09:23  Query failed attempt to write a readonly database  
attempt to write a readonly database  
2019-09-26 09:23     at System.Data.SQLite.SQLite3.Reset(SQLiteStatement stmt)  
   at System.Data.SQLite.SQLite3.Step(SQLiteStatement stmt)  
   at System.Data.SQLite.SQLiteDataReader.NextResult()  
   at System.Data.SQLite.SQLiteDataReader..ctor(SQLiteCommand cmd, CommandBehavior behave)  
   at System.Data.SQLite.SQLiteCommand.ExecuteReader(CommandBehavior behavior)  
   at System.Data.SQLite.SQLiteCommand.ExecuteDbDataReader(CommandBehavior behavior)  
   at System.Data.Common.DbCommand.System.Data.IDbCommand.ExecuteReader()  
   at RanOpt.iBuilding.Common.Database.DbUtility.ExecuteReader(String commandText, IDbDataParameter[] parameters) in C:\Users\ranplan-mzh\source\repos\SqliteTest\SqliteTest\DbUtility.cs:line 65  
   at RanOpt.iBuilding.Common.Database.DbUtility.ExecuteDataTable(String commandText, IDbDataParameter[] parameters) in C:\Users\ranplan-mzh\source\repos\SqliteTest\SqliteTest\DbUtility.cs:line 72  
   at RanOpt.iBuilding.Common.Database.DbUtility.ExecuteDataTable(IDbConnection connection, String commandText, IDbDataParameter[] parameters) in C:\Users\ranplan-mzh\source\repos\SqliteTest\SqliteTest\DbUtility.cs:line 23  
   at SqliteTest.SQLiteOperationViewModel.Update() in C:\Users\ranplan-mzh\source\repos\SqliteTest\SqliteTest\SQLiteOperationViewModel.cs:line 100  
