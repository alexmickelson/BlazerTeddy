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
            
            await studentRepository.AddAsync(sam);

            var newSam = await studentRepository.GetStudentAsync(sam.StudentId);
            newSam.StudentName.Should().Be("sam");
        }

        [Test]
        public async Task can_add_note_to_student()
        {
            var sam = new Student(){ StudentName = "sam" };
            await studentRepository.AddAsync(sam);
            var note = new Note(){ Content = "sam's note" };
            await studentRepository.AddNoteAsync(sam, note);

            var newSam = await studentRepository.GetStudentAsync(sam.StudentId);
            newSam.Notes.Where(n => n.Content == note.Content)
                .Should().NotBeNullOrEmpty();
        }
    }
}