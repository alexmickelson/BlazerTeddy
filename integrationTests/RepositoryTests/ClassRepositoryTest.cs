using NUnit.Framework;
using TeddyBlazor.Services;
using Npgsql;
using System.Runtime.InteropServices;
using System;
using TeddyBlazor.Models;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;

namespace IntegrationTests.RepositoryTests
{
    public class ClassRepositoryTest
    {
        private Func<NpgsqlConnection> getDbConnection;
        private Mock<ILogger<StudentRepository>> studentLoggerMoq;
        private Mock<ILogger<ClassRepository>> classLoggerMoq;
        private Mock<ILogger<CourseRepository>> courseLoggerMoq;
        private Func<string> psqlString;
        private IClassRepository classRepository;
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
        }

        private void initializeObjects()
        {
            getDbConnection = () => new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;");
            studentLoggerMoq = new Mock<ILogger<StudentRepository>>();
            classLoggerMoq = new Mock<ILogger<ClassRepository>>();
            courseLoggerMoq = new Mock<ILogger<CourseRepository>>();
            psqlString = () => string.Empty;
            studentRepository = new StudentRepository(getDbConnection, studentLoggerMoq.Object);
            courseRepository = new CourseRepository(getDbConnection, courseLoggerMoq.Object);
            classRepository = new ClassRepository(getDbConnection, classLoggerMoq.Object, psqlString);
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
                CourseName = "Math 1010",
                TeacherId = jonathan.TeacherId
            };
            await courseRepository.AddCourseAsync(math1010);

            await classRepository.EnrollStudentAsync(sam.StudentId, mathClass.ClassId, math1010.CourseId);
            await classRepository.EnrollStudentAsync(sam.StudentId, scienceClass.ClassId, math1010.CourseId);
            await classRepository.EnrollStudentAsync(sam.StudentId, notScienceClass.ClassId, math1010.CourseId);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task can_add_classroom()
        {
            scienceRoom.ClassRoomId.Should().NotBe(default(int));
            var newClassRoom = await classRepository.GetClassRoomAsync(scienceRoom.ClassRoomId);
            newClassRoom.ClassRoomName.Should().Be(scienceRoom.ClassRoomName);
            newClassRoom.SeatsHorizontally.Should().Be(scienceRoom.SeatsHorizontally);
            newClassRoom.SeatsVertically.Should().Be(scienceRoom.SeatsVertically);
        }
        
        [Test]
        public async Task can_add_teacher()
        {
            jonathan.TeacherId.Should().NotBe(default(int));
            var newTeacher = await classRepository.GetTeacherAsync(jonathan.TeacherId);
            newTeacher.TeacherName.Should().Be(jonathan.TeacherName);
        }

        [Test]
        public async Task can_get_class()
        {
            var newClass = await classRepository.GetClassAsync(mathClass.ClassId);

            newClass.Should().NotBeNull();
            newClass.ClassName.Should().Be(mathClass.ClassName);
            newClass.TeacherId.Should().Be(jonathan.TeacherId);
            newClass.ClassRoomId.Should().Be(scienceRoom.ClassRoomId);
        }

        [Test]
        public async Task invalid_seating_chart_thows_exception()
        {
            mathClass.SeatingChartByStudentID = new int[3,3];
            mathClass.SeatingChartByStudentID[1,1] = -30;

            await classRepository
                .Invoking(async cr => await cr.AddClassAsync(mathClass))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("cannot assign seat, no student with id -30");
        }

        [Test]
        public async Task can_save_seating_chart()
        {
            var mathClass = new ClassModel()
            {
                ClassName = "math",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId
            };

            mathClass.SeatingChartByStudentID = new int[3,3];
            mathClass.SeatingChartByStudentID[1,1] = sam.StudentId;
            mathClass.SeatingChartByStudentID[2,2] = ben.StudentId;
            mathClass.SeatingChartByStudentID[2,0] = tim.StudentId;

            await classRepository.AddClassAsync(mathClass);

            var newClass = await classRepository.GetClassAsync(mathClass.ClassId);

            newClass.SeatingChartByStudentID[1,1].Should().Be(sam.StudentId);
            newClass.SeatingChartByStudentID[2,2].Should().Be(ben.StudentId);
            newClass.SeatingChartByStudentID[2,0].Should().Be(tim.StudentId);
            newClass.SeatingChartByStudentID[0,0].Should().Be(default(int));
        }

