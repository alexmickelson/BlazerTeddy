using System;
using System.IO;
using Dapper;
using FluentAssertions;
using Npgsql;
using NUnit.Framework;
using TeddyBlazor.Data;

namespace IntegrationTests.DataTests
{
    public class DbConnectionValidatorTests
    {
        private Func<NpgsqlConnection> getDbConnection;

        [SetUp]
        public void Setup()
        {
            getDbConnection = () => new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;");
        }

        [Test]
        public void throws_error_when_no_database_exists()
        {
            var connectionString = "Server=notLocalhost;Port=2;User ID=not-teddy;Password=probablyRedactedOrSomething;";
            Func<NpgsqlConnection> getBogusDbConnection = () => new NpgsqlConnection(connectionString);
            var dbConnectionValidator = new DbConnectionValidator(getBogusDbConnection);

            Action act = (() => dbConnectionValidator.validateConnection());

            act.Should().Throw<Npgsql.NpgsqlException>()
                .WithMessage($"Cannot Connect to database with connection String {connectionString}");
        }

        [Test]
        public void creates_tables_if_dont_exist()
        {
            var dropTableSqlPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../postgres/DropTables.sql");
            var dropTablesSql = File.ReadAllText(dropTableSqlPath);
            var dbConnectionValidator = new DbConnectionValidator(getDbConnection);
            using(var dbConnection = getDbConnection())
            {
                dbConnection.Execute(dropTablesSql);
            }
            var startingTableCount = dbConnectionValidator.getTableCount();

            dbConnectionValidator.validateConnection();
            var endingTablecount = dbConnectionValidator.getTableCount();

            startingTableCount.Should().Be(0);
            endingTablecount.Should().BeInRange(5, 100);
        }

        [Test]
        public void dont_drop_tables_if_exist()
        {
            var dbConnectionValidator = new DbConnectionValidator(getDbConnection);
            int studentId;
            using(var dbConnection = getDbConnection())
            {
                studentId = dbConnection.QueryFirst<int>("INSERT INTO Student (StudentName) VALUES ('Tim') RETURNING StudentId;");
            }

            dbConnectionValidator.validateConnection();

            int timCount;
            using(var dbConnection = getDbConnection())
            {
                timCount = dbConnection.QueryFirst<int>(
                    @"select count(StudentId) from student where studentid = @studentId;",
                    new {studentId = studentId});
            }
            timCount.Should().Be(1);
        }
    }
}