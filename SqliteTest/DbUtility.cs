using System.Data;
using System.Data.Common;

namespace RanOpt.iBuilding.Common.Database
{
    public class DbUtility
    {
        private readonly DbConnection _connection;
        private readonly DbProviderFactory _dbProviderFactory;

        public static object ExecuteScalar(IDbConnection connection, string commandText, params IDbDataParameter[] parameters)
        {
            return new DbUtility(connection).ExecuteScalar(commandText, parameters);
        }

        public static int ExecuteNonQuery(IDbConnection connection, string commandText, params IDbDataParameter[] parameters)
        {
            return new DbUtility(connection).ExecuteNonQuery(commandText, parameters);
        }

        public static DataTable ExecuteDataTable(IDbConnection connection, string commandText, params IDbDataParameter[] parameters)
        {
            return new DbUtility(connection).ExecuteDataTable(commandText, parameters);
        }

        public static IDataReader ExecuteReader(IDbConnection connection, string commandText, params IDbDataParameter[] parameters)
        {
            return new DbUtility(connection).ExecuteReader(commandText, parameters);
        }

        public DbUtility(IDbConnection connection)
            : this((DbConnection)connection)
        { }

        public DbUtility(DbConnection connection)
        {
            _connection = connection;
            _dbProviderFactory = DbProviderFactories.GetFactory(_connection);
        }

        public IDbCommand CreateCommand(string commandText)
        {
            var command = _connection.CreateCommand();
            command.CommandText = commandText;

            return command;
        }

        public IDbDataParameter CreateParameter(string parameterName, object value)
        {
            var parameter = _dbProviderFactory.CreateParameter();
            // ReSharper disable once PossibleNullReferenceException
            parameter.ParameterName = parameterName;
            parameter.Value = value;

            return parameter;
        }

        public IDataReader ExecuteReader(string commandText, params IDbDataParameter[] parameters)
        {
            using (var command = CreateCommand(commandText))
            {
                foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
                return command.ExecuteReader();
            }
        }

        public DataTable ExecuteDataTable(string commandText, params IDbDataParameter[] parameters)
        {
            var result = new DataTable();
            using (var reader = ExecuteReader(commandText, parameters))
                result.Load(reader);

            return result;
        }

        public object ExecuteScalar(string commandText, params IDbDataParameter[] parameters)
        {
            using (var command = CreateCommand(commandText))
            {
                foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);

                return command.ExecuteScalar();
            }
        }

        public int ExecuteNonQuery(string commandText, params IDbDataParameter[] parameters)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = commandText;
                foreach (var dbDataParameter in parameters)
                    command.Parameters.Add(dbDataParameter);

                return command.ExecuteNonQuery();
            }
        }
    }
}