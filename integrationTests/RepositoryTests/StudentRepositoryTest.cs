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
using Moq;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Threading;

namespace IntegrationTests.RepositoryTests
{
    public class StudentRepositoryTest
    {
        private Mock<ILogger<StudentRepository>> studentLoggingMoq;
        private IStudentRepository studentRepository;
        private Mock<ILogger<ClassRepository>> classLoggerMoq;
        private Func<string> psqlString;
        private ClassRepository classRepository;

        [SetUp]
        public void Setup()
        {
            studentLoggingMoq = new Mock<ILogger<StudentRepository>>();
            Func<IDbConnection> getDbConnection = () =>
                new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;");
            studentRepository = new StudentRepository(getDbConnection,
                                                      studentLoggingMoq.Object);
            classLoggerMoq = new Mock<ILogger<ClassRepository>>();
            psqlString = () => string.Empty;
            classRepository = new ClassRepository(getDbConnection, classLoggerMoq.Object, psqlString);
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
            await studentRepository.AddUnsignedNoteAsync(sam, note);

            var newSam = await studentRepository.GetStudentAsync(sam.StudentId);
            newSam.Notes.Where(n => n.Content == note.Content)
                .Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task can_add_signed_note_to_database()
        {
            var jonathan = new Teacher()
            {
                TeacherName = "jonathan"
            };
            await classRepository.AddTeacherAsync(jonathan);
            var sam = new Student()
            {
                StudentName = "sam"
            };
            await studentRepository.AddStudentAsync(sam);
            var note = new Note()
            {
                Content = "A positive note about sam",
                StudentId = sam.StudentId,
                TeacherId = jonathan.TeacherId,
                NoteType = Note.NoteTypes.Positive,
                DateCreated = DateTime.Now
            };

            await studentRepository.AddSignedNoteAsync(sam, note, jonathan.TeacherId);

            var samWithNote = await studentRepository.GetStudentAsync(sam.StudentId);

            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).Count()
                       .Should().Be(1);
            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).First()
                       .Content.Should().Be(note.Content);
            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).First()
                       .StudentId.Should().Be(note.StudentId);
            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).First()
                       .TeacherId.Should().Be(note.TeacherId);
            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).First()
                       .NoteType.Should().Be(note.NoteType);
            var timedifference = samWithNote.Notes
                                .Where(n => n.NoteId == note.NoteId)
                                .First().DateCreated
                                - note.DateCreated;
            timedifference.Should().BeLessThan(new TimeSpan(2 * TimeSpan.TicksPerSecond));
        }

        [Test]
        public async Task can_add_unsigned_note_to_database()
        {
            var sam = new Student()
            {
                StudentName = "sam"
            };
            await studentRepository.AddStudentAsync(sam);
            var note = new Note()
            {
                Content = "A positive note about sam",
                StudentId = sam.StudentId,
                NoteType = Note.NoteTypes.Positive,
                DateCreated = DateTime.Now
            };

            Thread.Sleep(2000);
            await studentRepository.AddUnsignedNoteAsync(sam, note);

            var samWithNote = await studentRepository.GetStudentAsync(sam.StudentId);

            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).Count()
                       .Should().Be(1);
            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).First()
                       .Content.Should().Be(note.Content);
            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).First()
                       .StudentId.Should().Be(note.StudentId);
            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).First()
                       .NoteType.Should().Be(note.NoteType);

            samWithNote.Notes.Where(n => n.NoteId == note.NoteId).First()
                       .TeacherId.Should().Be(default(int));
            var timedifference = samWithNote.Notes
                                .Where(n => n.NoteId == note.NoteId)
                                .First().DateCreated
                                - note.DateCreated;
            timedifference.Should().BeLessThan(new TimeSpan(2 * TimeSpan.TicksPerSecond));
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

        [Test]
        public async Task can_get_list_of_students_by_classId()
        {
            var sam = new Student()
            {
                StudentName = "sam"
            };
            var jim = new Student()
            {
                StudentName = "jim"
            };
            await studentRepository.AddStudentAsync(sam);
            await studentRepository.AddStudentAsync(jim);
            var mathRoom = new ClassRoom()
            {
                ClassRoomName = "Math room"
            };
            await classRepository.AddClassRoomAsync(mathRoom);
            var jonathan = new Teacher()
            {
                TeacherName = "jonathan"
            };
            await classRepository.AddTeacherAsync(jonathan);
            var mathClass = new ClassModel()
            {
                ClassName = "Math in the afternoon",
                StudentIds = new int[] { sam.StudentId, jim.StudentId },
                ClassRoomId = mathRoom.ClassRoomId,
                TeacherId = jonathan.TeacherId
            };
            await classRepository.AddClassAsync(mathClass);

            var students = await studentRepository.GetStudentsByClassAsync(mathClass.ClassId);

            students.Count().Should().Be(2);
            var newSam = students.Where(s => s.StudentId == sam.StudentId).First();
            var newJim = students.Where(s => s.StudentId == jim.StudentId).First();
            newSam.StudentName.Should().Be(sam.StudentName);
            newJim.StudentName.Should().Be(jim.StudentName);
        }
    }
}