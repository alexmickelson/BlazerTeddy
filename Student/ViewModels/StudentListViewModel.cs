
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Student.Data;
using Student.Models;
using Student.Services;

namespace Student.ViewModels
{
    public class StudentListViewModel
    {
        public readonly IStudentRepository studentRepository;
        public string NameFilter = "";
        public int ClassIdFilter = -1;

        public StudentListViewModel(IStudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public IEnumerable<StudentInfo> GetFilteredStudents()
        {
            if(ClassIdFilter > 0)
            {
                return studentRepository.GetList()
                    .Where(s => s.Name.ToLower().StartsWith(NameFilter.ToLower()))
                    .Where(s => s.Courses.Where(c => c.CourseId == ClassIdFilter).Count() > 0);
            }
            else
            {
                return studentRepository.GetList()
                    .Where(s => s.Name.ToLower().StartsWith(NameFilter.ToLower()));
            }
        }
    }
}