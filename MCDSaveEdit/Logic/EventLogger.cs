using GameAnalyticsSDK.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#nullable enable

namespace MCDSaveEdit
{
    public class EventLogger
    {
        public static void init()
        {
#if DEBUG
            GameAnalytics.SetEnabledInfoLog(true);
            GameAnalytics.SetEnabledVerboseLog(false);
            GameAnalytics.SetEnabledEventSubmission(false);
#else
            GameAnalytics.SetEnabledInfoLog(true);
            GameAnalytics.SetEnabledVerboseLog(false);
            GameAnalytics.SetEnabledEventSubmission(true);
#endif
            GameAnalytics.ConfigureBuild(Constants.CURRENT_VERSION.ToString());

            //Comment out this line or fill in your own GameAnalytics game key and secret key
            GameAnalytics.Initialize(Secrets.GAME_ANALYTICS_GAME_KEY, Secrets.GAME_ANALYTICS_SECRET_KEY);
            GameAnalytics.StartSession();
        }

        public static void dispose()
        {
            GameAnalytics.EndSession();
        }

        public static void logEvent(string eventId, IDictionary<string, object>? fields = null)
        {
            if(fields != null)
            {
                string fieldsStr = string.Join(" ", fields.Select(pair => $"{pair.Key}={pair.Value}"));
                Debug.WriteLine($"[EVENT] {eventId} fields: {fieldsStr}");
            }
            else
            {
                Debug.WriteLine($"[EVENT] {eventId}");
            }
            GameAnalytics.AddDesignEvent(eventId, fields);
        }

        public static void logEvent(string eventId, double value)
        {
            Debug.WriteLine($"[EVENT] {eventId} value={value}");
            GameAnalytics.AddDesignEvent(eventId, value);
        }

        public static void logError(string message)
        {
            Debug.WriteLine($"[ERROR] {message}");
#if !DEBUG
            GameAnalytics.AddErrorEvent(EGAErrorSeverity.Error, message);
#endif
        }
    }
}
