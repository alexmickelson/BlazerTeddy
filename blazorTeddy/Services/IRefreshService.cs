using System;

namespace TeddyBlazor.Services
{
    public interface IRefreshService
    {
        event Action RefreshRequested;

        void CallRequestRefresh();
    }
}