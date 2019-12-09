using System;
using System.Collections.Generic;

namespace TeddyBlazor.Services
{
    public interface IRefreshService
    {
        void AddOrUpdateRefresh(string name, Action newRefreshAction);
        void CallRefresh();
        void RemoveRefresh(string name);
    }
}