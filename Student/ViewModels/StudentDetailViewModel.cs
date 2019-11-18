using Student.Models;
using Student.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student.ViewModels
{
    public class StudentDetailViewModel
    {
        public readonly IStudentRepository studentRepository;
        public StudentInfo Student;
        public string newNote;
        public string errorAlert;
        public int NewRestrictionId { get; set; }

        public StudentDetailViewModel(IStudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public void LoadStudent(int studentId)
        {
            Student = studentRepository.Get(studentId);
        }

        public IEnumerable<Note> GetNotes()
        {
            return Student.Notes ?? new List<Note>();
        }

        public void AddNote()
        {
            if (String.IsNullOrEmpty(newNote))
            {
                errorAlert = "Note cannot be Empty";
            }
            else
            {
                var note = new Note() { Content = newNote };
                studentRepository.AddNoteAsync(Student, note);
                errorAlert = "";
                newNote = "";
            }
        }

        public IEnumerable<StudentInfo> GetRestrictions()
        {
            return Student.Restrictions ?? new List<StudentInfo>();
        }

        public void AddRestriction()
        {
            studentRepository.AddRestrictionAsync(Student.StudentInfoId, NewRestrictionId);
        }
    }
}
