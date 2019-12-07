using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;

namespace TeddyBlazor.ViewModels.ClassDetail
{
    public class ClassDetailCourseListViewModel : IViewModel
    {
        public IEnumerable<Course> Courses { get; set; }

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