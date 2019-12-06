using TeddyBlazor.Models;
using TeddyBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TeddyBlazor.Models.Note;

namespace TeddyBlazor.ViewModels
{
    public class StudentDetailViewModel : IViewModel
    {
        public readonly IStudentRepository StudentRepository;
        public readonly INewNoteViewModel NewNoteVM;
        public Student Student;
        public int NewRestrictionId { get; set; }
        public int StudentId { get; set; }
        public IEnumerable<string> Restrictions { get; set; }

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

        public async Task LoadRestrictionsAsync()
        {
            IEnumerable<string> restrictions = new string[]{};

            foreach(var studentId in Student.Restrictions)
            {
                var restrictedStudent = await StudentRepository.GetStudentAsync(studentId);
                restrictions = restrictions.Append(restrictedStudent.StudentName);
            }
            Restrictions = restrictions;
        }

        public void AddRestriction()
        {
            var t = Task.Run(async () =>
            {
                await StudentRepository.AddRestrictionAsync(Student.StudentId, NewRestrictionId);
                await OnParametersSetAsync();
            });
            t.Wait();
        }

        public void OnInitialized()
        {
            
        }

        public Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public void OnParametersSet()
        {
        }

        public async Task OnParametersSetAsync()
        {
            await LoadStudentAsync(StudentId);
            await LoadRestrictionsAsync();
        } 
    }
}