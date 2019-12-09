using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TeddyBlazor.Models;
using TeddyBlazor.Services;
using TeddyBlazor.ViewModels;
using System.Linq;
using FluentAssertions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Test.ViewModelTests
{
    public class StudentListViewModelTests
    {
        private Mock<IStudentRepository> StudentRepoMoq;
        private Mock<IClassRepository> ClassRepoMoq;
        private Mock<IRefreshService> RefreshServiceMoq;
        private StudentListViewModel viewModel;
        private Course math;
        private Course science;
        private Student adam;
        private Student benny;
        private Student spencer;
        private ClassModel mondayAfternoon;

        [SetUp]
        public void SetUp()
        {
            StudentRepoMoq = new Mock<IStudentRepository>();
            ClassRepoMoq = new Mock<IClassRepository>();
            RefreshServiceMoq = new Mock<IRefreshService>();
            var LoggerMoq = new Mock<ILogger<StudentListViewModel>>();
            viewModel = new StudentListViewModel(StudentRepoMoq.Object,
                                                 ClassRepoMoq.Object,
                                                 RefreshServiceMoq.Object,
                                                 LoggerMoq.Object);


            math = new Course(){ CourseId = 1, CourseName = "math"};
            science = new Course(){ CourseId = 2, CourseName = "science"};
            adam = new Student(){ StudentName="adam", StudentId = 1 };
            benny = new Student(){ StudentName="benny", StudentId = 2 };
            spencer = new Student(){ StudentName="spencer", StudentId = 3 };

            mondayAfternoon = new ClassModel()
            {
                ClassId = 1,
                ClassName = "monday afternoon's class"
            };

            var studentList = new List<Student>()
                { adam, benny, spencer };
            var classList = new ClassModel[] { mondayAfternoon };
            ClassRepoMoq.Setup(cr => cr.GetAllClassesAsync()).ReturnsAsync(classList);
            StudentRepoMoq.Setup(sr => sr.GetListAsync()).ReturnsAsync(studentList);
            StudentRepoMoq.Setup(sr => sr.GetStudentsByClassAsync(mondayAfternoon.ClassId))
                          .ReturnsAsync(new Student[] { adam, spencer });
        }

        [Test]
        public void filter_teddy_blazor_list_by_name()
        {
            viewModel.NameFilter = "a";

            var actual = viewModel.GetFilteredStudents();
            actual.Count().Should().Be(1);
            actual.First().StudentName.Should().Be(adam.StudentName);
        }

        [Test]
        public void no_filters_set_returns_all_students()
        {
            var actual = viewModel.GetFilteredStudents();

            actual.Should().Contain(adam);
            actual.Should().Contain(benny);
            actual.Should().Contain(spencer);
        }

        [Test]
        public void can_filter_by_class()
        {
            viewModel.ClassIdFilter = mondayAfternoon.ClassId;

            var filteredStudents = viewModel.GetFilteredStudents();

            filteredStudents.Count().Should().Be(2);
            filteredStudents.Should().Contain(adam);
            filteredStudents.Should().Contain(spencer);
        }
    }
}