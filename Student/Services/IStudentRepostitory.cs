using System.Collections.Generic;
using System.Threading.Tasks;
using Student.Models;

namespace Student.Services
{
    public interface IStudentRepository
    {
        public List<StudentInfo> students { get; set; }
        public void Add(StudentInfo student);
        public Task AddNoteAsync(StudentInfo student, Note note);
        public StudentInfo Get(int studentId);
        public List<StudentInfo> GetList();
        public Task<StudentInfo> GetStudentAsync(int id);
        public Task InitializeStudentAsync();
        Task UpdateDatabaseAsync();
        Task AddRestrictionAsync(int studentId1, int studentId2);
        Task<IEnumerable<Course>> GetCoursesAsync();
    }
}