using System;

namespace TeddyBlazor.Data
{
    public class PostgresUrlParser
    {
        public PostgresUrlParser(string database_url)
        {
            if (!database_url.StartsWith("postgres://"))
                throw new ArgumentOutOfRangeException(nameof(database_url), "database_url should start with postgres://");
            var words = database_url.Split(':', '@', '/');
            var user = words[3];
            var password = words[4];
            var host = words[5];
            var port = words[6];
            var database = words[7];
            HerokuDatabaseUrl = database_url;
            DotNetConnectionString = $"Server={host}; Port={port}; Database={database}; User ID={user}; Password={password}; Trust Server Certificate = True; "; //SSL Mode=Require; 
            PsqlConnectionCommand = $"docker run -e PGPASSWORD={password} --rm -it postgres psql -h {host} -U {user} {database}"; // --set = sslmode = require
        }
        public string HerokuDatabaseUrl { get; }
        public string DotNetConnectionString { get; }
        public string PsqlConnectionCommand { get; }
        public static string ParseConnectionString(string database_url)
        {
            var parser = new PostgresUrlParser(database_url);
            return parser.DotNetConnectionString;
        }
    }
}