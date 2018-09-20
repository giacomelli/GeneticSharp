#if !UNITY_5_6_OR_NEWER
using System.Reflection;
using System;
using UnityEngine;
using System.Collections.Generic;

#if !UNITY_5_3 && !UNITY_5_4_OR_NEWER
using LegacyJson = UnityEngine.Analytics.DataPrivacyLegacySupport.SimpleJson;
#endif

#if !UNITY_5_2 && !UNITY_5_3 && !UNITY_5_4_OR_NEWER
using UnityEngine.Cloud.Analytics;
#endif

#if NETFX_CORE
using Windows.Storage;
using Windows.Foundation;
using Windows.ApplicationModel;
#endif

namespace UnityEngine.Analytics
{
    public partial class DataPrivacy : MonoBehaviour
    {
        static class DataPrivacyUtils
        {
            public static void SetOptOutStatus(DataPrivacy.OptOutStatus optOutStatus)
            {
                Type sessionConfigType = null;
                object sessionConfig = GetSessionConfigAndType(out sessionConfigType);
                if (sessionConfig == null || sessionConfigType == null)
                {
                    return;
                }

                FieldInfo enabledField = GetFieldUniversal(sessionConfigType, "m_AnalyticsEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
                if (enabledField == null)
                {
                    return;
                }

                var currentValue = (bool)enabledField.GetValue(sessionConfig);
                enabledField.SetValue(sessionConfig, currentValue && optOutStatus.analyticsEnabled);
            }

            public static string UserPostDataToJson(DataPrivacy.UserPostData postData)
            {
                return string.Format("{{\"appid\": \"{0}\", \"userid\": \"{1}\", \"sessionid\": {2}, \"platform\": \"{3}\", \"platformid\": {4}, \"sdk_ver\": \"{5}\", \"debug_device\": {6}, \"deviceid\": \"{7}\", \"plugin_ver\": \"{8}\" }}",
                    postData.appid,
                    postData.userid,
                    postData.sessionid,
                    postData.platform,
                    postData.platformid,
                    postData.sdk_ver,
                    postData.debug_device ? "true" : "false",
                    postData.deviceid,
                    postData.plugin_ver);
            }

            public static string OptOutStatusToJson(DataPrivacy.OptOutStatus optOutStatus)
            {
                return string.Format("{{\"OptOut\": {0},\"analyticsEnabled\": {1}, \"deviceStatsEnabled\": {2}, \"limitUserTracking\": {3}, \"performanceReportingEnabled\": {4} }}",
                    optOutStatus.optOut ? "true" : "false",
                    optOutStatus.analyticsEnabled ? "true" : "false",
                    optOutStatus.deviceStatsEnabled ? "true" : "false",
                    optOutStatus.limitUserTracking ? "true" : "false",
                    optOutStatus.performanceReportingEnabled ? "true" : "false");
            }

    #if UNITY_5_2 || UNITY_5_3 || UNITY_5_4_OR_NEWER
            const string kAnalyticsAssemblyName = "UnityEngine.Analytics";
            const string kAnalyticsClassName = "Analytics";
            static readonly string[] kAnalyticsClassAssemblies = { "UnityEngine.Analytics" };
    #else
            const string kAnalyticsAssemblyName = "UnityEngine.Cloud.Analytics";
            const string kAnalyticsClassName = "UnityAnalytics";
            static readonly string[] kAnalyticsClassAssemblies = { "Assembly-CSharp-firstpass", "Assembly-CSharp" };
    #endif

    #if NETFX_CORE
            static MethodInfo GetMethodUniversal(Type type, string methodString, BindingFlags flags, Type[] types = null)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo == null)
                {
                    return null;
                }

                if (types == null)
                {
                    return typeInfo.GetDeclaredMethod(methodString);
                }
                return typeInfo.GetDeclaredMethod(methodString);
            }

