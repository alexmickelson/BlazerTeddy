
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Student.Data;
using Student.Models;
using Student.Services;

namespace Test.ServiceTests
{
    public class StudentRepostoryTests
    {
        private DbContextOptions<OurDbContext> dbOptions;
        private StudentRepository studentRepostiory;
        private OurDbContext context;
        private OurDbContext context2;

        [SetUp]
        public void Setup()
        {

            var dbOptions = new DbContextOptionsBuilder<OurDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentRepository")
                .EnableSensitiveDataLogging()
                .Options;
            context = new OurDbContext(dbOptions);
            context2 = new OurDbContext(dbOptions);
            studentRepostiory = new StudentRepository(context);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void EditingReferenceOfStudentListEditsRepostoryList()
        {
            var students = studentRepostiory.GetList();
            int studentId = 3;
            var student = new StudentInfo(){
                StudentInfoId = studentId,
                Name = "Sam"
            };

            students.Add(student);
            var sam = studentRepostiory.Get(studentId);


            students.Should().BeSameAs(studentRepostiory.GetList());
            sam.Name.Should().Be(student.Name);
        }

        [Test]
        public void AddingNotesOnStudentsPersistsToRepostitory()
        {
            var students = studentRepostiory.GetList();
            int studentId = 3;
            var student = new StudentInfo(){
                StudentInfoId = studentId,
                Name = "Sam"
            };

            students.Add(student);
            student.Notes = new List<Note>();
            student.Notes.Add(new Note(){Content = "this is a note"});

            var sam = studentRepostiory.Get(studentId);
            sam.Notes.First().Content.Should().Be("this is a note");
        }

        [Test]
        public async Task ChangesPersistAfterReloadingDatabase()
        {
            var students = studentRepostiory.GetList();
            int studentId = 3;
            var student = new StudentInfo(){
                StudentInfoId = studentId,
                Name = "Sam"
            };
            students.Add(student);

            await studentRepostiory.InitializeStudentAsync();

            var sam = studentRepostiory.Get(studentId);
            students.Should().BeSameAs(studentRepostiory.GetList());
            sam.Name.Should().Be(student.Name);
        }

        [Test]
        public async Task ChangesToStudentsPersistAcrossStudentRepositoryInstances()
        {
            var studentRepostiory2 = new StudentRepository(context2);
            var students = studentRepostiory.GetList();
            int studentId = 3;
            var student = new StudentInfo(){
                StudentInfoId = studentId,
                Name = "Sam"
            };
            studentRepostiory.Add(student);

            await studentRepostiory.UpdateDatabaseAsync();
            await studentRepostiory2.InitializeStudentAsync();

            studentRepostiory2.Get(studentId).Name.Should().Be("Sam");
        }

        [Test]
        public async Task UpdatingSingleStudentPersistAcrossStudentRepositoryInstances()
        {
            var studentRepostiory2 = new StudentRepository(context);
            var students = studentRepostiory.GetList();
            int studentId = 3;
            var student = new StudentInfo(){
                StudentInfoId = studentId,
                Name = "Sam"
            };

            studentRepostiory.Add(student);
            await studentRepostiory.UpdateDatabaseAsync();
            
            student.Name = "richard";
            studentRepostiory2.Add(student);
            await studentRepostiory2.UpdateDatabaseAsync();

            await studentRepostiory.InitializeStudentAsync();

            studentRepostiory.Get(studentId).Name.Should().Be("richard");
        }

        [Test]
        public async Task UpdatingStudentRestrictionsPersists()
        {
            var studentRepostiory2 = new StudentRepository(context);
            var students = studentRepostiory.GetList();
            var student = new StudentInfo(){
                StudentInfoId = 3,
                Name = "Sam"
            };
            var student2 = new StudentInfo(){
                StudentInfoId = 4,
                Name = "bill"
            };
            studentRepostiory.Add(student);
            studentRepostiory.Add(student2);
            await studentRepostiory.UpdateDatabaseAsync();

            student.Restrictions = new List<StudentInfo>(){ student2 };
            await studentRepostiory.UpdateDatabaseAsync();

            await studentRepostiory2.InitializeStudentAsync();
            studentRepostiory2.Get(3).Restrictions.Should().Contain(student2);
        }

        [Test]
        public async Task UpdatingStudentNotesPersists()
        {
            var studentRepostiory2 = new StudentRepository(context);
            var students = studentRepostiory.GetList();
            var student = new StudentInfo()
            {
                StudentInfoId = 3,
                Name = "Sam"
            };
            var note = new Note()
            {
                NoteId = 5,
                Content = "sam's note"
            };
            studentRepostiory.Add(student);
            await studentRepostiory.UpdateDatabaseAsync();

            await studentRepostiory.AddNoteAsync(student, note);
            await studentRepostiory.UpdateDatabaseAsync();

            await studentRepostiory2.InitializeStudentAsync();
            studentRepostiory2.Get(3).Notes.Should().Contain(note);
        }
    }
}