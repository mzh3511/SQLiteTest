//  ****************************************************************************
//  Ranplan Wireless Network Design Ltd.
//  __________________
//   All Rights Reserved. [2019]
// 
//  NOTICE:
//  All information contained herein is, and remains the property of
//  Ranplan Wireless Network Design Ltd. and its suppliers, if any.
//  The intellectual and technical concepts contained herein are proprietary
//  to Ranplan Wireless Network Design Ltd. and its suppliers and may be
//  covered by U.S. and Foreign Patents, patents in process, and are protected
//  by trade secret or copyright law.
//  Dissemination of this information or reproduction of this material
//  is strictly forbidden unless prior written permission is obtained
//  from Ranplan Wireless Network Design Ltd.
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Prism.Commands;
using RanOpt.iBuilding.Common.Database;
using RanOpt.iBuilding.Common.UI;

namespace SqliteTest
{
    /// <summary>
    /// 经测试，数据库连接和文件流是冲突的
    /// 1 先打开数据库连接，再打开文件流，这时打开文件流出错
    /// 2 先打开只读文件流，再打开数据库连接，这时可以建立数据库连接，可以 Select 不能 Update
    /// </summary>
    public class SQLiteOperationViewModel : INotifyPropertyChanged
    {
        private readonly string _filePath = GetFilePath();
        private SQLiteConnection _connection;
        private string _message;
        private Stream _stream;
        private FileMode _fileMode = FileMode.Open;
        private FileAccess _fileAccess = FileAccess.Read;
        private FileShare _fileShare = FileShare.Read;

        public string Message
        {
            get => _message;
            set => PropertyChanged.RaiseIfChanged(this, ref _message, value, nameof(Message));
        }

        public IEnumerable<FileMode> FileModeList => Enum.GetValues(typeof(FileMode)).OfType<FileMode>();

        public FileMode FileMode
        {
            get => _fileMode;
            set => PropertyChanged.RaiseIfChanged(this, ref _fileMode, value, nameof(FileMode));
        }

        public IEnumerable<FileAccess> FileAccessList => Enum.GetValues(typeof(FileAccess)).OfType<FileAccess>();

        public FileAccess FileAccess
        {
            get => _fileAccess;
            set => PropertyChanged.RaiseIfChanged(this, ref _fileAccess, value, nameof(FileAccess));
        }

        public IEnumerable<FileShare> FileShareList => Enum.GetValues(typeof(FileShare)).OfType<FileShare>();

        public FileShare FileShare
        {
            get => _fileShare;
            set => PropertyChanged.RaiseIfChanged(this, ref _fileShare, value, nameof(FileShare));
        }

        public ICommand OpenCommand { get; }
        public ICommand QueryCommand { get; }
        public ICommand UpdateCommand { get; }

        public ICommand OpenFileCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SQLiteOperationViewModel()
        {
            OpenCommand = new DelegateCommand(OpenDatabase);

            QueryCommand = new DelegateCommand(Query);

            UpdateCommand = new DelegateCommand(Update);

            OpenFileCommand = new DelegateCommand(OpenFile);
        }

        private void Update()
        {
            const string sql = @"UPDATE TDataBaseType SET Version = '1.1.1.99' WHERE Type = 'Device'";

            AppendMessage($"Execute SQL {sql}");

            try
            {
                var dataTable = DbUtility.ExecuteDataTable(_connection, sql);
                AppendMessage($"Query succeed row count ={dataTable.Rows.Count}");
            }
            catch (Exception e)
            {
                AppendMessage($"Query failed {e.Message}");
                AppendMessage($"{e.StackTrace}");
            }
        }

        private void OpenFile()
        {
            try
            {
                _stream?.Dispose();

                _stream = File.Open(_filePath, FileMode, FileAccess, FileShare);

                AppendMessage($"Open file succeed, FileMode={FileMode}, FileAccess={FileAccess}, FileShare={FileShare}, ");
            }
            catch (Exception e)
            {
                AppendMessage($"Open file failed, FileMode={FileMode}, FileAccess={FileAccess}, FileShare={FileShare}, ");
                AppendMessage($"{e.Message}");
                AppendMessage($"{e.StackTrace}");
            }
        }

        private void Query()
        {
            const string sql = @"Select Type,Version From TDataBaseType";

            AppendMessage($"Execute SQL {sql}");

            try
            {
                var dataTable = DbUtility.ExecuteDataTable(_connection, sql);
                AppendMessage($"Query succeed row count ={dataTable.Rows.Count}");
            }
            catch (Exception e)
            {
                AppendMessage($"Query failed {e.Message}");
                AppendMessage($"{e.StackTrace}");
            }
        }

        private void OpenDatabase()
        {
            var connectionString = GetConnectionStringBuilder(_filePath, "12345678").ConnectionString;
            _connection = new SQLiteConnection(connectionString, true);
            _connection.StateChange += Connection_StateChange;

            _connection.Open();
        }

        private void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            AppendMessage($"State -> {_connection.State}");
        }

        private void AppendMessage(string message)
        {
            var newLine = string.IsNullOrEmpty(Message) ? string.Empty : Environment.NewLine;
            Message += $"{newLine}{DateTime.Now:yyyy-MM-dd hh:mm}  {message}";
        }

        private static SQLiteConnectionStringBuilder GetConnectionStringBuilder(string filepath, string password)
        {
            var sqlBuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = filepath,
                CacheSize = 20000000,
                Pooling = true,
                Version = 3
            };
            if (!string.IsNullOrEmpty(password))
                sqlBuilder.Password = password;
            return sqlBuilder;
        }


        private static string GetFilePath()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(dir, "DeviceDB.db");
        }
    }
}