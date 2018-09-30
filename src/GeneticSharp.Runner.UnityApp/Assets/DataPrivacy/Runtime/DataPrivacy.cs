#if UNITY_2018_3_OR_NEWER
#error "The Data Privacy Plugin is included in the Analytics Library Package and is not longer needed. Please remove this plugin."
#else //UNITY_2018_3_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine.Analytics
{
    public partial class DataPrivacy : MonoBehaviour
    {
        [Serializable]
#if UNITY_2017_3_OR_NEWER
        internal
#endif
        struct UserPostData
        {
            public string appid;
            public string userid;
            public long sessionid;
            public string platform;
            public UInt32 platformid;
            public string sdk_ver;
            public bool debug_device;
            public string deviceid;
            public string plugin_ver;
        }

        // Nested structs must be Serializable for JsonUtility
        [Serializable]
#if UNITY_2017_3_OR_NEWER
        internal
#endif
        struct OptOutStatus
        {
            public bool optOut;
            public bool analyticsEnabled;
            public bool deviceStatsEnabled;
            public bool limitUserTracking;
            public bool performanceReportingEnabled;
        }

        [Serializable]
#if UNITY_2017_3_OR_NEWER
        internal
#endif
        struct RequestData
        {
            public string date;
        }

        [Serializable]
#if UNITY_2017_3_OR_NEWER
        internal
#endif
        struct OptOutResponse
        {
            public RequestData request;
            public OptOutStatus status;
        }

        [Serializable]
#if UNITY_2017_3_OR_NEWER
        internal
#endif
        struct TokenData
        {
            public string url;
            public string token;
        }


        const string kVersion = "1.1.0";
        const string kVersionString = "DataPrivacyPlugin/" + kVersion;

#if UNITY_2017_3_OR_NEWER
        internal
#endif
        const string kBaseUrl = "https://data-optout-service.uca.cloud.unity3d.com";
        const string kOptOutUrl = kBaseUrl + "/player/opt_out";
        const string kTokenUrl = kBaseUrl + "/token";

        const string kPrefAnalyticsEnabled = "data.analyticsEnabled";
        const string kPrefDeviceStatsEnabled = "data.deviceStatsEnabled";
        const string kPrefLimitUserTracking = "data.limitUserTracking";
        const string kPrefPerformanceReportingEnabled = "data.performanceReportingEnabled";
        const string kOptOut = "data.optOut";

        static DataPrivacy instance { get; set; }

        // Use this for initialization
        void Start()
        {
            StartCoroutine(FetchOptOutStatusCoroutine());
        }

        #if UNITY_5_2 || UNITY_5_3 || UNITY_5_4_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        #endif
        public static void Initialize()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DataPrivacy");
                instance = go.AddComponent<DataPrivacy>();
                go.hideFlags = HideFlags.HideAndDontSave;
                Object.DontDestroyOnLoad(go);
            }
        }

        public static void FetchOptOutStatus(Action<bool> optOutAction = null)
        {
            if (instance == null)
            {
                Debug.LogError("You must call Initialize() before calling FetchOptOutStatus!");
                return;
            }
            instance.StartCoroutine(FetchOptOutStatusCoroutine(optOutAction));
        }

        public static void FetchPrivacyUrl(Action<string> success, Action<string> failure = null)
        {
            if (instance == null)
            {
                Debug.LogError("You must call Initialize() before calling FetchPrivacyUrl!");
                return;
            }
            instance.StartCoroutine(FetchPrivacyUrlCoroutine(success, failure));
        }

        static OptOutStatus LoadFromPlayerPrefs()
        {
            OptOutStatus optOutStatus = new OptOutStatus();

            optOutStatus.analyticsEnabled = PlayerPrefs.GetInt(kPrefAnalyticsEnabled, 1) == 1;
            optOutStatus.deviceStatsEnabled = PlayerPrefs.GetInt(kPrefDeviceStatsEnabled, 1) == 1;
            optOutStatus.limitUserTracking = PlayerPrefs.GetInt(kPrefLimitUserTracking, 0) == 1;
            optOutStatus.performanceReportingEnabled = PlayerPrefs.GetInt(kPrefPerformanceReportingEnabled, 1) == 1;
            optOutStatus.optOut = PlayerPrefs.GetInt(kOptOut, 0) == 1;
            return optOutStatus;
        }

        static void SaveToPlayerPrefs(OptOutStatus optOutStatus)
        {
            PlayerPrefs.SetInt(kPrefAnalyticsEnabled, optOutStatus.analyticsEnabled ? 1 : 0);
            PlayerPrefs.SetInt(kPrefDeviceStatsEnabled, optOutStatus.deviceStatsEnabled ? 1 : 0);
            PlayerPrefs.SetInt(kPrefLimitUserTracking, optOutStatus.limitUserTracking ? 1 : 0);
            PlayerPrefs.SetInt(kPrefPerformanceReportingEnabled, optOutStatus.performanceReportingEnabled ? 1 : 0);
            PlayerPrefs.SetInt(kOptOut, optOutStatus.optOut ? 1 : 0);
        }

#if UNITY_2017_3_OR_NEWER
        internal
