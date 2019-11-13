
using System.Collections.Generic;
using System.Threading.Tasks;
using Student.Data;
using Student.Models;
using Student.Services;

namespace Student.ViewModels
{
    public class StudentListViewModel
    {
        private readonly StudentRepository studentRepository;
        public IEnumerable<StudentInfo> Students;
        public StudentListViewModel(StudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
            Students = this.studentRepository.GetStudents();
        }

        public  IEnumerable<StudentInfo> GetStudents()
        {
            return  studentRepository.GetStudents(); 
        }
    }
}