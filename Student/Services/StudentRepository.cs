
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
            this.dbContext = dbContext;
            asyncConstructor();
        }
        private async void asyncConstructor()
        {
            students = await dbContext.Students.ToArrayAsync();
        }
        
        public async Task<IEnumerable<StudentInfo>> GetStudentsAsync()
        {
            return students;
        }

        public async Task<StudentInfo> GetStudentAsync(int id)
        {
            students = await dbContext.Students.ToArrayAsync();
            return students.FirstOrDefault(s => s.StudentInfoId == id);
        }

    }
}