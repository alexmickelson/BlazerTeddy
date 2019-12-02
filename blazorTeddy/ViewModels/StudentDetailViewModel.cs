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
            Student = new Student();
        }

        public async Task LoadStudentAsync(int studentId)
        {
            Student = await StudentRepository.GetStudentAsync(studentId);
        }

        public IEnumerable<Note> GetNotes()
        {
            return Student.Notes ?? new List<Note>();
        }

        public async Task AddNoteAsync()
        {
            if (String.IsNullOrEmpty(NewNote))
            {
                errorAlert = "Note cannot be Empty";
            }
            else
            {
                var note = new Note() { Content = NewNote };
                await StudentRepository.AddNoteAsync(Student, note);
                errorAlert = "";
                NewNote = "";
            }
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