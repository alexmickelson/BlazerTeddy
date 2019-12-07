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
        private Mock<ILogger<CourseRepository>> courseLoggerMoq;
        private Func<string> psqlString;
        private ClassRepository classRepository;
        private CourseRepository courseRepository;
        private Student sam;
        private Student jim;
        private Teacher jonathan;
        private Course math1010;
        private ClassModel mathClass;
        private Note samsNote;

        [SetUp]
        public async Task Setup()
        {
            studentLoggingMoq = new Mock<ILogger<StudentRepository>>();
            Func<IDbConnection> getDbConnection = () =>
                new NpgsqlConnection("Server=localhost;Port=5433;User ID=teddy;Password=teddy;");
            classLoggerMoq = new Mock<ILogger<ClassRepository>>();
            courseLoggerMoq = new Mock<ILogger<CourseRepository>>();
            psqlString = () => string.Empty;
            classRepository = new ClassRepository(getDbConnection, classLoggerMoq.Object, psqlString);
            courseRepository = new CourseRepository(getDbConnection, courseLoggerMoq.Object);
            
            studentRepository = new StudentRepository(getDbConnection, studentLoggingMoq.Object);

            await seedDatbase();
        }

        private async Task seedDatbase()
        {
            sam = new Student() { StudentName = "sam" };
            jim = new Student() { StudentName = "jim" };
            await studentRepository.AddStudentAsync(sam);
            await studentRepository.AddStudentAsync(jim);
            samsNote = new Note(){ Content = "sam's note" };
            await studentRepository.AddUnsignedNoteAsync(sam, samsNote);
            var mathRoom = new ClassRoom()
            {
                ClassRoomName = "Math room"
            };
            await classRepository.AddClassRoomAsync(mathRoom);
            jonathan = new Teacher()
            {
                TeacherName = "jonathan"
            };
            await classRepository.AddTeacherAsync(jonathan);
            math1010 = new Course() 
            {
                CourseName = "math 1010",
                TeacherId = jonathan.TeacherId
            };
            await courseRepository.AddCourseAsync(math1010);
            mathClass = new ClassModel()
            {
                ClassName = "Math in the afternoon",
                ClassRoomId = mathRoom.ClassRoomId,
                TeacherId = jonathan.TeacherId
            };
            await classRepository.AddClassAsync(mathClass);
            await classRepository.EnrollStudentAsync(sam.StudentId, mathClass.ClassId, math1010.CourseId);
            await classRepository.EnrollStudentAsync(jim.StudentId, mathClass.ClassId, math1010.CourseId);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task can_insert_and_retrieve_student()
        {
            var newSam = await studentRepository.GetStudentAsync(sam.StudentId);
            newSam.StudentName.Should().Be(sam.StudentName);
        }

        [Test]
        public async Task can_add_note_to_student()
        {
            var newSam = await studentRepository.GetStudentAsync(sam.StudentId);
            newSam.Notes.Where(n => n.Content == samsNote.Content)
                .Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task can_add_signed_note_to_database()
        {
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
            var note = new Note()
            {
                Content = "A positive note about sam",
                StudentId = sam.StudentId,
                NoteType = Note.NoteTypes.Positive,
                DateCreated = DateTime.Now
            };

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
            await studentRepository.AddRestrictionAsync(sam.StudentId, jim.StudentId);
            var newSam = await studentRepository.GetStudentAsync(sam.StudentId);
            
            newSam.Restrictions.Should().Contain(jim.StudentId);
        }

        [Test]
        public async Task restrictions_are_transative()
        {            
            await studentRepository.AddRestrictionAsync(sam.StudentId, jim.StudentId);
            var newJim = await studentRepository.GetStudentAsync(jim.StudentId);
            
            newJim.Restrictions.Should().Contain(sam.StudentId);
        }

        [Test]
        public async Task can_get_list_of_students_by_classId()
        {

            var students = await studentRepository.GetStudentsByClassAsync(mathClass.ClassId);

            students.Count().Should().Be(2);
            var newSam = students.Where(s => s.StudentId == sam.StudentId).First();
            var newJim = students.Where(s => s.StudentId == jim.StudentId).First();
            newSam.StudentName.Should().Be(sam.StudentName);
            newJim.StudentName.Should().Be(jim.StudentName);
        }
    }
}