using GameAnalyticsSDK.Net;
using System;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEdit.Services
{
    public class GameAnalyticsRemoteConfig: Config
    {
        private readonly TimeSpan configDownloadTimeout = TimeSpan.FromSeconds(5);

        public GameAnalyticsRemoteConfig() : base() { }

        protected override Task downloadAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() => {
                var startTime = DateTime.Now;
                var timeoutTime = startTime + configDownloadTimeout;
                while (!GameAnalytics.IsRemoteConfigsReady() && DateTime.Now.CompareTo(timeoutTime) < 0)
                {
                    Console.WriteLine("Waiting for configs...");
                }
                if (GameAnalytics.IsRemoteConfigsReady())
                {
                    stableReleaseVersionString = GameAnalytics.GetRemoteConfigsValueAsString("STABLE_VER", string.Empty);
                    betaReleaseVersionString = GameAnalytics.GetRemoteConfigsValueAsString("BETA_VER", string.Empty);
                    isConfigsReady = true;
                }
                tcs.SetResult(isConfigsReady);
            });
            return tcs.Task;
        }

    }
}
