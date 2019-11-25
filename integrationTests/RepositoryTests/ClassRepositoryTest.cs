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
        private IClassRepository classRepository;

        [SetUp]
        public void Setup()
        {
            classRepository = new ClassRepository(() => new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;"));
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task can_get_list_of_classes()
        {
            var classRoom = new ClassRoom(){ ClassRoomName = "Science Room" };
            var teacher = new Teacher(){ TeacherName = "jonathan" };
            var classModel = new ClassModel()
            {
                ClassName = "math",
                Teacher = teacher,
                ClassRoom = classRoom
            };

            await classRepository.AddClassAsync(classModel);

            var newClass = await classRepository.GetClassAsync(classModel.ClassId);
            newClass.Should().NotBeNull();
            newClass.ClassName.Should().Be(classModel.ClassName);
            newClass.Teacher.TeacherName.Should().Be(teacher.TeacherName);
            newClass.ClassRoom.ClassRoomName.Should().Be(classRoom.ClassRoomName);
        }
    }
}