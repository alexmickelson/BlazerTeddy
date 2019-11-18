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

        public void addNote()
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
    }
}
