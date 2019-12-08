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
    public class AssignmentRepositoryTest
    {
        private Func<NpgsqlConnection> getDbConnection;
        private Func<string> psqlString;
        private ClassRepository classRepository;
        private StudentRepository studentRepository;
        private CourseRepository courseRepository;
        private AssignmentRepository assignemtRepository;
        private ClassRoom scienceRoom;
        private Teacher jonathan;
        private Student sam;
        private Student ben;
        private Student tim;
        private ClassModel mathClass;
        private Course math1010;
        private Course science1010;
        private Assignment readChapterOne;
        private Assignment bookWork;

        [SetUp]
        public async Task Setup()
        {
            initializeObjects();
            await seedDatabase();

            readChapterOne = new Assignment()
            {
                CourseId = math1010.CourseId,
                AssignmentName = "read chapter one",
                AssignmentDescription = "read all of chapter one"
            };
            bookWork = new Assignment()
            {
                CourseId = math1010.CourseId,
                AssignmentName = "some bookwork",
                AssignmentDescription = "some tedious assignment"
            };
            await assignemtRepository.AddAssignmentAsync(readChapterOne);
            await assignemtRepository.AddAssignmentAsync(bookWork);
        }

        [Test]
        public async Task can_get_assignment()
        {
            var loadedReadChapterOne = await assignemtRepository.GetAssignmentAsync(readChapterOne.AssignmentId);

            loadedReadChapterOne.AssignmentDescription.Should().Be(readChapterOne.AssignmentDescription);
            loadedReadChapterOne.AssignmentName.Should().Be(readChapterOne.AssignmentName);
            loadedReadChapterOne.CourseId.Should().Be(readChapterOne.CourseId);
        }

        [Test]
        public async Task can_get_assignment_list_by_course()
        {
            var assignments = await assignemtRepository.GetAssignmentsAsync(math1010.CourseId);

            assignments.Count().Should().Be(2);
            assignments.First(a => a.AssignmentId == readChapterOne.AssignmentId)
                       .AssignmentDescription
                       .Should().Be(readChapterOne.AssignmentDescription);
            assignments.First(a => a.AssignmentId == bookWork.AssignmentId)
                       .AssignmentDescription
                       .Should().Be(bookWork.AssignmentDescription);
            assignments.First(a => a.AssignmentId == bookWork.AssignmentId)
                       .AssignmentName
                       .Should().Be(bookWork.AssignmentName);
        }

        [Test]
        public async Task can_update_assignment()
        {
            bookWork.AssignmentDescription = "an updated description";
            bookWork.AssignmentName = "an updated name";
            await assignemtRepository.UpdateAssignmentAsync(bookWork);

            var loadedBookwork = await assignemtRepository.GetAssignmentAsync(bookWork.AssignmentId);

            loadedBookwork.AssignmentDescription.Should().Be(bookWork.AssignmentDescription);
            loadedBookwork.AssignmentName.Should().Be(bookWork.AssignmentName);
            loadedBookwork.CourseId.Should().Be(bookWork.CourseId);
        }

        private void initializeObjects()
        {
            getDbConnection = () => new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;");
            var studentLoggerMoq = new Mock<ILogger<StudentRepository>>();
            var classLoggerMoq = new Mock<ILogger<ClassRepository>>();
            var courseLoggerMoq = new Mock<ILogger<CourseRepository>>();
            var assignmentLoggerMoq = new Mock<ILogger<AssignmentRepository>>();
            psqlString = () => string.Empty;
            classRepository = new ClassRepository(getDbConnection, classLoggerMoq.Object, psqlString);
            studentRepository = new StudentRepository(getDbConnection, studentLoggerMoq.Object);
            courseRepository = new CourseRepository(getDbConnection, courseLoggerMoq.Object);

            assignemtRepository = new AssignmentRepository(getDbConnection, assignmentLoggerMoq.Object);
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
            await classRepository.AddClassAsync(mathClass);
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
            await classRepository.EnrollStudentAsync(tim.StudentId, mathClass.ClassId, science1010.CourseId);
        }
    }
}