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
    public class StudentRepositoryTest
    {
        private StudentRepository studentRepository;

        [SetUp]
        public void Setup()
        {
            studentRepository = new StudentRepository(() => new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;"));
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task can_insert_and_retrieve_student()
        {
            var sam = new Student(){ StudentName = "sam" };
            
            await studentRepository.AddStudentAsync(sam);

            var newSam = await studentRepository.GetStudentAsync(sam.StudentId);
            newSam.StudentName.Should().Be("sam");
        }

        [Test]
        public async Task can_add_note_to_student()
        {
            var sam = new Student(){ StudentName = "sam" };
            await studentRepository.AddStudentAsync(sam);
            var note = new Note(){ Content = "sam's note" };
            await studentRepository.AddNoteAsync(sam, note);

            var newSam = await studentRepository.GetStudentAsync(sam.StudentId);
            newSam.Notes.Where(n => n.Content == note.Content)
                .Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task can_add_restriction_to_student()
        {
            var sam = new Student(){ StudentName = "sam" };
            var jim = new Student(){ StudentName = "jim" };
            await studentRepository.AddStudentAsync(sam);
            await studentRepository.AddStudentAsync(jim);
            
            await studentRepository.AddRestrictionAsync(sam.StudentId, jim.StudentId);
            var newSam = await studentRepository.GetStudentAsync(sam.StudentId);
            
            newSam.Restrictions.Should().Contain(jim.StudentId);
        }

        [Test]
        public async Task restrictions_are_transative()
        {
            var sam = new Student(){ StudentName = "sam" };
            var jim = new Student(){ StudentName = "jim" };
            await studentRepository.AddStudentAsync(sam);
            await studentRepository.AddStudentAsync(jim);
            
            await studentRepository.AddRestrictionAsync(sam.StudentId, jim.StudentId);
            var newJim = await studentRepository.GetStudentAsync(jim.StudentId);
            
            newJim.Restrictions.Should().Contain(sam.StudentId);
        }
    }
}