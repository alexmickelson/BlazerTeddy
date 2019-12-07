using NUnit.Framework;
using TeddyBlazor.Services;
using Npgsql;
using Docker.DotNet;
using System.Runtime.InteropServices;
using System;
using TeddyBlazor.Models;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;

namespace IntegrationTests.RepositoryTests
{
    public class ClassRepositoryTest
    {
        private Func<NpgsqlConnection> getDbConnection;
        private Mock<ILogger<StudentRepository>> studentLoggerMoq;
        private Mock<ILogger<ClassRepository>> classLoggerMoq;
        private Func<string> psqlString;
        private IClassRepository classRepository;
        private StudentRepository studentRepository;
        private ClassRoom scienceRoom;
        private Teacher jonathan;
        private Teacher heber;
        private Student sam;
        private Student ben;
        private Student tim;
        private ClassModel mathClass;
        private ClassModel scienceClass;
        private ClassModel notScienceClass;

        [SetUp]
        public async Task Setup()
        {
            getDbConnection = () => new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;");
            studentLoggerMoq = new Mock<ILogger<StudentRepository>>();
            classLoggerMoq = new Mock<ILogger<ClassRepository>>();
            psqlString = () => string.Empty;
            classRepository = new ClassRepository(getDbConnection, classLoggerMoq.Object, psqlString);

            studentRepository = new StudentRepository(getDbConnection, studentLoggerMoq.Object);
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
            jonathan = new Teacher(){ TeacherName = "jonathan" };
            heber = new Teacher(){ TeacherName = "not jonathan" };
            sam = new Student(){ StudentName = "sam"};
            ben = new Student(){ StudentName = "ben"};
            tim = new Student(){ StudentName = "tim"};
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
                StudentIds = new int[]{ sam.StudentId }
            };
            scienceClass = new ClassModel()
            {
                ClassName = "science",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId,
                StudentIds = new int[]{ sam.StudentId }
            };
            notScienceClass = new ClassModel()
            {
                ClassName = "not science",
                TeacherId = heber.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId,
                StudentIds = new int[]{ sam.StudentId }
            };
            await classRepository.AddClassAsync(mathClass);
            await classRepository.AddClassAsync(scienceClass);
            await classRepository.AddClassAsync(notScienceClass);
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

            mathClass.SeatingChartByStudentID = new int[3,3];
            mathClass.SeatingChartByStudentID[1,1] = sam.StudentId;

            await classRepository
                .Invoking(async cr => await cr.AddClassAsync(mathClass))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("cannot save seating chart 3,3 in classroom with dimensions 2,1");
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
            var studentRepository = new StudentRepository(getDbConnection, studentLoggerMoq.Object);
            var scienceRoom = new ClassRoom()
            { 
                ClassRoomName = "Science Room",
                SeatsHorizontally = 3,
                SeatsVertically = 3
            };
            var jonathan = new Teacher(){ TeacherName = "jonathan" };
            var sam = new Student(){ StudentName = "sam"};
            await classRepository.AddClassRoomAsync(scienceRoom);
            await classRepository.AddTeacherAsync(jonathan);
            await studentRepository.AddStudentAsync(sam);
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
                     .StudentIds
                     .Should().Contain(sam.StudentId);
            classList.Where(c => c.ClassId == scienceClass.ClassId)
                     .First()
                     .StudentIds
                     .Should().Contain(sam.StudentId);
        }

        [Test]
        public async Task add_students_in_class_on_insert()
        {
            var studentRepository = new StudentRepository(getDbConnection, studentLoggerMoq.Object);
            var scienceRoom = new ClassRoom()
            { 
                ClassRoomName = "Science Room",
                SeatsHorizontally = 3,
                SeatsVertically = 3
            };
            var jonathan = new Teacher(){ TeacherName = "jonathan" };
            var sam = new Student(){ StudentName = "sam"};
            await classRepository.AddClassRoomAsync(scienceRoom);
            await classRepository.AddTeacherAsync(jonathan);
            await studentRepository.AddStudentAsync(sam);
            var math = new ClassModel()
            {
                ClassName = "math",
                TeacherId = jonathan.TeacherId,
                ClassRoomId = scienceRoom.ClassRoomId,
                StudentIds = new int[] { sam.StudentId }
            };
            await classRepository.AddClassAsync(math);

            var newMath = await classRepository.GetClassAsync(math.ClassId);

            newMath.StudentIds.Count().Should().Be(1);
            newMath.StudentIds.First().Should().Be(sam.StudentId);
            
        }

    }
}