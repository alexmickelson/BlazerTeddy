using System.Threading.Tasks;
using TeddyBlazor.Models;
using TeddyBlazor.Services;

namespace TeddyBlazor.ViewModels
{
    public class SeatingChartViewModel : IViewModel
    {
        private readonly IStudentRepository studentRepository;
        private readonly IClassRepository classRepository;

        public ClassModel SelectedClass { get; set; }
        public ClassRoom ClassRoom { get; set; }
        public double HorizontalFraction => (1.0 / this.ClassRoom.SeatsHorizontally) * 100;
        public SeatingChartViewModel(IStudentRepository studentRepository,
                                     IClassRepository classRepository)
        {
            this.studentRepository = studentRepository;
            this.classRepository = classRepository;
            SelectedClass = new ClassModel();
            ClassRoom = new ClassRoom();
        }

        public string GetStudentOrDefault(int i,int j)
        {
            return "";
        }

        public string GetCourseOrDefault(int i,int j)
        {
            return "";
        }

        public int GetStudentIdOrDefault(int i,int j)
        {
            return 1;
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
            ClassRoom = SelectedClass.ClassRoomId == 0
                ? ClassRoom
                : await classRepository.GetClassRoomAsync(SelectedClass.ClassRoomId);
        }
    }
}