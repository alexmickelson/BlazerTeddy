
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

        public StudentListViewModel(StudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public async Task<IEnumerable<StudentInfo>> GetStudentsAsync()
        {
            return await studentRepository.GetStudentsAsync(); 
        }
    }
}