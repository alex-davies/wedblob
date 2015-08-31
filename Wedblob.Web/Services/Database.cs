using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Wedblob.Web.Services
{
    public class Database
    {
        private readonly string _connectionString;
        private readonly string _providerName;
        private readonly DbProviderFactory _dbProviderFactory;

        public Database(string connectionStringName)
        {
            var connectionStringConfig = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringConfig == null)
                throw new InvalidOperationException("Can not find a connection string with the name '" + connectionStringName + "'");

            _connectionString = connectionStringConfig.ConnectionString;
            _providerName = !string.IsNullOrWhiteSpace(connectionStringConfig.ProviderName)
                ? connectionStringConfig.ProviderName
                : "System.Data.SqlClient";
            _dbProviderFactory = DbProviderFactories.GetFactory(_providerName);

        }

        public async Task<T> Execute<T>(Func<IDbConnection, Task<T>> func)
        {
            using (var connection = _dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                await connection.OpenAsync();
                return await func(connection);
            }

        }

        public async Task Execute<T>(Func<IDbConnection, Task> func)
        {
            using (var connection = _dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = _connectionString;
                await connection.OpenAsync();
                await func(connection);
            }
        }
    }
}