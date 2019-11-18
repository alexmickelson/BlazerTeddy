
using System.Collections.Generic;
using System.Threading.Tasks;
using Student.Data;
using Student.Models;
using Student.Services;

namespace Student.ViewModels
{
    public class StudentListViewModel
    {
        public readonly StudentRepository studentRepository;
        public StudentListViewModel(StudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public  IEnumerable<StudentInfo> GetStudents()
        {
            return  studentRepository.GetList(); 
        }
    }
}