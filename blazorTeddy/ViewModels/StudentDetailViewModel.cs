using TeddyBlazor.Models;
using TeddyBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeddyBlazor.ViewModels
{
    public class StudentDetailViewModel
    {
        public readonly IStudentRepository StudentRepository;
        public Student Student;
        public string NewNote;
        public string errorAlert;
        public int NewRestrictionId { get; set; }

        public StudentDetailViewModel(IStudentRepository StudentRepository)
        {
            this.StudentRepository = StudentRepository;
            Student = new Student();
        }

        public void LoadStudent(int studentId)
        {
            Student = StudentRepository.GetStudent(studentId);
        }

        public IEnumerable<Note> GetNotes()
        {
            return Student.Notes ?? new List<Note>();
        }

        public void AddNote()
        {
            if (String.IsNullOrEmpty(NewNote))
            {
                errorAlert = "Note cannot be Empty";
            }
            else
            {
                var note = new Note() { Content = NewNote };
                StudentRepository.AddNote(Student, note);
                errorAlert = "";
                NewNote = "";
            }
        }



        public IEnumerable<int> GetRestrictions()
        {
            return Student.Restrictions ?? new int[0];
        }

        public void AddRestriction()
        {
            StudentRepository.AddRestriction(Student.StudentId, NewRestrictionId);
        }


    }
}
