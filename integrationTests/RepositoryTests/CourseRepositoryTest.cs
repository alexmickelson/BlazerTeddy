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
        private Course science1010;

        [SetUp]
        public async Task Setup()
        {
            initializeObjects();
            await seedDatabase();
        }

        [Test]
        public async Task can_get_course_from_database()
        {
            var storedMathCourse = await courseRepository.GetCourseAsync(math1010.CourseId);

            storedMathCourse.CourseId.Should().NotBe(default(int), "storing the course assigns it an id");
            storedMathCourse.CourseName.Should().Be(math1010.CourseName, "the course is called math 1010");
            storedMathCourse.TeacherId.Should().Be(jonathan.TeacherId, "Jonathan is thae math teacher");
            storedMathCourse.StudentIds.Count().Should().Be(1, "Only Sam is in math 1010");
            storedMathCourse.StudentIds.Should().Contain(sam.StudentId);
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

        [Test]
        public async Task can_get_courses_in_a_class()
        {
            var courses = await courseRepository.GetCoursesByClassId(mathClass.ClassId);

            courses.Count().Should().Be(2);
            courses.Select(c => c.CourseId).Should().Contain(math1010.CourseId);
            courses.Select(c => c.CourseName).Should().Contain(math1010.CourseName);
            courses.Select(c => c.CourseId).Should().Contain(science1010.CourseId);
            courses.Select(c => c.CourseName).Should().Contain(science1010.CourseName);
        }

        [Test]
        public async Task can_get_courses_in_a_class_without_duplicates()
        {
            await classRepository.EnrollStudentAsync(tim.StudentId, mathClass.ClassId, math1010.CourseId);
            var courses = await courseRepository.GetCoursesByClassId(mathClass.ClassId);

            courses.Count().Should().Be(2);
            courses.Select(c => c.CourseId).Should().Contain(math1010.CourseId);
            courses.Select(c => c.CourseName).Should().Contain(math1010.CourseName);
            courses.Select(c => c.CourseId).Should().Contain(science1010.CourseId);
            courses.Select(c => c.CourseName).Should().Contain(science1010.CourseName);
        }

        [Test]
        public async Task can_get_student_ids_with_course()
        {
            var loadedMath1010 = await courseRepository.GetCourseAsync(math1010.CourseId);

            loadedMath1010.StudentIds.Count().Should().Be(1);
            loadedMath1010.StudentIds.Should().Contain(sam.StudentId);
        }

        [Test]
        public async Task can_get_courses_by_teacher_id()
        {
            var jonathanCourses = await courseRepository.GetCoursesByTeacherId(jonathan.TeacherId);

            jonathanCourses.Count().Should().Be(2);
            jonathanCourses.First(c => c.CourseId == math1010.CourseId)
                           .CourseName
                           .Should().Be(math1010.CourseName);
            jonathanCourses.First(c => c.CourseId == science1010.CourseId)
                           .CourseName
                           .Should().Be(science1010.CourseName);
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
                ClassRoomId = scienceRoom.ClassRoomId
            };
            scienceClass = new ClassModel()
            {
                ClassName = "science",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId
            };
            notScienceClass = new ClassModel()
            {
                ClassName = "not science",
                TeacherId = heber.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId
            };
            await classRepository.AddClassAsync(mathClass);
            await classRepository.AddClassAsync(scienceClass);
            await classRepository.AddClassAsync(notScienceClass);

            math1010 = new Course()
            {
                CourseName = "math 1010",
                TeacherId = jonathan.TeacherId
            };
            science1010 = new Course()
            {
                CourseName = "science 1010",
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