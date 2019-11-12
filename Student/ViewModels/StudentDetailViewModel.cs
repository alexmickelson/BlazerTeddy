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
        public readonly StudentRepository studentRepository;
        public StudentInfo student;
        public string newComment;
        public string errorAlert;

        public StudentDetailViewModel()
        {
            // this.studentRepository = studentRepository;
            // student = this.studentRepository.GetStudent(studentID);

        }
        public void addNote()
        {
            if (String.IsNullOrEmpty(newComment))
            {
                errorAlert = "Note cannot be Empty";
            }
            else
            {
                student.Notes.Add(new Note()
                {
                    Content = newComment
                });
                errorAlert = "";
                newComment = "";
            }
        }

    }
}
