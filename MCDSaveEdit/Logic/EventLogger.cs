using GameAnalyticsSDK.Net;
using System;
using System.Collections.Generic;
#nullable enable

namespace MCDSaveEdit
{

    public class EventLogger
    {
        public static void init()
        {
#if DEBUG
            GameAnalytics.SetEnabledInfoLog(true);
#endif
            GameAnalytics.ConfigureBuild(Constants.CURRENT_RELEASE_TAG_NAME);

            //Comment out this line or fill in your own GameAnalytics game key and secret key
            GameAnalytics.Initialize(Secrets.GAME_ANALYTICS_GAME_KEY, Secrets.GAME_ANALYTICS_SECRET_KEY);
        }

        public static void logEvent(string eventId, IDictionary<string, object>? fields = null)
        {
            GameAnalytics.AddDesignEvent(eventId, fields);
        }
        public static void logEvent(string eventId, double value)
        {
            GameAnalytics.AddDesignEvent(eventId, value);
        }
    }
}
