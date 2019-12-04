using System;
using System.Data;
using System.IO;
using Dapper;

namespace TeddyBlazor.Data
{
    public class DbConnectionValidator
    {
        private readonly Func<IDbConnection> getDbConnection;

        public DbConnectionValidator(Func<IDbConnection> getDbConnection)
        {
            this.getDbConnection = getDbConnection;
        }
        public void validateConnection()
        {
            int tableCount;
            var connectionString = getConnectionString();
            try
            {
                tableCount = getTableCount();
            }
            catch (Exception)
            {
                throw new Npgsql.NpgsqlException($"Cannot Connect to database with connection String {connectionString}");
            }

            if(noTablesInDatabase(tableCount))
            {
                createTables();
            }

        }

        private void createTables()
        {
            var createTableSqlPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../postgres/startup/CreateTables.sql");
            var createTablesSql = File.ReadAllText(createTableSqlPath);
            using(var dbConnection = getDbConnection())
            {
                dbConnection.Execute(createTablesSql);
            }        
        }

        private string getConnectionString()
        {
            using (var dbConnection = getDbConnection())
            {
                return dbConnection.ConnectionString;
            }
        }

        private bool noTablesInDatabase(int tableCount)
        {
            return tableCount < 1;
        }

        public int getTableCount()
        {
            using (var dbConnection = getDbConnection())
            {
                return dbConnection.QueryFirstOrDefault<int>(
                    @"select count(tablename) from pg_tables 
                          where schemaname='public';");
            }
        }
    }
}