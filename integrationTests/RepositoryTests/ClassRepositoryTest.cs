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

namespace IntegrationTests.RepositoryTests
{
    public class ClassRepositoryTest
    {
        private Func<NpgsqlConnection> getDbConnection;
        private IClassRepository classRepository;

        [SetUp]
        public void Setup()
        {
            getDbConnection = () => new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;");
            classRepository = new ClassRepository(getDbConnection);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task can_add_classroom()
        {
            var classRoom = new ClassRoom()
            { 
                ClassRoomName = "Science Room",
                SeatsHorizontally = 4,
                SeatsVertically = 5
            };

            await classRepository.AddClassRoomAsync(classRoom);

            classRoom.ClassRoomId.Should().NotBe(default(int));
            var newClassRoom = await classRepository.GetClassRoomAsync(classRoom.ClassRoomId);
            newClassRoom.ClassRoomName.Should().Be(classRoom.ClassRoomName);
            newClassRoom.SeatsHorizontally.Should().Be(classRoom.SeatsHorizontally);
            newClassRoom.SeatsVertically.Should().Be(classRoom.SeatsVertically);
        }
        
        [Test]
        public async Task can_add_teacher()
        {
            var teacher = new Teacher(){ TeacherName = "jonathan" };

            await classRepository.AddTeacherAsync(teacher);

            teacher.TeacherId.Should().NotBe(default(int));
            var newTeacher = await classRepository.GetTeacherAsync(teacher.TeacherId);
            newTeacher.TeacherName.Should().Be(teacher.TeacherName);
        }

        [Test]
        public async Task can_get_list_of_classes()
        {
            var classRoom = new ClassRoom(){ ClassRoomName = "Science Room" };
            var teacher = new Teacher(){ TeacherName = "jonathan" };
            await classRepository.AddClassRoomAsync(classRoom);
            await classRepository.AddTeacherAsync(teacher);
            var classModel = new ClassModel()
            {
                ClassName = "math",
                TeacherId = teacher.TeacherId,
                ClassRoomId = classRoom.ClassRoomId
            };
            await classRepository.AddClassAsync(classModel);

            var newClass = await classRepository.GetClassAsync(classModel.ClassId);

            newClass.Should().NotBeNull();
            newClass.ClassName.Should().Be(classModel.ClassName);
            newClass.TeacherId.Should().Be(teacher.TeacherId);
            newClass.ClassRoomId.Should().Be(classRoom.ClassRoomId);
        }

        [Test]
        public async Task invalid_seating_chart_thows_exception()
        {
            var classRoom = new ClassRoom()
            { 
                ClassRoomName = "Science Room",
                SeatsHorizontally = 3,
                SeatsVertically = 3
            };
            var teacher = new Teacher(){ TeacherName = "jonathan" };
            await classRepository.AddClassRoomAsync(classRoom);
            await classRepository.AddTeacherAsync(teacher);
            var classModel = new ClassModel()
            {
                ClassName = "math",
                TeacherId = teacher.TeacherId,
                ClassRoomId = classRoom.ClassRoomId
            };

            classModel.SeatingChartByStudentID = new int[3,3];
            classModel.SeatingChartByStudentID[1,1] = -30;

            await classRepository
                .Invoking(async cr => await cr.AddClassAsync(classModel))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("cannot assign seat, no student with id -30");
        }

        [Test]
        public async Task can_save_seating_chart()
        {
            var studentRepository = new StudentRepository(getDbConnection);
            var classRoom = new ClassRoom()
            { 
                ClassRoomName = "Science Room",
                SeatsHorizontally = 3,
                SeatsVertically = 3
            };
            var teacher = new Teacher(){ TeacherName = "jonathan" };
            var student1 = new Student(){ StudentName = "sam"};
            var student2 = new Student(){ StudentName = "ben"};
            var student3 = new Student(){ StudentName = "tim"};
            await classRepository.AddClassRoomAsync(classRoom);
            await classRepository.AddTeacherAsync(teacher);
            await studentRepository.AddStudentAsync(student1);
            await studentRepository.AddStudentAsync(student2);
            await studentRepository.AddStudentAsync(student3);
            var classModel = new ClassModel()
            {
                ClassName = "math",
                TeacherId = teacher.TeacherId,
                ClassRoomId = classRoom.ClassRoomId
            };

            classModel.SeatingChartByStudentID = new int[3,3];
            classModel.SeatingChartByStudentID[1,1] = student1.StudentId;
            classModel.SeatingChartByStudentID[2,2] = student2.StudentId;
            classModel.SeatingChartByStudentID[2,0] = student3.StudentId;

            await classRepository.AddClassAsync(classModel);

            var newClass = await classRepository.GetClassAsync(classModel.ClassId);

            newClass.SeatingChartByStudentID[1,1].Should().Be(student1.StudentId);
            newClass.SeatingChartByStudentID[2,2].Should().Be(student2.StudentId);
            newClass.SeatingChartByStudentID[2,0].Should().Be(student3.StudentId);
            newClass.SeatingChartByStudentID[0,0].Should().Be(default(int));
        }

        [Test]
        public async Task seating_chart_must_be_valid_dimensions()
        {
            var studentRepository = new StudentRepository(getDbConnection);
            var classRoom = new ClassRoom()
            { 
                ClassRoomName = "Science Room",
                SeatsHorizontally = 2,
                SeatsVertically = 1
            };
            var teacher = new Teacher(){ TeacherName = "jonathan" };
            var student = new Student(){ StudentName = "sam"};
            await classRepository.AddClassRoomAsync(classRoom);
            await classRepository.AddTeacherAsync(teacher);
            await studentRepository.AddStudentAsync(student);
            var classModel = new ClassModel()
            {
                ClassName = "math",
                TeacherId = teacher.TeacherId,
                ClassRoomId = classRoom.ClassRoomId
            };

            classModel.SeatingChartByStudentID = new int[3,3];
            classModel.SeatingChartByStudentID[1,1] = student.StudentId;

            await classRepository
                .Invoking(async cr => await cr.AddClassAsync(classModel))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("cannot save seating chart 3,3 in classroom with dimensions 2,1");
        }

        [Test]
        public async Task can_update_classroom()
        {
            var classRoom = new ClassRoom()
            { 
                ClassRoomName = "Science Room",
                SeatsHorizontally = 4,
                SeatsVertically = 5
            };
            await classRepository.AddClassRoomAsync(classRoom);

            classRoom.ClassRoomName = "not the science room";

            await classRepository.UpdateClassRoomAsync(classRoom);

            classRoom.ClassRoomId.Should().NotBe(default(int));
            var newClassRoom = await classRepository.GetClassRoomAsync(classRoom.ClassRoomId);
            newClassRoom.ClassRoomName.Should().Be(classRoom.ClassRoomName);
            newClassRoom.SeatsHorizontally.Should().Be(classRoom.SeatsHorizontally);
            newClassRoom.SeatsVertically.Should().Be(classRoom.SeatsVertically);
            
        }
        
        [Test]
        public async Task can_update_teacher()
        {
            var teacher = new Teacher(){ TeacherName = "jonathan" };
            await classRepository.AddTeacherAsync(teacher);
            teacher.TeacherName = "heber";
            
            await classRepository.UpdateTeacherAsync(teacher);
            var newTeacher = await classRepository.GetTeacherAsync(teacher.TeacherId);

            newTeacher.TeacherName.Should().Be(teacher.TeacherName);
        }

        [Test]
        public async Task can_update_class()
        {
            var studentRepository = new StudentRepository(getDbConnection);
            var classRoom = new ClassRoom()
            { 
                ClassRoomName = "Science Room",
                SeatsHorizontally = 3,
                SeatsVertically = 3
            };
            var teacher = new Teacher(){ TeacherName = "jonathan" };
            var student = new Student(){ StudentName = "sam"};
            await classRepository.AddClassRoomAsync(classRoom);
            await classRepository.AddTeacherAsync(teacher);
            await studentRepository.AddStudentAsync(student);
            var classModel = new ClassModel()
            {
                ClassName = "math",
                TeacherId = teacher.TeacherId,
                ClassRoomId = classRoom.ClassRoomId
            };
            classModel.SeatingChartByStudentID = new int[3,3];
            classModel.SeatingChartByStudentID[1,1] = student.StudentId;
            await classRepository.AddClassAsync(classModel);

            classModel.SeatingChartByStudentID[0,0] = student.StudentId;
            classModel.SeatingChartByStudentID[1,1] = default(int);

            await classRepository.UpdateClassAsync(classModel);

            var newClass = classRepository.GetClassAsync(classModel.ClassId);

            classModel.SeatingChartByStudentID[0,0].Should().Be(student.StudentId);
            classModel.SeatingChartByStudentID[1,1].Should().Be(default(int));
        }

    }
}