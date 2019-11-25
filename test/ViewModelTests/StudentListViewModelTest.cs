using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TeddyBlazor.Models;
using TeddyBlazor.Services;
using TeddyBlazor.ViewModels;
using System.Linq;
using FluentAssertions;
using System.Threading.Tasks;

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
                new Student(){ StudentName="adam"},
                new Student(){ StudentName="benny"},
                new Student(){ StudentName="spencer"}
            };
            TeddyBlazorRepoMoq.Setup(sr => sr.GetList()).Returns(TeddyBlazorlist);

            viewModel.NameFilter = "a";

            var actual = viewModel.GetFilteredStudents();
            actual.Count().Should().Be(1);
            actual.First().StudentName.Should().Be("adam");
        }

        [Test]
        public void NoFiltersSetReturnsAllstudents()
        {
            var math = new Course(){ CourseId = 1, Name = "math"};
            var science = new Course(){ CourseId = 2, Name = "science"};
            var student1 = new Student(){ StudentName="adam"};
            var student2 = new Student(){ StudentName="benny"};
            var student3 = new Student(){ StudentName="spencer"};

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