#if UNITY_5_6_OR_NEWER && !UNITY_2018_3_OR_NEWER

using System;

namespace UnityEngine.Analytics
{
    public partial class DataPrivacy : MonoBehaviour
    {
#if UNITY_2017_3_OR_NEWER
        internal
#endif
        static class DataPrivacyUtils
        {
            public static void SetOptOutStatus(DataPrivacy.OptOutStatus optOutStatus)
            {
                try
                {
                    Analytics.enabled = Analytics.enabled && optOutStatus.analyticsEnabled;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                try
                {
                    Analytics.deviceStatsEnabled = Analytics.deviceStatsEnabled && optOutStatus.deviceStatsEnabled;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                try
                {
                    Analytics.limitUserTracking = Analytics.limitUserTracking || optOutStatus.limitUserTracking;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                #if UNITY_ANALYTICS
                try
                {
                    PerformanceReporting.enabled = PerformanceReporting.enabled && optOutStatus.performanceReportingEnabled;
                }
                catch (MissingMethodException)
                {
                    // Due to a broken binding, setting PerformanceReporting.enabled throws a
                    // MissingMethodException on 5.6 through 2017.2. A fix will be backported for all
                    // remaining supported versions (2017.1+), so try to set it anyway, and catch the
                    // exception if it happens.
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                #endif
            }

            public static string UserPostDataToJson(DataPrivacy.UserPostData postData)
            {
                return JsonUtility.ToJson(postData);
            }

            public static string OptOutStatusToJson(DataPrivacy.OptOutStatus optOutStatus)
            {
                return JsonUtility.ToJson(optOutStatus);
            }

            public static DataPrivacy.OptOutResponse ParseOptOutResponse(string json)
            {
                return JsonUtility.FromJson<DataPrivacy.OptOutResponse>(json);
            }

            public static DataPrivacy.TokenData ParseTokenData(string json)
            {
                return JsonUtility.FromJson<DataPrivacy.TokenData>(json);
            }

            public static string GetUserId()
            {
    #if UNITY_2017_2_OR_NEWER
                return AnalyticsSessionInfo.userId;
    #else
                return PlayerPrefs.GetString("unity.cloud_userid");
    #endif
            }

            public static long GetSessionId()
            {
    #if UNITY_2017_2_OR_NEWER
                return AnalyticsSessionInfo.sessionId;
    #else
                string sessionId = PlayerPrefs.GetString("unity.player_sessionid");
                long result = 0;
                long.TryParse(sessionId, out result);
                return result;
    #endif
            }

            public static string GetApplicationId()
            {
                return Application.cloudProjectId;
            }

            public static string GetDeviceId()
            {
                return SystemInfo.deviceUniqueIdentifier;
            }
        }
    }
}
#endif //UNITY_5_6_OR_NEWER && !UNITY_2018_3_OR_NEWER
