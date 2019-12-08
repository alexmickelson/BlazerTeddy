using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels
{
    public class CourseListViewModel : IViewModel
    {
        private readonly ICourseRepository courseRepository;
        public IEnumerable<Course> Courses { get; set; }
        public int TeacherId { get; set; }

        public CourseListViewModel(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
            Courses = new Course[] {};
            TeacherId = 1;
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
            Courses = await courseRepository.GetCoursesByTeacherId(TeacherId);
        }
    }
}