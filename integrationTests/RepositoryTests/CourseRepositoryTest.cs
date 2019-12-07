using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using NUnit.Framework;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace IntegrationTests.RepositoryTests
{
    public class CourseRepositoryTests
    {
        private System.Func<NpgsqlConnection> getDbConnection;
        private Mock<ILogger<StudentRepository>> studentLoggerMoq;
        private Mock<ILogger<ClassRepository>> classLoggerMoq;
        private Mock<ILogger<CourseRepository>> courseLoggerMoq;
        private Func<string> psqlString;
        private ClassRepository classRepository;
        private StudentRepository studentRepository;
        private CourseRepository courseRepository;
        private ClassRoom scienceRoom;
        private Teacher jonathan;
        private Teacher heber;
        private Student sam;
        private Student ben;
        private Student tim;
        private ClassModel mathClass;
        private ClassModel scienceClass;
        private ClassModel notScienceClass;
        private Course math1010;

        [SetUp]
        public async Task Setup()
        {
            initializeObjects();
            await seedDatabase();

            math1010 = new Course()
            {
                CourseName = "math 1010",
                TeacherId = jonathan.TeacherId
            };
            await courseRepository.AddCourseAsync(math1010);
            await classRepository.EnrollStudentAsync(sam.StudentId, mathClass.ClassId, math1010.CourseId);
        }

        [Test]
        public async Task can_get_course_from_database()
        {
            var storedMathCourse = await courseRepository.GetCourseAsync(math1010.CourseId);
            storedMathCourse.CourseId.Should().NotBe(default(int), "storing the course assigns it an id");
            storedMathCourse.CourseName.Should().Be(math1010.CourseName, "the course is called math 1010");
            storedMathCourse.TeacherId.Should().Be(jonathan.TeacherId, "Jonathan is thae math teacher");
        }

        [Test]
        public async Task invalid_teacher_id_throws_error()
        {
            var unsavedTeacher = new Teacher() { TeacherId = -30 };

            var invalidCourse = new Course()
            {
                CourseName = "shouldn't get inserted",
                TeacherId = unsavedTeacher.TeacherId
            };

            await courseRepository
                .Invoking(async cr => await cr.AddCourseAsync(invalidCourse))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"cannot save course with invalid teacherId: {unsavedTeacher.TeacherId}");
        }

        [Test]
        public async Task can_get_course_from_by_class_and_student_id()
        {
            var storedMathCourse = await courseRepository.GetCourseAsync(sam.StudentId, mathClass.ClassId);
            storedMathCourse.CourseId.Should().NotBe(default(int), "storing the course assigns it an id");
            storedMathCourse.CourseName.Should().Be(math1010.CourseName, "the course is called math 1010");
            storedMathCourse.TeacherId.Should().Be(jonathan.TeacherId, "Jonathan is thae math teacher");
        }

        private void initializeObjects()
        {
            getDbConnection = () => new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;");
            studentLoggerMoq = new Mock<ILogger<StudentRepository>>();
            classLoggerMoq = new Mock<ILogger<ClassRepository>>();
            courseLoggerMoq = new Mock<ILogger<CourseRepository>>();
            psqlString = () => string.Empty;
            classRepository = new ClassRepository(getDbConnection, classLoggerMoq.Object, psqlString);
            studentRepository = new StudentRepository(getDbConnection, studentLoggerMoq.Object);

            courseRepository = new CourseRepository(getDbConnection, courseLoggerMoq.Object);
        }

        private async Task seedDatabase()
        {
            scienceRoom = new ClassRoom()
            {
                ClassRoomName = "Science Room",
                SeatsHorizontally = 10,
                SeatsVertically = 8
            };
            scienceRoom = new ClassRoom()
            {
                ClassRoomName = "Science Room",
                SeatsHorizontally = 10,
                SeatsVertically = 8
            };
            jonathan = new Teacher() { TeacherName = "jonathan" };
            heber = new Teacher() { TeacherName = "not jonathan" };
            sam = new Student() { StudentName = "sam" };
            ben = new Student() { StudentName = "ben" };
            tim = new Student() { StudentName = "tim" };
            await classRepository.AddClassRoomAsync(scienceRoom);
            await classRepository.AddTeacherAsync(jonathan);
            await classRepository.AddTeacherAsync(heber);
            await studentRepository.AddStudentAsync(sam);
            await studentRepository.AddStudentAsync(ben);
            await studentRepository.AddStudentAsync(tim);
            mathClass = new ClassModel()
            {
                ClassName = "math",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId,
                // StudentIds = new int[]{ sam.StudentId }
            };
            scienceClass = new ClassModel()
            {
                ClassName = "science",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId,
                // StudentIds = new int[]{ sam.StudentId }
            };
            notScienceClass = new ClassModel()
            {
                ClassName = "not science",
                TeacherId = heber.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId,
                // StudentIds = new int[]{ sam.StudentId }
            };
            await classRepository.AddClassAsync(mathClass);
            await classRepository.AddClassAsync(scienceClass);
            await classRepository.AddClassAsync(notScienceClass);
        }

    }
}