using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface IStudentRepository
    {
        public Task AddNoteAsync(Student student, Note note);
        public Task<List<Student>> GetListAsync();
        public Task<Student> GetStudentAsync(int id);
        public Task UpdateStudentsAsync();
        Task SaveChangesAsync();
        public Task AddRestrictionAsync(int studentId1, int studentId2);
    }
}