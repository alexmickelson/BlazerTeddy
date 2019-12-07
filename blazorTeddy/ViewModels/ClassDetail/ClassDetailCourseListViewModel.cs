using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels.ClassDetail
{
    public class ClassDetailCourseListViewModel : IViewModel
    {
        private readonly ICourseRepository courseRepository;
        private readonly IStudentRepository studentRepository;

        public int ClassId { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Student> Students { get; set; }
        

        public ClassDetailCourseListViewModel(ICourseRepository courseRepository,
                                              IStudentRepository studentRepository)
        {
            this.courseRepository = courseRepository;
            this.studentRepository = studentRepository;
            Courses = new Course[] {};
            Students = new Student[] {};
        }

        public IEnumerable<string> GetStudentNamesByCourse(int courseId)
        {
            var c = Courses
                .Single(c => c.CourseId == courseId)
                .StudentIds
                .Join(
                    Students,
                    sid => sid,
                    s => s.StudentId,
                    (sid, s) => s.StudentName
                );
                return c;
        }

        public void OnInitialized()
        {
        }

        public Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public void OnParametersSet()
        {

        }

        public async Task OnParametersSetAsync()
        {
            Courses = await courseRepository.GetCoursesByClassId(ClassId);
            Students = await studentRepository.GetStudentsByClassAsync(ClassId);
        }
    }
}