using System;
using System.Threading.Tasks;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels
{
    public class ClassDetailViewModel : IViewModel
    {
        private readonly IClassRepository classRepository;

        public int ClassId { get; set; }
        public ClassModel SelectedClass { get; set; }
        public Action<int> SelectTab { get; set; }
        public Action Reload { get; set; }
        public string SeatingChartCssClass { get; set; }
        public string CourseCssClass { get; set; }

        public ClassDetailViewModel(IClassRepository classRepository)
        {
            this.classRepository = classRepository;
            SelectedClass = new ClassModel();
            Reload = () => {};
            SelectTab = (tabNumber) => {
                SeatingChartCssClass = tabNumber == 0
                    ? string.Empty
                    : "collapse";
                CourseCssClass = tabNumber == 1
                    ? string.Empty
                    : "collapse";
                Reload();
            };
            SelectTab(0);
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
            SelectedClass = await classRepository.GetClassAsync(ClassId);
        }
    }
}