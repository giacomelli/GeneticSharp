# Unity Analytics Data Privacy Plugin

Use this Unity Analytics Data Privacy plugin to help achieve compliance 
with the European Union General Data Protection Regulation (GDPR). 
The plugin provides APIs to allow players to manage their 
data privacy preferences. Integrate this Data Privacy plugin if you use 
Unity services such as Analytics, IAP, Performance Reporting, or Multiplayer. 
Do not integrate this plugin if you use the Unity Ads service — Unity Ads 
provides data privacy options to players built in to any Ads you display.

The tools provided by this plugin allow you to open a web page where 
your players can view the data Unity services have collected about them, 
delete that data, and opt-out from collection of personal data.

For complete documentation about integrating this Data Privacy plugin, see 
[Unity Analytics and the EU General Data Protection Regulation (GDPR)]
(https://docs.unity3d.com/Manual/UnityAnalyticsDataPrivacy.html).

For general information about Unity and GDPR, see 
[Unity Statement on GDPR Readiness](https://unity3d.com/legal/gdpr).

## Compatibility

This version of the plugin is compatible with all versions of Unity
that support Analytics, from 4.7 onward.

**Important:** If a player has a browser pop-up blocker enabled, their browser 
can prevent the data privacy page from opening. Some browsers note that a page 
has been blocked, but others provide no notice at all. Consider adding a message 
in your user interface that warns players that a pop-up blocker can prevent the 
page from opening.

The following Unity versions have known problems:

- On Unity versions 5.6 through 2017.2, Unity throws an exception when the 
   plugin attempts to set the `PerformanceReporting.enabled` flag (due to a bad binding). 
  This exception is caught and ignored. A fix for the `PerformanceReporting.enabled` binding
   is currently being back ported to all supported versions (2017.1 and 2017.2) and
   will land in subsequent Unity patch releases.

- When using use the separate Unity Analytics plugin (Unity 5.1 or earlier), you must manually 
  initialize the plugin. Initialize the Data Privacy plugin immediately after you start the Analytics SDK:
  
        void Start () {
            const string projectId = "SAMPLE-UNITY-PROJECT-ID";
            UnityAnalytics.StartSDK (projectId);
            DataPrivacy.Initialize ();
        }


## About

The plugin has two main parts:

* DataPrivacy class -- checks the player's current data privacy preference and 
  configures Unity services accordingly. Also provides functions for fetching 
  the URL of the player's personal data privacy page.
* DataPrivacyButton prefab -- provides a Button object you can add to your UI, 
  which opens the player's data privacy web page. Players can set their data 
  privacy preferences on this web page. 

### UnityEngine.Analytics.DataPrivacy

`UnityEngine.Analytics.DataPrivacy` is a static class defined in
`DataPrivacy.cs`that serves two purposes:

1. It automatically checks the player's opt-out status at the start of a
   game and enables or disables personally identifiable data collection.
   - This happens just by nature of having this file in your project's Assets,
     which is accomplished via a `RuntimeInitializeOnLoad` attribute. Nothing needs to
     be added to a scene.
  - Note that when using use the separate Unity Analytics plugin (Unity 5.1 or earlier), you must 
    manually initialize the Data Privacy plugin by calling `DataPrivacy.Initialize()` immediately 
    after you start the Analytics plugin.

2. It provides two main APIs:
  - `FetchPrivacyUrl(Action<string> success, Action<string> failure = null)`
    - This fetches a personalized URL from our web server that a player may
      visit to view the data Unity Analytics has collected about them, request
      that data be deleted, or opt-out from collection of personal data. This
      URL expires after a short amount of time, so always fetch the URL immediately 
      before opening it.
    - `success` is a callback function that is called upon successful
      retrieval of the URL, with the URL passed as its argument.
    - `failure` is an optional function called if the game fails to retrieve
      the URL.  An error message describing the reason is passed as its argument.
  - `FetchOptOutStatus(Action<bool> optOutAction = null)`  queries our server for the 
      player's opt-out status, and disables certain data collection as necessary. 
      - `FetchOptOutStatus` is automatically called upon startup. 
      - You can also call `FetchOptOutStatus()` yourself to fetch the 
        player's current data collection preference. The `optOutAction` function you provide is called
        when the network fetch request is complete. The argument passed to your action is true when
        the player has opted out and false, otherwise.

                void OnOptOutStatus(bool optOut)
                {
                    if(optOut)
                    {
                        Debug.Log("Player has opted-out of personal data collection.");
                    }
                }
                DataPrivacy.FetchOptOutStatus(OnOptOutStatus);

### UnityEngine.Analytics.DataPrivacyButton

`DataPrivacyButton.cs` provides a child class of `UnityEngine.UI.Button` that
automatically registers a click handler to fetch the player's personalized
data URL and open it in a browser window.

You can add the button to a scene through the following means:

- Add the DataPrivacyButton prefab to a Canvas object in a scene
- Replace the Button component of an existing button GameObject with
  DataPrivacyButton.cs

## Usage

1. To query the player's opt-out status at app startup, simply ensure that
   `DataPrivacy.cs` exists in your project's Assets directory.

2. To provide the player with a link to their personal data URL and options to
   opt out of personal data collection, either add the provided
   DataPrivacyButton to your game's UI and style as necessary, or use the
   `UnityEngine.Analytics.DataPrivacy.FetchPrivacyUrl` API to fetch the URL and
   present it how you see fit.

**Important:** If you are using Unity 5.1 or earlier (which require the separate Unity 
Analytics plugin), you must call `DataPrivacy.Initialize()` manually. Call `Initialize()`
immediately after you call the `UnityAnalytics.StartSDK ()` function.

