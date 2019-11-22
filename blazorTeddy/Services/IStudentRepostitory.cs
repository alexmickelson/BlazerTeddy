using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface IStudentRepository
    {
        public List<Student> students { get; set; }
        public void Add(Student student);
        public Task AddNoteAsync(Student student, Note note);
        public Student Get(int studentId);
        public List<Student> GetList();
        public Task<Student> GetStudentAsync(int id);
        public Task InitializeStudentsAsync();
        Task UpdateDatabaseAsync();
        public Task AddRestrictionAsync(int studentId1, int studentId2);
    }
}