        [Test]
        public async Task seating_chart_must_be_valid_dimensions()
        {
            var mathClass = new ClassModel()
            {
                ClassName = "math",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId
            };
            var horizontal = scienceRoom.SeatsHorizontally;
            var vertical = scienceRoom.SeatsVertically;
            mathClass.SeatingChartByStudentID = new int[horizontal + 1, vertical + 1];
            mathClass.SeatingChartByStudentID[1,1] = sam.StudentId;

            await classRepository
                .Invoking(async cr => await cr.AddClassAsync(mathClass))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage(
                    $"cannot save seating chart {horizontal + 1},{vertical + 1} in classroom with dimensions {horizontal},{vertical}");
        }

        [Test]
        public async Task can_update_classroom()
        {
            var scienceRoom = new ClassRoom()
            { 
                ClassRoomName = "Science Room",
                SeatsHorizontally = 4,
                SeatsVertically = 5
            };
            await classRepository.AddClassRoomAsync(scienceRoom);

            scienceRoom.ClassRoomName = "not the science room";

            await classRepository.UpdateClassRoomAsync(scienceRoom);

            scienceRoom.ClassRoomId.Should().NotBe(default(int));
            var newClassRoom = await classRepository.GetClassRoomAsync(scienceRoom.ClassRoomId);
            newClassRoom.ClassRoomName.Should().Be(scienceRoom.ClassRoomName);
            newClassRoom.SeatsHorizontally.Should().Be(scienceRoom.SeatsHorizontally);
            newClassRoom.SeatsVertically.Should().Be(scienceRoom.SeatsVertically);
            
        }
        
        [Test]
        public async Task can_update_teacher()
        {
            var jonathan = new Teacher(){ TeacherName = "jonathan" };
            await classRepository.AddTeacherAsync(jonathan);

            jonathan.TeacherName = "heber";
            await classRepository.UpdateTeacherAsync(jonathan);

            var newTeacher = await classRepository.GetTeacherAsync(jonathan.TeacherId);
            newTeacher.TeacherName.Should().Be(jonathan.TeacherName);
        }

        [Test]
        public async Task can_update_class()
        {
            var mathClass = new ClassModel()
            {
                ClassName = "math",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId
            };
            mathClass.SeatingChartByStudentID = new int[3,3];
            mathClass.SeatingChartByStudentID[1,1] = sam.StudentId;
            await classRepository.AddClassAsync(mathClass);

            mathClass.SeatingChartByStudentID[0,0] = sam.StudentId;
            mathClass.SeatingChartByStudentID[1,1] = default(int);

            await classRepository.UpdateClassAsync(mathClass);

            var newClass = classRepository.GetClassAsync(mathClass.ClassId);

            mathClass.SeatingChartByStudentID[0,0].Should().Be(sam.StudentId);
            mathClass.SeatingChartByStudentID[1,1].Should().Be(default(int));
        }

        [Test]
        public async Task can_get_classes_by_teacher_id()
        {
            var classList = await classRepository.GetClassesByTeacherId(jonathan.TeacherId);

            classList.Count().Should().Be(2);

            classList.Where(c => c.ClassId == mathClass.ClassId).Count().Should().Be(1);
            classList.Where(c => c.ClassId == scienceClass.ClassId).Count().Should().Be(1);
            classList.Where(c => c.ClassId == notScienceClass.ClassId).Count().Should().Be(0);

            classList.Where(c => c.ClassId == mathClass.ClassId)
                     .First()
                     .Enrollments.Select(e => e.StudentId)
                     .Should().Contain(sam.StudentId, "sam is enrolled in the mathclass");
            classList.Where(c => c.ClassId == scienceClass.ClassId)
                     .First()
                     .Enrollments.Select(e => e.StudentId)
                     .Should().Contain(sam.StudentId, "sam is enrolled in the scienceclass");
        }

    }
}