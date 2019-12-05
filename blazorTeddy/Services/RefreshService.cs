using System;
using System.Threading.Tasks;

namespace TeddyBlazor.Services
{
    public class RefreshService : IRefreshService
    {
        public event Action RefreshRequested;
        public void CallRequestRefresh()
        {
            RefreshRequested?.DynamicInvoke();
        }
    }
}