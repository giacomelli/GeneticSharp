# Changelog

## [1.1.0] - 2018-09-07

### Added
- Added DataPrivacyButton prefab.
- Added Data Privacy icon to button text.

### Removed
- Removed GameObject/UI/Data Privacy Button menu item, in favor of prefab.

### Fixed
- FetchOptOutStatus's optOutAction callback was not called if JsonUtility failed
  to parse the response from the server.
- Fixed exception when using .NET runtime caused by structs not being
  serializable.


## [1.0.1] - 2018-05-18

### Changed
- Plugin now points at production service endpoint.
- Resized button to fit text.

### Fixed
- Fixed PerformanceReporting crash on Android/iOS when Analytics is disabled.
- Don't override User-Agent header on WebGL.
- Fixed the missing UNITY_5_3_OR_NEWER define in versions less than 5.3.4.
- Fixed JSON parsing exception for IL2CPP platforms on 5.2 and prior by
  including SimpleJson parser for those versions.
- Fixed bug accessing userid and deviceid on UWP.


## [1.0.0] - 2018-05-17

### Added
- Added error logs if API calls are made without userid, appid, or deviceid.

### Changed
- Renamed DataManagement -> DataPrivacy
- Renamed DataButton -> DataPrivacyButton
- Renamed GetDataUrl -> FetchPrivacyUrl
- Updated Button menu item from "Data Opt-Out Button" to "Data Privacy Button"
- Updated Button text from "Open Data Controls" to "Open Data Privacy Page"
- Changed methods, structs, and constants that are not part of the public API
  from public to private/internal.
- Making plugin throw error in 2018.3 or newer to show that it has been included
  in the Analytics Library package
- If developer overrides analytics flags and the service returns more-premissive
  settings, respect the more-restrictive developer settings.

### Fixed
- Fixed how we access deviceid for legacy versions.
- Fixed parsing JSON responses for legacy versions.
- Added TouchInputModule to button for Unity versions 5.2 and older.
- Fixed button so that it becomes non-interactible while request is in flight.
- Fixed how we acces deviceid on Android.
- Handle silent WWW failure on opt_out request, in addition to token request.
- Fixed obscure User-Agent header on 5.3 and 2017.1.


## [0.1.2] - 2018-05-16

### Added
- Support for Unity 5.6.
- Support for Unity 5.2.
- Support for Unity 4.7 with the Analytics plugin available on the Asset Store.

### Changed
- The PerformanceReporting.enabled binding error will be caught as a known
  exception and reported as a Warning in the Console, as opposed to an Error.

### Fixed
- Data OptOut Button disables itself while request is in flight, so it can't be
  clicked repeatedly.
- Fixed minor typo in Debug.Log message.
- Clicking the Data Opt Out Button on WebGL will open it in a new tab.
- Work around a bug where 5.5 doesn't realize that a WWW request fails, so it
  doesn't report an error, but has an empty response.


## [0.1.1] - 2018-05-14

### Added
- Add User-Agent header to requests with platform and plugin version info.

### Changed
- Point plugin to staging server by default.
- Update API endpoints to new schema. /opt_out -> /player/opt_out

### Fixed
- Always set `request.chunkedTransfer = false` due to a bug in 2017.3 that
  enabled it by default. This manifests as a missing "Content-Length" request
  header.


## [0.1.0] - 2018-05-11

### Added
- Query opt-out status upon game startup and apply settings.
- Provide button to fetch and open personal data URL in brower.
- Add tests.
