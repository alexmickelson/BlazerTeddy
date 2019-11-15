
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Student.Data;
using Student.Models;

namespace Student.Services {
    public class StudentRepository {
        private readonly OurDbContext dbContext;
        public List<StudentInfo> students { get; set; }

        public StudentRepository(OurDbContext dbContext)
        {
            students = new List<StudentInfo>();
            this.dbContext = dbContext;
            var t = InitializeStudentAsync();
        }
        private async Task InitializeStudentAsync()
        {
            var tempList = await dbContext.Students.ToArrayAsync();
            foreach(var student in tempList)
            {
                if (!students.Contains(student)){
                    students.Append(student);
                }
            }
        }

        public List<StudentInfo> GetStudents()
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
    }
}