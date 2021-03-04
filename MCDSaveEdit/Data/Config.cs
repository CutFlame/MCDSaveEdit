using GameAnalyticsSDK.Net;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEdit
{
    public class Config
    {
        public static Config instance = new GameAnalyticsRemoteConfig();

        public bool showInventoryIndexOrEquipmentSlot {
            get {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public virtual async Task<bool> isNewVersionAvailable()
        {
            string latest = await latestReleaseVersionString();
            return string.IsNullOrWhiteSpace(latest) || Constants.CURRENT_VERSION_NUMBER.Equals(latest);
        }

        public virtual async Task<string> latestReleaseVersionString()
        {
            //Using GitHub
            try
            {
                var request = WebRequest.Create(Constants.LATEST_RELEASE_GITHUB_URL);
                using var response = await request.GetResponseAsync();
                return response.ResponseUri.Segments.Last();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return string.Empty;
        }

        public virtual string newVersionDownloadURL()
        {
            return Constants.LATEST_RELEASE_GITHUB_URL;
        }
    }

    public class GameAnalyticsRemoteConfig: Config
    {
        public override Task<string> latestReleaseVersionString()
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Run(() => {
                if (GameAnalytics.IsRemoteConfigsReady())
                {
                    var latestVersionString = GameAnalytics.GetRemoteConfigsValueAsString("LATEST_VER", string.Empty);
                    tcs.SetResult(latestVersionString);
                }
                else
                {
                    tcs.SetResult(string.Empty);
                }
            });
            return tcs.Task;
        }

        public override string newVersionDownloadURL()
        {
            if (GameAnalytics.IsRemoteConfigsReady())
            {
                var useMod = GameAnalytics.GetRemoteConfigsValueAsString("USE_MOD_URL", string.Empty);
                if (!string.IsNullOrWhiteSpace(useMod))
                {
                    return string.Format(Constants.LATEST_RELEASE_MOD_URL_FORMAT, useMod);
                }
            }
            return base.newVersionDownloadURL();
        }

    }
}
