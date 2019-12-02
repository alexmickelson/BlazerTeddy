using System;
using System.Data;
using ServiceStack.OrmLite;

namespace Test.ServiceTests
{
    public static class DbConnectionFactory
    {
        public static Func<IDbConnection> GetMemoryConnection()
        {
            var dbFactory = new OrmLiteConnectionFactory(":memory:", ServiceStack.OrmLite.SqliteDialect.Provider);
            return () => dbFactory.Open();
        }
    }
}