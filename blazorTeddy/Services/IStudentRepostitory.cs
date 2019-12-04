using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetListAsync();
        Task<Student> GetStudentAsync(int id);
        Task UpdateStudentsAsync();
        Task SaveChangesAsync();
        Task AddRestrictionAsync(int studentId1, int studentId2);
        Task AddStudentAsync(Student sam);
        Task AddUnsignedNoteAsync(Student student, Note note);
        Task AddSignedNoteAsync(Student student, Note note, int teacherId);
    }
}