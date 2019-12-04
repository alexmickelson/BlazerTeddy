using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TeddyBlazor.Models;
using TeddyBlazor.Services;
using TeddyBlazor.ViewModels;

namespace Test.ViewModelTests
{
    public class StudentDetailViewModelTest
    {
        public Mock<IStudentRepository> StudentRepoMoq { get; private set; }

        private StudentDetailViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            StudentRepoMoq = new Mock<IStudentRepository>();
            viewModel = new StudentDetailViewModel(StudentRepoMoq.Object);
        }


        [Test]
        public async Task student_refreshes_after_adding_note()
        {
            var student1 = new Student(){ StudentName="adam", StudentId = 1 };
            var studentList = new List<Student>() { student1 };
            var studentNote = "a note about adam";

            StudentRepoMoq.Setup(sr => sr.GetListAsync())
                .ReturnsAsync(studentList);
            StudentRepoMoq.Setup(sr => sr.GetStudentAsync(student1.StudentId))
                .ReturnsAsync(studentList.Find(s => s.StudentId == student1.StudentId));
            StudentRepoMoq.Setup(sr => sr.AddUnsignedNoteAsync(student1, It.IsAny<Note>()))
                .Callback(() => student1.Notes = student1.Notes.Append(new Note(){Content = studentNote}));
                
            viewModel.NewNote = studentNote;
            await viewModel.LoadStudentAsync(1);

            await viewModel.AddNoteAsync();

            viewModel.Student.Notes.Where(n => n.Content == studentNote).Should().NotBeNullOrEmpty();
        }


        [Test]
        public async Task student_refreshes_after_adding_restriction()
        {
            var student1 = new Student(){ StudentName="adam", StudentId = 1 };
            var student2 = new Student(){ StudentName="spencer", StudentId = 2 };
            var studentList = new List<Student>() { student1 };

            StudentRepoMoq.Setup(sr => sr.GetListAsync())
                .ReturnsAsync(studentList);
            StudentRepoMoq.Setup(sr => sr.GetStudentAsync(student1.StudentId))
                .ReturnsAsync(studentList.Find(s => s.StudentId == student1.StudentId));
            StudentRepoMoq.Setup(sr => sr.AddRestrictionAsync(student1.StudentId, student2.StudentId))
                .Callback(() => student1.Restrictions = student1.Restrictions.Append(student2.StudentId));

            await viewModel.LoadStudentAsync(1);
            viewModel.NewRestrictionId = student2.StudentId;

            await viewModel.AddRestrictionAsync();

            viewModel.Student.Restrictions.Should().Contain(student2.StudentId);
        }

        [Test]
        public void add_signed_note_if_box_checked()
        {

        }
    }
}