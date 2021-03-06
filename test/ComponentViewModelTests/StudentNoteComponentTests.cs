using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TeddyBlazor.Models;
using TeddyBlazor.Services;
using TeddyBlazor.ViewModels;

namespace Test.ComponentViewModelTests
{
    public class NewNoteComponentTests
    {
        public Mock<IStudentRepository> StudentRepoMoq { get; private set; }

        private Mock<ILogger<StudentNoteViewModel>> loggerMoq;
        private StudentNoteViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            StudentRepoMoq = new Mock<IStudentRepository>();
            loggerMoq = new Mock<ILogger<StudentNoteViewModel>>();
            viewModel = new StudentNoteViewModel(StudentRepoMoq.Object, loggerMoq.Object);
        }

        [Test]
        public void add_unsigned_note_if_box_checked()
        {
            var adam = new Student(){ StudentName="adam", StudentId = 1 };
            var studentList = new List<Student>() { adam };

            StudentRepoMoq.Setup(sr => sr.GetListAsync())
                .ReturnsAsync(studentList);
            StudentRepoMoq.Setup(sr => sr.GetStudentAsync(adam.StudentId))
                .ReturnsAsync(studentList.Find(s => s.StudentId == adam.StudentId));
            
            StudentRepoMoq.Setup(sr => sr.AddUnsignedNoteAsync(It.IsAny<Student>(), It.IsAny<Note>()))
                          .Verifiable();

            viewModel.Student = adam;
            viewModel.IsAnonymousNote = true;
            viewModel.Note.Content = "a note about adam";
            viewModel.AddNote();

            StudentRepoMoq.Verify();

        }
    }
}