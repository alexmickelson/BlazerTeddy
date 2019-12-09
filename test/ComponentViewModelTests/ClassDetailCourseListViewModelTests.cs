using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TeddyBlazor.Models;
using TeddyBlazor.Services;
using TeddyBlazor.ViewModels.ClassDetail;

namespace Test.ComponentViewModelTests
{
    public class ClassDetailCourseListViewModelTests
    {
        private Mock<ICourseRepository> courseRepoMoq;
        private Mock<IStudentRepository> studentRepoMoq;
        private Mock<IClassRepository> classRepoMoq;
        private ClassDetailCourseListViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            courseRepoMoq = new Mock<ICourseRepository>();
            studentRepoMoq = new Mock<IStudentRepository>();
            classRepoMoq = new Mock<IClassRepository>();
            viewModel = new ClassDetailCourseListViewModel(courseRepoMoq.Object,
                                                           studentRepoMoq.Object,
                                                           classRepoMoq.Object);
        }

        [Test]
        public void GetStudentNamesInCourse_filters_by_CourseId()
        {
            viewModel.Students = new Student[]
            {
                new Student()
                {
                    StudentId = 1,
                    StudentName = "sam"
                },
                new Student()
                {
                    StudentId = 2,
                    StudentName = "tim"
                }
            };
            viewModel.Courses = new Course[]
            {
                new Course()
                {
                    CourseId = 1,
                    CourseName = "math 1010",
                    StudentIds = new int[] { 1 }
                },
                new Course()
                {
                    CourseId = 2,
                    CourseName = "science 1010",
                    StudentIds = new int[] { 2 }
                }
            };

            var math1010Students = viewModel.GetStudentNamesByCourse(1);
            
            math1010Students.Count().Should().Be(1);
            math1010Students.Should().Contain("sam");
        }
    }
}