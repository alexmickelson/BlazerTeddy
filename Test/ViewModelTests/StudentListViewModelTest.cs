using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TeddyBlazor.Models;
using TeddyBlazor.Services;
using TeddyBlazor.ViewModels;
using System.Linq;
using FluentAssertions;

namespace Test.ViewModelTests
{
    public class StudentListViewModelTests
    {
        private Mock<IStudentRepository> TeddyBlazorRepoMoq;
        private StudentListViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            TeddyBlazorRepoMoq = new Mock<IStudentRepository>();
            viewModel = new StudentListViewModel(TeddyBlazorRepoMoq.Object);
        }

        [Test]
        public void FilterTeddyBlazorListByName()
        {
            var TeddyBlazorlist = new List<Student>()
            {
                new Student(){ Name="adam"},
                new Student(){ Name="benny"},
                new Student(){ Name="spencer"}
            };
            TeddyBlazorRepoMoq.Setup(sr => sr.GetList()).Returns(TeddyBlazorlist);

            viewModel.NameFilter = "a";

            var actual = viewModel.GetFilteredStudents();
            actual.Count().Should().Be(1);
            actual.First().Name.Should().Be("adam");
        }

        [Test]
        public void CanFilterstudentsByCourse()
        {
            var math = new Course(){ CourseId = 1, Name = "math"};
            var science = new Course(){ CourseId = 2, Name = "science"};
            var TeddyBlazorlist = new List<Student>()
            {
                new Student(){ Name="adam", Courses = new []{math}},
                new Student(){ Name="benny", Courses = new []{math, science}},
                new Student(){ Name="spencer", Courses = new []{science}}
            };
            TeddyBlazorRepoMoq.Setup(sr => sr.GetList()).Returns(TeddyBlazorlist);

            viewModel.NameFilter = "";
            viewModel.ClassIdFilter = math.CourseId;

            var actual = viewModel.GetFilteredStudents();
            actual.Count().Should().Be(2);
            actual.First().Name.Should().Be("adam");
            actual.Last().Name.Should().Be("benny");

        }

        [Test]
        public void NoFiltersSetReturnsAllstudents()
        {
            var math = new Course(){ CourseId = 1, Name = "math"};
            var science = new Course(){ CourseId = 2, Name = "science"};
            var student1 = new Student(){ Name="adam", Courses = new []{math}};
            var student2 = new Student(){ Name="benny", Courses = new []{math, science}};
            var student3 = new Student(){ Name="spencer", Courses = new []{science}};

            var studentList = new List<Student>()
                { student1, student2, student3 };
            TeddyBlazorRepoMoq.Setup(sr => sr.GetList()).Returns(studentList);

            var actual = viewModel.GetFilteredStudents();
            actual.Should().Contain(student1);
            actual.Should().Contain(student2);
            actual.Should().Contain(student3);
        }
    }
}