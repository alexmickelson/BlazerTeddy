using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels
{
    public class CourseDetailViewModel : IViewModel
    {
        private readonly ICourseRepository courseRepository;
        private readonly IAssignmentRepository assignmentRepository;

        public int CourseId { get; set; }
        public Course Course { get; set; }
        public IEnumerable<Assignment> Assignments { get; set; }

        public CourseDetailViewModel(ICourseRepository courseRepository,
                                     IAssignmentRepository assignmentRepository)
        {
            this.courseRepository = courseRepository;
            this.assignmentRepository = assignmentRepository;
            Course = new Course();
            Assignments = new Assignment[] {};
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
            Assignments = await assignmentRepository.GetAssignmentsAsync(CourseId);
        }
    }
}