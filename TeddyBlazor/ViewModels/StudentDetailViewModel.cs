﻿using TeddyBlazor.Models;
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
        }

        public void LoadTeddyBlazor(int TeddyBlazorId)
        {
            Student = StudentRepository.Get(TeddyBlazorId);
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
                StudentRepository.AddNoteAsync(Student, note);
                errorAlert = "";
                NewNote = "";
            }
        }



        public IEnumerable<Student> GetRestrictions()
        {
            return Student.Restrictions ?? new List<Student>();
        }

        public void AddRestriction()
        {
            StudentRepository.AddRestrictionAsync(Student.Id, NewRestrictionId);
        }
    }
}