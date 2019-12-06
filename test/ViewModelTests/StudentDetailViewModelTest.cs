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

        private Mock<INewNoteViewModel> NewNoteVMMoq;
        private StudentDetailViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            StudentRepoMoq = new Mock<IStudentRepository>();
            NewNoteVMMoq = new Mock<INewNoteViewModel>();
            viewModel = new StudentDetailViewModel(StudentRepoMoq.Object, NewNoteVMMoq.Object);
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
            NewNoteVMMoq.Setup(nn => nn.SetStudent(It.IsAny<Student>()));

            await viewModel.LoadStudentAsync(1);
            viewModel.NewRestrictionId = student2.StudentId;

            await viewModel.AddRestrictionAsync();

            viewModel.Student.Restrictions.Should().Contain(student2.StudentId);
        }

        [Test]
        public void get_restrictions_returns_list_of_strings()
        {
            var adam = new Student()
            {
                StudentName="adam",
                StudentId = 1,
                Restrictions = new int[]{ 2 }
            };
            var spencer = new Student()
            {
                StudentName="spencer",
                StudentId = 2,
                Restrictions = new int[]{ 1 }
            };

            StudentRepoMoq.Setup(sr => sr.GetStudentAsync(1)).ReturnsAsync(adam);
            StudentRepoMoq.Setup(sr => sr.GetStudentAsync(2)).ReturnsAsync(spencer);
            viewModel.Student = adam;

            var restrictions = viewModel.GetRestrictions();

            restrictions.Count().Should().Be(1);
            restrictions.First().Should().Be(spencer.StudentName);
        }

    }
}