            static PropertyInfo GetPropertyUniversal(Type type, string property, BindingFlags flags)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo == null)
                {
                    return null;
                }
                return typeInfo.GetDeclaredProperty(property);
            }

            static FieldInfo GetFieldUniversal(Type type, string property, BindingFlags flags)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo == null)
                {
                    return null;
                }
                return typeInfo.GetDeclaredField(property);
            }

    #else
            static MethodInfo GetMethodUniversal(Type type, string methodString, BindingFlags flags, Type[] types = null)
            {
                if (types == null)
                {
                    return type.GetMethod(methodString, flags);
                }
                return type.GetMethod(methodString, flags,
                    null,
                    types,
                    null);
            }

            static PropertyInfo GetPropertyUniversal(Type type, string property, BindingFlags flags)
            {
                return type.GetProperty(property, flags);
            }

            static FieldInfo GetFieldUniversal(Type type, string property, BindingFlags flags)
            {
                return type.GetField(property, flags);
            }

    #endif

            static object GetSessionImplField(string fieldName)
            {
                Type analyticsType = null;
                for (int i = 0; i < kAnalyticsClassAssemblies.Length; ++i)
                {
                    analyticsType = Type.GetType(string.Format("{0}.{1}, {2}", kAnalyticsAssemblyName, kAnalyticsClassName, kAnalyticsClassAssemblies[i]));
                    if (analyticsType != null)
                    {
                        break;
                    }
                }

                if (analyticsType == null)
                {
                    return null;
                }

                Type sessionImplType = Type.GetType(string.Format("{0}.SessionImpl, {0}", kAnalyticsAssemblyName));
                if (sessionImplType == null)
                {
                    return null;
                }

                MethodInfo info = GetMethodUniversal(analyticsType, "GetSingleton", BindingFlags.Static | BindingFlags.NonPublic);
                if (info == null)
                {
                    return null;
                }

                object sessionImpl = info.Invoke(null, null);
                if (sessionImpl == null)
                {
                    return null;
                }

                FieldInfo fieldInfo = GetFieldUniversal(sessionImplType, fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                {
                    return null;
                }

                return fieldInfo.GetValue(sessionImpl);
            }

            static object GetSessionConfigAndType(out Type sessionConfigType)
            {
                sessionConfigType = Type.GetType(string.Format("{0}.SessionConfig, {0}", kAnalyticsAssemblyName));
                if (sessionConfigType == null)
                {
                    return null;
                }
                return GetSessionImplField("m_SessionConfig");
            }

            static object GetSessionInfoAndType(out Type sessionInfoType)
            {
                sessionInfoType = Type.GetType(string.Format("{0}.SessionInfo, {0}", kAnalyticsAssemblyName));
                if (sessionInfoType == null)
                {
                    return null;
                }
                return GetSessionImplField("m_SessionInfo");
            }

    #if UNITY_5_3 || UNITY_5_4_OR_NEWER

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
    #if UNITY_5_3
                return (string)GetSessionImplField("m_UserId");
    #else
                return PlayerPrefs.GetString("unity.cloud_userid");
    #endif
            }

            public static long GetSessionId()
            {
    #if UNITY_5_3
                Type sessionInfoType = null;
                object sessionInfo = GetSessionInfoAndType(out sessionInfoType);
                if (sessionInfo == null || sessionInfoType == null)
                {
                    return 0;
                }

                PropertyInfo sessionIdProperty = GetPropertyUniversal(sessionInfoType, "sessionId", BindingFlags.Public | BindingFlags.Instance);
                if (sessionIdProperty == null)
                {
                    return 0;
                }

                return (long)sessionIdProperty.GetValue(sessionInfo, null);
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
    #if UNITY_ANDROID
                PropertyInfo deviceIdProperty = GetPropertyUniversal(typeof(SystemInfo), "deviceUniqueIdentifier", BindingFlags.Static | BindingFlags.Public);
                return (string)deviceIdProperty.GetValue(null, null);
    #else
                return SystemInfo.deviceUniqueIdentifier;
    #endif
            }

    #else //UNITY_5_3 || UNITY_5_4_OR_NEWER

            public static DataPrivacy.OptOutResponse ParseOptOutResponse(string json)
            {
                var dict = (IDictionary<string, object>)LegacyJson.SimpleJson.DeserializeObject(json);

                var response = new DataPrivacy.OptOutResponse();

                if (dict.ContainsKey("request"))
                {
                    var requestDict = (IDictionary<string, object>)dict["request"];
                    if (requestDict.ContainsKey("date"))
                    {
                        response.request.date = requestDict["date"].ToString();
                    }
                    else
                    {
                        response.request.date = string.Empty;
                    }
                }

                if (!dict.ContainsKey("status"))
                {
                    // No status section, just return what we've go so far, like JsonUtility
                    return response;
                }

                var statusDict = (IDictionary<string, object>)dict["status"];
                if (statusDict.ContainsKey("optOut"))
                {
                    response.status.optOut = bool.Parse(statusDict["optOut"].ToString());
                }

                if (statusDict.ContainsKey("analyticsEnabled"))
                {
                    response.status.analyticsEnabled = bool.Parse(statusDict["analyticsEnabled"].ToString());
                }

                if (statusDict.ContainsKey("deviceStatsEnabled"))
                {
                    response.status.deviceStatsEnabled = bool.Parse(statusDict["deviceStatsEnabled"].ToString());
                }

                if (statusDict.ContainsKey("limitUserTracking"))
                {
                    response.status.limitUserTracking = bool.Parse(statusDict["limitUserTracking"].ToString());
                }

                if (statusDict.ContainsKey("performanceReportingEnabled"))
                {
                    response.status.performanceReportingEnabled = bool.Parse(statusDict["performanceReportingEnabled"].ToString());
                }

                return response;
            }

            public static DataPrivacy.TokenData ParseTokenData(string json)
            {
                var dict = (IDictionary<string, object>)LegacyJson.SimpleJson.DeserializeObject(json);
                DataPrivacy.TokenData token = new DataPrivacy.TokenData();

                if (dict.ContainsKey("url"))
                {
                    token.url = dict["url"].ToString();
                }
                if (dict.ContainsKey("token"))
                {
                    token.token = dict["token"].ToString();
                }

                return token;
            }

            public static string GetUserId()
            {
                return (string)GetSessionImplField("m_UserId");
            }

            public static long GetSessionId()
            {
                Type sessionInfoType = null;
                object sessionInfo = GetSessionInfoAndType(out sessionInfoType);
                if (sessionInfo == null || sessionInfoType == null)
                {
                    return 0;
                }

                PropertyInfo sessionIdProperty = GetPropertyUniversal(sessionInfoType, "sessionId", BindingFlags.Public | BindingFlags.Instance);
                if (sessionIdProperty == null)
                {
                    return 0;
                }

                return (long)sessionIdProperty.GetValue(sessionInfo, null);
            }

            public static string GetApplicationId()
            {
    #if UNITY_5_2
                return Application.cloudProjectId;
    #else
                return (string)GetSessionImplField("m_AppId");
    #endif
            }

            public static string GetDeviceId()
            {
    #if UNITY_5_2
    #if UNITY_ANDROID
                PropertyInfo deviceIdProperty = GetPropertyUniversal(typeof(SystemInfo), "deviceUniqueIdentifier", BindingFlags.Static | BindingFlags.Public);
                return (string)deviceIdProperty.GetValue(null, null);
    #else
                return SystemInfo.deviceUniqueIdentifier;
    #endif
    #else //UNITY_5_2
                Type systemInfoType = Type.GetType("UnityEngine.Cloud.Analytics.ISystemInfo, UnityEngine.Cloud.Analytics");
                if (systemInfoType == null)
                {
                    return null;
                }

                PropertyInfo deviceIdProperty = GetPropertyUniversal(systemInfoType, "deviceUniqueIdentifier", BindingFlags.Public | BindingFlags.Instance);
                if (deviceIdProperty == null)
                {
                    return null;
                }

                object systemInfo = GetSessionImplField("m_SystemInfo");
                if (systemInfo == null)
                {
                    return null;
                }

                return (string)deviceIdProperty.GetValue(systemInfo, null);
    #endif //UNITY_5_2
            }

    #endif //UNITY_5_3 || UNITY_5_4_OR_NEWER
        }
    }
}
#endif //!UNITY_5_6_OR_NEWER
