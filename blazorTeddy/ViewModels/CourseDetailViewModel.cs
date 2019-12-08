using System.Threading.Tasks;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels
{
    public class CourseDetailViewModel : IViewModel
    {
        private readonly ICourseRepository courseRepository;
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public CourseDetailViewModel(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
            Course = new Course();
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
            Course = await courseRepository.GetCourseAsync(CourseId);
        }
    }
}