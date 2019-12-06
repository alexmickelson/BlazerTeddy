namespace TeddyBlazor.ViewModels
{
    public interface IBaseViewModel
    {
        // https://blazor-tutorial.net/lifecycle-methods
        void OnInitialized();
        void OnInitializedAsync();
        void OnParametersSet();
        void OnParametersSetAsync();
        void OnAfterRender();
        void OnAfterRenderAsync();
    }
}