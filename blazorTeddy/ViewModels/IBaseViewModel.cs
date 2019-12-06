using System.Threading.Tasks;

namespace TeddyBlazor.ViewModels
{
    public interface IBaseViewModel
    {
        // https://blazor-tutorial.net/lifecycle-methods
        void OnInitialized();
        Task OnInitializedAsync();
        void OnParametersSet();
        Task OnParametersSetAsync();
    }
}