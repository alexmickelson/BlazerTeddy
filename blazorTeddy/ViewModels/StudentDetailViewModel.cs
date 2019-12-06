using TeddyBlazor.Models;
using TeddyBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TeddyBlazor.Models.Note;

namespace TeddyBlazor.ViewModels
{
    public class StudentDetailViewModel : IBaseViewModel
    {
        public readonly IStudentRepository StudentRepository;
        public readonly INewNoteViewModel NewNoteVM;
        public Student Student;
        public int NewRestrictionId { get; set; }

        public StudentDetailViewModel(IStudentRepository StudentRepository,
                                      INewNoteViewModel newNoteViewModel)
        {
            this.StudentRepository = StudentRepository;
            this.NewNoteVM = newNoteViewModel;
            Student = new Student();
        }

        public async Task LoadStudentAsync(int studentId)
        {
            Student = await StudentRepository.GetStudentAsync(studentId);
            NewNoteVM.SetStudent(Student);
        }

        public IEnumerable<Note> GetNotes()
        {
            return Student.Notes ?? new Note[]{};
        }



        public IEnumerable<string> GetRestrictions()
        {
            IEnumerable<string> restrictions = new string[]{};

            IEnumerable<Task<Student>> tasks = new Task<Student>[]{};
            foreach(var studentId in Student.Restrictions)
            {
                tasks = tasks.Append(StudentRepository.GetStudentAsync(studentId));
            }
            foreach(var task in tasks)
            {
                restrictions = restrictions.Append(task.Result.StudentName);
            }
            return restrictions;
        }

        public async Task AddRestrictionAsync()
        {
            await StudentRepository.AddRestrictionAsync(Student.StudentId, NewRestrictionId);
        }

        public void OnInitialized()
        {
            throw new NotImplementedException();
        }

        public void OnInitializedAsync()
        {
            throw new NotImplementedException();
        }

        public void OnParametersSet()
        {
            throw new NotImplementedException();
        }

        public void OnParametersSetAsync()
        {
            throw new NotImplementedException();
        }

        public void OnAfterRender()
        {
            throw new NotImplementedException();
        }

        public void OnAfterRenderAsync()
        {
            throw new NotImplementedException();
        }
    }
}