using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace BookAPI.Database
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string connectionString;

        public MySqlConnectionFactory(IConfiguration config)
        {
            connectionString = config["ConnectionString"];
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
