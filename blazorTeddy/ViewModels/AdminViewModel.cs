using TeddyBlazor.Models;
using TeddyBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TeddyBlazor.Models.Note;

namespace TeddyBlazor.ViewModels
{
    public class AdminViewModel : IViewModel
    {
    
        public readonly IStudentRepository StudentRepository;
        public readonly IClassRepository TeacherRepository;
        public readonly ICourseRepository CourseRepository;
        private readonly IRefreshService refreshService;
        public Student Student;
        public Teacher Teacher;
        public Course Course;

        public IEnumerable<string> Restrictions { get; set; }

        public AdminViewModel(IStudentRepository StudentRepository,
                              IClassRepository TeacherRepository,
                              ICourseRepository CourseRepository,
                              IRefreshService refreshService)
        {
            this.StudentRepository = StudentRepository;
            this.TeacherRepository = TeacherRepository;
            this.CourseRepository = CourseRepository;
            this.refreshService = refreshService;
            Student = new Student();
            Teacher = new Teacher();
            Course = new Course();
       
        }

        public void AddStudent()
        {

            var t = Task.Run(async () =>
            {
                await StudentRepository.AddStudentAsync(Student);
                Student = new Student();
                refreshService.CallRefresh();
            });
            t.Wait();
        }

        public void AddTeacher()
        {
            var t = Task.Run(async () =>
            {
                await TeacherRepository.AddTeacherAsync(Teacher);
                Teacher = new Teacher();
                refreshService.CallRefresh();
            });
            t.Wait();
        }
         public void AddCourse()
         {
            var t = Task.Run(async () =>
            {
                await CourseRepository.AddCourseAsync(Course);
                Course = new Course();
                refreshService.CallRefresh();
            });
            t.Wait();
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

        public Task OnParametersSetAsync()
        {
            return Task.CompletedTask;
        } 
    }
}