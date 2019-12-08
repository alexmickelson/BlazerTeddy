using System.Threading.Tasks;

namespace TeddyBlazor.ViewModels
{
    public interface IViewModel
    {
        // https://blazor-tutorial.net/lifecycle-methods
        void OnInitialized();
        Task OnInitializedAsync();
        void OnParametersSet();
        Task OnParametersSetAsync();
    }
}