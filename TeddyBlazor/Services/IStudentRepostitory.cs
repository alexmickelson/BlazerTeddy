using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface IStudentRepository
    {
        public List<Student> students { get; set; }
        public void Add(Student TeddyBlazor);
        public Task AddNoteAsync(Student TeddyBlazor, Note note);
        public Student Get(int TeddyBlazorId);
        public List<Student> GetList();
        public Task<Student> GetTeddyBlazorAsync(int id);
        public Task InitializeTeddyBlazorAsync();
        Task UpdateDatabaseAsync();
        public Task AddRestrictionAsync(int TeddyBlazorId1, int TeddyBlazorId2);
    }
}