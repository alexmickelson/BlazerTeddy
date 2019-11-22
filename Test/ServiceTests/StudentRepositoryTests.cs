
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ServiceStack.OrmLite;
using TeddyBlazor.Data;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace Test.ServiceTests
{
    public class StudentRepositoryTests
    {
        private DbContextOptions<OurDbContext> dbOptions;
        private StudentRepository TeddyBlazorRepostiory;
        private OurDbContext context;
        private OurDbContext context2;
        private OrmLiteConnectionFactory dbFactory;
        private SqliteConnection sqlite;
        private Mock<IConfiguration> configMoq;


        [SetUp]
        public void Setup()
        {

            var dbOptions = new DbContextOptionsBuilder<OurDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentRepository")
                .EnableSensitiveDataLogging()
                .Options;
            context = new OurDbContext(dbOptions);
            context2 = new OurDbContext(dbOptions);


            dbFactory = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);

            string createTableScript = File.ReadAllText(Directory.GetCurrentDirectory() + "/../../../../TeddyBlazor/Data/SqlQueries/CreateTables.sql");
            string seedDatabaseScript = File.ReadAllText(Directory.GetCurrentDirectory() + "/../../../../TeddyBlazor/Data/SqlQueries/SeedDatabase.sql");
            using (var connection = dbFactory.Open())
            {
                var s = connection.QueryMultiple(createTableScript);
                var e = connection.QueryMultiple(seedDatabaseScript);
            }

            TeddyBlazorRepostiory = new StudentRepository(context, () => dbFactory.Open());
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task can_read_from_in_memory_database()
        {
            IEnumerable<Student> students;
            using (var connection = dbFactory.Open())
            {
                var sqlResults = connection.QueryMultiple("select * from TeddyBlazor;");
                students = sqlResults.Read<Student>();
            }
            students.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void can_write_to_in_memory_database()
        {
            var max = new Student(){Name = "max"};
            SqlMapper.GridReader sqlResults;
            
            using (var connection = dbFactory.Open())
            {
                connection.Insert<Student>(max);
            }
            using (var connection = dbFactory.Open())
            {
                sqlResults = connection.QueryMultiple("select * from TeddyBlazor;");
            }
            sqlResults.Read<Student>()
                      .Where(s => s.Name == max.Name)
                      .Should()
                      .NotBeNullOrEmpty();
        }

        // [Test]
        // public async Task EditingReferenceOfTeddyBlazorListEditsRepostoryList()
        // {
        //     var students = TeddyBlazorRepostiory.GetList();
        //     var TeddyBlazor = new TeddyBlazorInfo()
        //     {
        //         Name = "Sam"
        //     };

        //     students.Add(TeddyBlazor);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();
        //     await TeddyBlazorRepostiory.InitializeTeddyBlazorAsync();

        //     var sam = TeddyBlazorRepostiory.Get(3);
        //     students.Should().BeSameAs(TeddyBlazorRepostiory.GetList());
        //     sam.Name.Should().Be(TeddyBlazor.Name);
        // }

        [Test]
        public void AddingNotesOnstudentsPersistsToRepostitory()
        {
            var students = TeddyBlazorRepostiory.GetList();
            int TeddyBlazorId = 3;
            var TeddyBlazor = new Student()
            {
                StudentId = TeddyBlazorId,
                Name = "Sam"
            };

            students.Add(TeddyBlazor);
            TeddyBlazor.Notes = new List<Note>();
            TeddyBlazor.Notes.Add(new Note() { Content = "this is a note" });

            var sam = TeddyBlazorRepostiory.Get(TeddyBlazorId);
            sam.Notes.First().Content.Should().Be("this is a note");
        }

        // [Test]
        // public async Task ChangesPersistAfterReloadingDatabase()
        // {
        //     var students = TeddyBlazorRepostiory.GetList();
        //     int TeddyBlazorId = 3;
        //     var TeddyBlazor = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = TeddyBlazorId,
        //         Name = "Sam"
        //     };
        //     students.Add(TeddyBlazor);

        //     await TeddyBlazorRepostiory.InitializeTeddyBlazorAsync();

        //     var sam = TeddyBlazorRepostiory.Get(TeddyBlazorId);
        //     students.Should().BeSameAs(TeddyBlazorRepostiory.GetList());
        //     sam.Name.Should().Be(TeddyBlazor.Name);
        // }

        // [Test]
        // public async Task ChangesTostudentsPersistAcrossStudentRepositoryInstances()
        // {
        //     var TeddyBlazorRepostiory2 = new StudentRepository(context2, configMoq.Object);
        //     var students = TeddyBlazorRepostiory.GetList();
        //     int TeddyBlazorId = 3;
        //     var TeddyBlazor = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = TeddyBlazorId,
        //         Name = "Sam"
        //     };
        //     TeddyBlazorRepostiory.Add(TeddyBlazor);

        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();
        //     await TeddyBlazorRepostiory2.InitializeTeddyBlazorAsync();

        //     TeddyBlazorRepostiory2.Get(TeddyBlazorId).Name.Should().Be("Sam");
        // }

        // [Test]
        // public async Task UpdatingSingleTeddyBlazorPersistAcrossStudentRepositoryInstances()
        // {
        //     var TeddyBlazorRepostiory2 = new StudentRepository(context, configMoq.Object);
        //     var students = TeddyBlazorRepostiory.GetList();
        //     int TeddyBlazorId = 3;
        //     var TeddyBlazor = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = TeddyBlazorId,
        //         Name = "Sam"
        //     };

        //     TeddyBlazorRepostiory.Add(TeddyBlazor);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     TeddyBlazor.Name = "richard";
        //     TeddyBlazorRepostiory2.Add(TeddyBlazor);
        //     await TeddyBlazorRepostiory2.UpdateDatabaseAsync();

        //     await TeddyBlazorRepostiory.InitializeTeddyBlazorAsync();

        //     TeddyBlazorRepostiory.Get(TeddyBlazorId).Name.Should().Be("richard");
        // }

        // [Test]
        // public async Task UpdatingTeddyBlazorRestrictionsPersists()
        // {
        //     var TeddyBlazorRepostiory2 = new StudentRepository(context, configMoq.Object);
        //     var students = TeddyBlazorRepostiory.GetList();
        //     var TeddyBlazor = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = 3,
        //         Name = "Sam"
        //     };
        //     var TeddyBlazor2 = new TeddyBlazorInfo(){
        //         TeddyBlazorInfoId = 4,
        //         Name = "bill"
        //     };
        //     TeddyBlazorRepostiory.Add(TeddyBlazor);
        //     TeddyBlazorRepostiory.Add(TeddyBlazor2);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     TeddyBlazor.Restrictions = new List<TeddyBlazorInfo>(){ TeddyBlazor2 };
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     await TeddyBlazorRepostiory2.InitializeTeddyBlazorAsync();
        //     TeddyBlazorRepostiory2.Get(3).Restrictions.Should().Contain(TeddyBlazor2);
        // }

        // [Test]
        // public async Task UpdatingTeddyBlazorNotesPersists()
        // {
        //     var TeddyBlazorRepostiory2 = new StudentRepository(context, configMoq.Object);
        //     var students = TeddyBlazorRepostiory.GetList();
        //     var TeddyBlazor = new TeddyBlazorInfo()
        //     {
        //         TeddyBlazorInfoId = 3,
        //         Name = "Sam"
        //     };
        //     var note = new Note()
        //     {
        //         NoteId = 5,
        //         Content = "sam's note"
        //     };
        //     TeddyBlazorRepostiory.Add(TeddyBlazor);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     await TeddyBlazorRepostiory.AddNoteAsync(TeddyBlazor, note);
        //     await TeddyBlazorRepostiory.UpdateDatabaseAsync();

        //     await TeddyBlazorRepostiory2.InitializeTeddyBlazorAsync();
        //     TeddyBlazorRepostiory2.Get(3).Notes.Should().Contain(note);
        // }
    }
}