using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public interface IStudentRepository
    {
        public void AddNote(Student student, Note note);
        public List<Student> GetList();
        public Student GetStudent(int id);
        public void UpdateStudents();
        public void SaveChanges();
        public void AddRestriction(int studentId1, int studentId2);
    }
}