#endif
        static UserPostData GetUserData()
        {
            var postData = new UserPostData
            {
                appid = DataPrivacyUtils.GetApplicationId(),
                userid = DataPrivacyUtils.GetUserId(),
                sessionid = DataPrivacyUtils.GetSessionId(),
                platform = Application.platform.ToString(),
                platformid = (UInt32)Application.platform,
                sdk_ver = Application.unityVersion,
                debug_device = Debug.isDebugBuild,
                deviceid = DataPrivacyUtils.GetDeviceId(),
                plugin_ver = kVersionString
            };

            return postData;
        }

        static string GetUserAgent()
        {
        #if UNITY_2017_1
            var message = "UnityPlayer/{0} {1}/{2}{3} {4}";
        #else
            var message = "UnityPlayer/{0} ({1}/{2}{3} {4})";
        #endif
            return String.Format(message,
                Application.unityVersion,
                Application.platform.ToString(),
                (UInt32)Application.platform,
                Debug.isDebugBuild ? "-dev" : "",
                kVersionString);
        }

        static IEnumerator FetchOptOutStatusCoroutine(Action<bool> optOutAction = null)
        {
            // Load from player prefs
            var localOptOutStatus = LoadFromPlayerPrefs();
            DataPrivacyUtils.SetOptOutStatus(localOptOutStatus);

            var userData = GetUserData();

            if (string.IsNullOrEmpty(userData.appid))
            {
                Debug.LogError("Could not find AppID for the project!  Make sure you have set your Cloud Project ID in Unity Analytics!");
            }

            if (string.IsNullOrEmpty(userData.userid))
            {
                Debug.LogError("Could not find UserID!  Make sure that you have enabled Unity Analytics for this project");
            }

            if (string.IsNullOrEmpty(userData.deviceid))
            {
                Debug.LogError("Could not find DeviceID!");
            }

            var query = string.Format(kOptOutUrl + "?appid={0}&userid={1}&deviceid={2}", userData.appid, userData.userid, userData.deviceid);
            var baseUri = new Uri(kBaseUrl);
            var uri = new Uri(baseUri, query);

            WWW www = new WWW(uri.ToString(), null, new Dictionary<string, string>()
            {
#if !UNITY_5_3 && !UNITY_WEBGL
                { "User-Agent", GetUserAgent() }
#endif
            });
            yield return www;

            if (!String.IsNullOrEmpty(www.error) || String.IsNullOrEmpty(www.text))
            {
                var error = www.error;
                if (String.IsNullOrEmpty(error))
                {
                    // 5.5 sometimes fails to parse an error response, and the only clue will be
                    // in www.responseHeadersString, which isn't accessible.
                    error = "Empty response";
                }
                Debug.LogWarning(String.Format("Failed to load data opt-opt status from {0}: {1}", www.url, error));
                if (optOutAction != null)
                {
                    optOutAction(localOptOutStatus.optOut);
                }
                yield break;
            }

            OptOutStatus optOutStatus;
            try
            {
                OptOutResponse response = DataPrivacyUtils.ParseOptOutResponse(www.text);
                optOutStatus = response.status;
            }
            catch (Exception e)
            {
                Debug.LogWarning(String.Format("Failed to load data opt-opt status from {0}: {1}", www.url, e.ToString()));
                if (optOutAction != null)
                {
                    optOutAction(localOptOutStatus.optOut);
                }
                yield break;
            }

            DataPrivacyUtils.SetOptOutStatus(optOutStatus);
            SaveToPlayerPrefs(optOutStatus);

            Debug.Log("Opt-out preferences successfully retrieved, applied and saved:\n" +
                DataPrivacyUtils.OptOutStatusToJson(optOutStatus));
            if (optOutAction != null)
            {
                optOutAction(optOutStatus.optOut);
            }
        }

#if UNITY_2017_3_OR_NEWER
        internal
#endif
        static IEnumerator FetchPrivacyUrlCoroutine(Action<string> success, Action<string> failure = null)
        {
            string postJson = DataPrivacyUtils.UserPostDataToJson(GetUserData());
            byte[] bytes = Encoding.UTF8.GetBytes(postJson);

            WWW www = new WWW(kTokenUrl, bytes, new Dictionary<string, string>()
            {
                { "Content-Type", "application/json" },
#if !UNITY_5_3 && !UNITY_WEBGL
                { "User-Agent", GetUserAgent() }
#endif
            });
            //request.chunkedTransfer = false; // needed for WWW in 2017.3?
            yield return www;

            if (!String.IsNullOrEmpty(www.error) || String.IsNullOrEmpty(www.text))
            {
                var error = www.error;
                if (String.IsNullOrEmpty(error))
                {
                    // 5.5 sometimes fails to parse an error response, and the only clue will be
                    // in www.responseHeadersString, which isn't accessible.
                    error = "Empty response";
                }
                if (failure != null)
                {
                    failure(error);
                    yield break;
                }
            }
            else
            {
                TokenData tokenData;
                try
                {
                    tokenData = DataPrivacyUtils.ParseTokenData(www.text);
                }
                catch (Exception e)
                {
                    failure(e.ToString());
                    yield break;
                }

                success(tokenData.url);
            }
        }
    }
}
#endif //UNITY_2018_3_OR_NEWER
