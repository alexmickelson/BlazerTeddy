
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Student.Data;
using Student.Models;

namespace Student.Services {
    public class StudentRepository {
        private readonly OurDbContext dbContext;
        public IEnumerable<StudentInfo> students { get; set; }

        public StudentRepository(OurDbContext dbContext)
        {
            students = new StudentInfo[0];
            this.dbContext = dbContext;
            InitializeStudentAsync();
        }
        private async void InitializeStudentAsync()
        {
            var tempList = await dbContext.Students.ToArrayAsync();

            foreach(var student in tempList)
            {
                if (!students.Contains(student)){
                    students.Append(student);
                }
            }

        }

        public IEnumerable<StudentInfo> GetStudents()
        {
            InitializeStudentAsync();
            return students;
        }

        public async Task<StudentInfo> GetStudentAsync(int id)
        {
            students = await dbContext.Students.ToArrayAsync();
            return students.FirstOrDefault(s => s.StudentInfoId == id);
        }




    }
}