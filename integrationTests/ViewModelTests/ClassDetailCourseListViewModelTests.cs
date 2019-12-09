using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using NUnit.Framework;
using TeddyBlazor.Models;
using TeddyBlazor.Services;
using TeddyBlazor.ViewModels.ClassDetail;

namespace IntegrationTests.ViewModelTests
{
    public class ClassDetailCourseListViewModelTests
    {
        private System.Func<NpgsqlConnection> getDbConnection;
        private Mock<ILogger<StudentRepository>> studentLoggerMoq;
        private Mock<ILogger<ClassRepository>> classLoggerMoq;
        private Mock<ILogger<CourseRepository>> courseLoggerMoq;
        private Func<string> psqlString;
        private ClassRepository classRepository;
        private StudentRepository studentRepository;
        private CourseRepository courseRepository;
        private ClassDetailCourseListViewModel viewModel;
        private ClassRoom scienceRoom;
        private Teacher jonathan;
        private Teacher heber;
        private Student sam;
        private Student ben;
        private Student tim;
        private ClassModel mathClass;
        private ClassModel scienceClass;
        private Course math1010;
        private Course science1010;

        [SetUp]
        public async Task Setup()
        {
            initializeObjects();
            await seedDatabase();

        }
        
        [Test]
        public async Task courses_initialize_correcly()
        {
            viewModel.ClassId = mathClass.ClassId;
            await viewModel.OnParametersSetAsync();

            viewModel.Courses.Count().Should().Be(2);
            viewModel.Courses
                .First(c => c.CourseId == math1010.CourseId)
                .StudentIds
                .Count()
                .Should().Be(1);
            viewModel.Courses
                .First(c => c.CourseId == math1010.CourseId)
                .StudentIds
                .Should().Contain(sam.StudentId);
        }
        
        [Test]
        public async Task GetStudentNamesInCourse_filters_by_CourseId()
        {
            viewModel.ClassId = mathClass.ClassId;
            await viewModel.OnParametersSetAsync();

            var studentNames = viewModel.GetStudentNamesByCourse(math1010.CourseId);

            studentNames.Count().Should().Be(1);
            studentNames.Should().Contain(sam.StudentName);
        }

        private void initializeObjects()
        {
            getDbConnection = () => new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;");
            Func<string> getString = () => string.Empty;
            studentLoggerMoq = new Mock<ILogger<StudentRepository>>();
            classLoggerMoq = new Mock<ILogger<ClassRepository>>();
            courseLoggerMoq = new Mock<ILogger<CourseRepository>>();
            classLoggerMoq = new Mock<ILogger<ClassRepository>>();
            psqlString = () => string.Empty;
            classRepository = new ClassRepository(getDbConnection, classLoggerMoq.Object, psqlString);
            studentRepository = new StudentRepository(getDbConnection, studentLoggerMoq.Object);
            courseRepository = new CourseRepository(getDbConnection, courseLoggerMoq.Object);
            classRepository = new ClassRepository(getDbConnection, classLoggerMoq.Object, getString);
            viewModel = new ClassDetailCourseListViewModel(courseRepository, studentRepository, classRepository);
        }


        private async Task seedDatabase()
        {
            scienceRoom = new ClassRoom()
            {
                ClassRoomName = "Science Room",
                SeatsHorizontally = 10,
                SeatsVertically = 8
            };
            jonathan = new Teacher() { TeacherName = "jonathan" };
            sam = new Student() { StudentName = "sam" };
            ben = new Student() { StudentName = "ben" };
            tim = new Student() { StudentName = "tim" };
            await classRepository.AddClassRoomAsync(scienceRoom);
            await classRepository.AddTeacherAsync(jonathan);
            await studentRepository.AddStudentAsync(sam);
            await studentRepository.AddStudentAsync(ben);
            await studentRepository.AddStudentAsync(tim);
            mathClass = new ClassModel()
            {
                ClassName = "math",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId
            };
            scienceClass = new ClassModel()
            {
                ClassName = "science",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId
            };
            await classRepository.AddClassAsync(mathClass);
            await classRepository.AddClassAsync(scienceClass);

            math1010 = new Course()
            {
                CourseName = "math 1010",
                TeacherId = jonathan.TeacherId
            };
            science1010 = new Course()
            {
                CourseName = "math 1010",
                TeacherId = jonathan.TeacherId
            };
            await courseRepository.AddCourseAsync(math1010);
            await courseRepository.AddCourseAsync(science1010);
            await classRepository.EnrollStudentAsync(sam.StudentId, mathClass.ClassId, math1010.CourseId);
            await classRepository.EnrollStudentAsync(ben.StudentId, mathClass.ClassId, science1010.CourseId);
            await classRepository.EnrollStudentAsync(tim.StudentId, scienceClass.ClassId, science1010.CourseId);
        }
    }
}