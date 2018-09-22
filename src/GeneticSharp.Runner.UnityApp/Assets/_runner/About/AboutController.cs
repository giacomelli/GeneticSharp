using System;
using UnityEngine;
using UnityEngine.Analytics;

public class AboutController : MonoBehaviour {

	public void OpenGeneticSharpUrl()
    {
        Application.OpenURL("https://github.com/giacomelli/GeneticSharp");
    }

    public void OpenAuthorBlogUrl()
    {
        Application.OpenURL("http://diegogiacomelli.com.br");
    }

    public void OpenDataPrivacyUrl()
    {
        DataPrivacy.FetchPrivacyUrl(OnURLReceived, OnFailure);
    }

    static void OnFailure(string reason)
    {
        Debug.LogWarning(String.Format("Failed to get data privacy page URL: {0}", reason));
    }

    void OnURLReceived(string url)
    {
        Application.OpenURL(url);
    }
}