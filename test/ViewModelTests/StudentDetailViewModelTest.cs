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
        public async Task get_restrictions_returns_list_of_strings()
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

            await viewModel.LoadRestrictionsAsync();

            viewModel.Restrictions.Count().Should().Be(1);
            viewModel.Restrictions.First().Should().Be(spencer.StudentName);
        }

    }
}