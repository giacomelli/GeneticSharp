#if !UNITY_2018_3_OR_NEWER
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.Analytics
{
    public class DataPrivacyButton : Button
    {
        bool urlOpened = false;

        DataPrivacyButton()
        {
            onClick.AddListener(OpenDataPrivacyUrl);
        }

        void OnFailure(string reason)
        {
            interactable = true;
            Debug.LogWarning(String.Format("Failed to get data privacy url: {0}", reason));
        }

        void OpenUrl(string url)
        {
            interactable = true;
            urlOpened = true;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Application.ExternalEval("window.open(\"" + url + "\",\"_blank\")");
        #else
            Application.OpenURL(url);
        #endif
        }

        void OpenDataPrivacyUrl()
        {
            interactable = false;
            DataPrivacy.FetchPrivacyUrl(OpenUrl, OnFailure);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && urlOpened)
            {
                urlOpened = false;
                DataPrivacy.FetchOptOutStatus();
            }
        }
    }
}
#endif //!UNITY_2018_3_OR_NEWER
