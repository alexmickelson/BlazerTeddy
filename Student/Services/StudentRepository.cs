
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Student.Data;
using Student.Models;

namespace Student.Services {

    public class StudentRepository : IStudentRepository
    {
        private readonly OurDbContext dbContext;
        public List<StudentInfo> students { get; set; }

        public StudentRepository(OurDbContext dbContext)
        {
            students = new List<StudentInfo>();
            this.dbContext = dbContext;

        // var math = new Course(){ CourseId = 1, Name = "math"};
        // var science = new Course(){ CourseId = 2, Name = "science"};
        // var student1 = new StudentInfo(){ StudentInfoId=1, Name="adam"};
        // var student2 = new StudentInfo(){ StudentInfoId=2, Name="benny"};
        // var student3 = new StudentInfo(){ StudentInfoId=3, Name="spencer"};
        // dbContext.Courses.Add(math);
        // dbContext.Courses.Add(science);
        // dbContext.SaveChanges();
        // Add(student1);
        // Add(student2);
        // Add(student3);
        // var ta = UpdateDatabaseAsync();
        // ta.Wait();


            var t = InitializeStudentAsync();
        }
        public async Task InitializeStudentAsync()
        {
            var tempList = await dbContext.Students.ToArrayAsync();
            foreach (var student in tempList)
            {
                if (!students.Contains(student))
                {
                    students.Add(student);
                }
            }
        }

        public List<StudentInfo> GetList()
        {
            var t = InitializeStudentAsync();
            return students;
        }

        public async Task<StudentInfo> GetStudentAsync(int id)
        {
            await InitializeStudentAsync();
            return students.FirstOrDefault(s => s.StudentInfoId == id);
        }

        public StudentInfo Get(int studentId)
        {
            var s = students.FirstOrDefault(s => s.StudentInfoId == studentId);
            return s;
        }

        public void Add(StudentInfo student)
        {
            students.Add(student);
        }

        public async Task UpdateDatabaseAsync()
        {
            foreach (var student in students)
            {
                if (dbContext.Students.Contains(student))
                {
                    var s = dbContext.Students.First(s => s.StudentInfoId == student.StudentInfoId);
                    s.Update(student);
                    dbContext.Students.Update(s);
                }
                else
                {
                    await dbContext.Students.AddAsync(student);
                }
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task AddNoteAsync(StudentInfo student, Note note)
        {
            if (student.Notes == null)
            {
                student.Notes = new List<Note>();
            }
            await dbContext.Notes.AddAsync(note);
            student.Notes.Add(note);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddRestrictionAsync(int studentId1, int studentId2)
        {
            var student1 = Get(studentId1);
            var student2 = Get(studentId2);
            if (student1.Restrictions == null)
            {
                student1.Restrictions = new List<StudentInfo>();
            }
            if (student2.Restrictions == null)
            {
                student2.Restrictions = new List<StudentInfo>();
            }
            student1.Restrictions.Add(student2);
            student2.Restrictions.Add(student1);
            await dbContext.SaveChangesAsync();
        }

        
    }
}