using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels
{
    public class ClassListViewModel : IViewModel
    {
        private readonly IClassRepository classRepository;

        public int TeacherId { get; set; }
        public IEnumerable<ClassModel> Classes { get; set; }

        public ClassListViewModel(IClassRepository classRepository)
        {
            this.classRepository = classRepository;
            Classes = new ClassModel[] {};
            TeacherId = 1; // TODO: make the logged in user
        }
        public void OnInitialized()
        {
            
        }

        public async Task OnInitializedAsync()
        {
            Classes = await classRepository.GetClassesByTeacherId(TeacherId);
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