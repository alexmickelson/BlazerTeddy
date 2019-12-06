using TeddyBlazor.Models;
using TeddyBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TeddyBlazor.Models.Note;

namespace TeddyBlazor.ViewModels
{
    public class StudentDetailViewModel
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



        public IEnumerable<int> GetRestrictions()
        {
            return Student.Restrictions ?? new int[0];
        }

        public async Task AddRestrictionAsync()
        {
            await StudentRepository.AddRestrictionAsync(Student.StudentId, NewRestrictionId);
        }

    }
}