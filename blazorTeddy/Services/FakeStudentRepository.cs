using System.Collections.Generic;
using TeddyBlazor.Models;

namespace TeddyBlazor.Services
{
    public class FakeStudentRepository : IStudentRepository
    {
        public void AddNote(Student student, Note note)
        {
        }

        public void AddRestriction(int studentId1, int studentId2)
        {
            //  throw new System.NotImplementedException();
        }

        public List<Student> GetList()
        {
            return new List<Student>()
            {
                new Student(){ StudentName = "sam", StudentId = 1}
            };
            //  throw new System.NotImplementedException();
        }

        public Student GetStudent(int id)
        {
            return new Student()
            {
                StudentName = "sam",
                StudentId = 1
            };
            //  throw new System.NotImplementedException();
        }

        public void SaveChanges()
        {
            //  throw new System.NotImplementedException();
        }

        public void UpdateStudents()
        {
            //  throw new System.NotImplementedException();
        }
    }
}