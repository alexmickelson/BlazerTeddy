using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface IStudentRepository
    {
        public Task<List<Student>> GetListAsync();
        public Task<Student> GetStudentAsync(int id);
        public Task UpdateStudentsAsync();
        public Task SaveChangesAsync();
        public Task AddRestrictionAsync(int studentId1, int studentId2);
        public Task AddStudentAsync(Student sam);
        public Task AddUnsignedNoteAsync(Student student, Note note);
        public Task AddSignedNoteAsync(Student student, Note note, int teacherId);
    }
}