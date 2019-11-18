
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
<<<<<<< HEAD
                // var s = studentRepository.GetList();
                // var n = s.Where(s => s.Name.ToLower().StartsWith(NameFilter.ToLower())).ToArray();
                // var d = n.Where(s => {
                //         return s.Courses == null
                //             ? false
                //             : s.Courses.Where(c => c.CourseId == CourseIdFilter).Count() > 0;
                //     }).ToArray();
                return studentRepository.GetList()
                    .Where(s => s.Name.ToLower().StartsWith(NameFilter.ToLower()))
                    .Where(s => {
                        return s.StudentCourses == null
                            ? false
                            : s.StudentCourses.Where(sc => sc.CourseId == CourseIdFilter).Count() > 0;
                    })
                    .ToArray();
=======
                return studentRepository.GetList()
                    .Where(s => s.Name.ToLower().StartsWith(NameFilter.ToLower()))
                    .Where(s => s.Courses.Where(c => c.CourseId == ClassIdFilter).Count() > 0);
>>>>>>> parent of 7e8e239... many to many course to students giving issues
            }
            else
            {
                return studentRepository.GetList()
                    .Where(s => s.Name.ToLower().StartsWith(NameFilter.ToLower()));
            }
        }
    }
}