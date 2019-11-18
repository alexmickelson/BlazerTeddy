
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
        public int CourseIdFilter = -1;

        public StudentListViewModel(IStudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public IEnumerable<StudentInfo> GetFilteredStudents()
        {
            if(CourseIdFilter > 0)
            {
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
            }
            else
            {
                return studentRepository.GetList()
                    .Where(s => s.Name.ToLower().StartsWith(NameFilter.ToLower()));
            }
        }

        public IEnumerable<Course> GetCourses()
        {
            var t = studentRepository.GetCoursesAsync();
            t.Wait();
            return t.Result;
        }
    }
}