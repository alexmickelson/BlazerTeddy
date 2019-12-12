using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TeddyBlazor.Services
{

    public class RefreshService : IRefreshService
    {
        private readonly ILogger<RefreshService> logger;

        public ConcurrentDictionary<string, Action> RefreshActions { get; set; }
        public RefreshService(ILogger<RefreshService> logger)
        {
            RefreshActions = new ConcurrentDictionary<string, Action>();
            this.logger = logger;
        }

        public void AddOrUpdateRefresh(string name, Action newRefreshAction)
        {
            if(RefreshActions.ContainsKey(name))
            {
                logger.LogInformation($"updating refresh action {name}");
            }
            else
            {
                logger.LogInformation($"adding {name} to the refresh list");
            }
            RefreshActions[name] = newRefreshAction;
        }

        public void RemoveRefresh(string name)
        {
            Action outAction;
            logger.LogInformation($"removing {name} from the refresh list");
            RefreshActions.Remove(name, out outAction);
        }

        public void CallRefresh()
        {
            var keys = String.Join(", ", RefreshActions.Keys);
            logger.LogInformation($"calling refresh on {keys}.");
            foreach (var a in RefreshActions)
            {
                RefreshActions[a.Key].Invoke();
            }
        }
    }
}
