using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Student.Models;
using Student.Services;
using Student.ViewModels;
using System.Linq;
using FluentAssertions;

namespace Test.ViewModelTests
{
    public class StudentListViewModelTests
    {
        private Mock<IStudentRepository> studentRepoMoq;
        private StudentListViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            studentRepoMoq = new Mock<IStudentRepository>();
            viewModel = new StudentListViewModel(studentRepoMoq.Object);
        }

        [Test]
        public void FilterStudentListByName()
        {
            var studentlist = new List<StudentInfo>()
            {
                new StudentInfo(){ Name="adam"},
                new StudentInfo(){ Name="benny"},
                new StudentInfo(){ Name="spencer"}
            };
            studentRepoMoq.Setup(sr => sr.GetList()).Returns(studentlist);

            viewModel.NameFilter = "a";
            viewModel.CourseIdFilter = -1;

            var actual = viewModel.GetFilteredStudents();
            actual.Count().Should().Be(1);
            actual.First().Name.Should().Be("adam");
        }

        [Test]
        public void CanFilterStudentsByCourse()
        {
            var math = new Course(){ CourseId = 1, Name = "math"};
            var science = new Course(){ CourseId = 2, Name = "science"};
            var relationship1 = new StudentCourseRelationship(){CourseId = math.CourseId, StudentInfoId = 1};
            var relationship2 = new StudentCourseRelationship(){CourseId = math.CourseId, StudentInfoId = 2};
            var relationship3 = new StudentCourseRelationship(){CourseId = science.CourseId, StudentInfoId = 2};
            var relationship4 = new StudentCourseRelationship(){CourseId = science.CourseId, StudentInfoId = 3};
            var student1 = new StudentInfo(){ StudentInfoId=1, Name="adam", StudentCourses = new [] {relationship1}};
            var student2 = new StudentInfo(){ StudentInfoId=2, Name="benny", StudentCourses = new [] {relationship2, relationship3}};
            var student3 = new StudentInfo(){ StudentInfoId=3, Name="spencer", StudentCourses = new [] {relationship4}};
            var studentlist = new List<StudentInfo>()
                { student1, student2, student3};

            studentRepoMoq.Setup(sr => sr.GetList()).Returns(studentlist);

            viewModel.NameFilter = "";
            viewModel.CourseIdFilter = math.CourseId;

            var actual = viewModel.GetFilteredStudents();
            actual.Count().Should().Be(2);
            actual.First().Name.Should().Be("adam");
            actual.Last().Name.Should().Be("benny");

        }

        [Test]
        public void NoFiltersSetReturnsAllStudents()
        {
            var math = new Course(){ CourseId = 1, Name = "math"};
            var science = new Course(){ CourseId = 2, Name = "science"};
            var relationship1 = new StudentCourseRelationship(){CourseId = math.CourseId, StudentInfoId = 1};
            var relationship2 = new StudentCourseRelationship(){CourseId = math.CourseId, StudentInfoId = 2};
            var relationship3 = new StudentCourseRelationship(){CourseId = science.CourseId, StudentInfoId = 2};
            var relationship4 = new StudentCourseRelationship(){CourseId = science.CourseId, StudentInfoId = 3};
            var student1 = new StudentInfo(){ StudentInfoId=1, Name="adam", StudentCourses = new [] {relationship1}};
            var student2 = new StudentInfo(){ StudentInfoId=2, Name="benny", StudentCourses = new [] {relationship2, relationship3}};
            var student3 = new StudentInfo(){ StudentInfoId=3, Name="spencer", StudentCourses = new [] {relationship4}};
            var studentlist = new List<StudentInfo>()
                { student1, student2, student3};

            studentRepoMoq.Setup(sr => sr.GetList()).Returns(studentlist);
            viewModel.CourseIdFilter = -1;

            var actual = viewModel.GetFilteredStudents();
            actual.Should().Contain(student1);
            actual.Should().Contain(student2);
            actual.Should().Contain(student3);
        }
    }
}