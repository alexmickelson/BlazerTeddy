namespace TeddyBlazor.ViewModels
{
    public interface IBaseViewModel
    {
        void OnInitialized();
        void OnInitializedAsync();
        void OnParametersSet();
        void OnParametersSetAsync();
        void OnAfterRender();
        void OnAfterRenderAsync();
    }
}