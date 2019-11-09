
using System.Collections.Generic;
using System.Linq;
using Student.Models;

namespace Student.Services {
    public class StudentRepository {
        private static StudentRepository singletonRepo { get; set; }
        public static StudentRepository GetRepository()
        {
            return singletonRepo ?? (singletonRepo = new StudentRepository());
        }

        public List<StudentInfo> students { get; set; }
        
        private StudentRepository()
        {
            students = new List<StudentInfo>();
            students.Add(new StudentInfo(){
                ID=1,
                Name="jerry",
                Notes=new List<Note>()
            });
            students.Add(new StudentInfo(){
                ID=2,
                Name="Gerry",
                Notes=new List<Note>()
            });
            students.Add(new StudentInfo(){
                ID=3,
                Name="ahjerry",
                Notes=new List<Note>()
            });

            
        }
        public List<StudentInfo> GetStudents()
        {
            return students;
        }

        public StudentInfo GetStudent(int id)
        {
            return students.FirstOrDefault(s => s.ID == id);
        }

    }
}