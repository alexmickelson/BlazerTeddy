
using System.Collections.Generic;
using System.Linq;
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

        [SetUp]
        public void Setup()
        {

            var dbOptions = new DbContextOptionsBuilder<OurDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentRepository")
                .Options;
            context = new OurDbContext(dbOptions);
            studentRepostiory = new StudentRepository(context);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void EditingReferenceOfStudentListEditsRepostoryList()
        {
            var students = studentRepostiory.GetStudents();
            int studentId = 3;
            var student = new StudentInfo(){
                StudentInfoId = studentId,
                Name = "Sam"
            };

            students.Add(student);
            var sam = studentRepostiory.Get(studentId);


            students.Should().BeSameAs(studentRepostiory.GetStudents());
            sam.Name.Should().Be(student.Name);
        }
    }
}