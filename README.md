# A .NET Library for the Voicemeeter Remote API built on A-tG's Dynamic Wrapper

This library is meant to ease interaction with Voicemeeter. It provides:

- Typed methods for interacting with VoicemeeterRemote via A-tG's [dynamic wrapper](https://github.com/A-tG/Voicemeeter-Remote-API-dll-dynamic-wrapper)
- An object model representing Voicemeeter parameters and commands
- An object model representing Voicemeeter MacroButtons
- Brief, informative console messages and exceptions

Notable changes will be documented in the [CHANGELOG](CHANGELOG.md).

## Prerequisites

- [Voicemeeter](https://voicemeeter.com/) must be installed on the target machine.
- The Voicemeeter Remote DLL (`VoicemeeterRemote.dll`) must be present and accessible.

### Tested Against

Voicemeeter December 2025 release:

- Voicemeeter Standard 1.1.2.2
- Voicemeeter Banana 2.1.2.2
- Voicemeeter Potato 3.1.2.2

## Target Frameworks

.NET Standard 2.0, compatible with:

- .NET Core 2.0+
- .NET Framework 4.6.1+
- .NET 5.0+

## Usage

### Remote

The `Remote` class provides access to the Voicemeeter Remote API.

Optional parameter:

- `wrapper`: an instance of `AtgDev.Voicemeeter.RemoteApiWrapper`

  or

- `dllPath`: a custom path string to the Voicemeeter Remote DLL

The following properties are available:

- `LoginStatus`: `LoginResponse`; Possible values: `Ok, VoicemeeterNotRunning, LoggedOut, Unknown`
- `LoggedIn`: `bool`; `true` if `LoginStatus` is either `Ok` or `VoicemeeterNotRunning`, otherwise `false`
- `RunningKind`: `Kind`; Possible values: `Standard, Banana, Potato, None, Unknown`
- `RunningVersion`: `VmVersion`

The following methods are available (see below for details):

- `Login()`: `void`; Updates `LoginStatus` on successful login.
- `Logout()`: `void`; Updates `LoginStatus` on timeout or successful logout.
- `Run(app)`: `void`; Launches the application specified by `app`: `int`, `App`, `Kind`, `string`.
- `GetKind()`: `Kind`; Returns the currently running Voicemeeter `Kind`.
- `GetVersion()`: `VmVersion`; Returns the currently running Voicemeeter `VmVersion`.

`Login()` must be called before any other methods, and `Logout()` should be called when finished.

example:

```csharp
using VoicemeeterAPI;

using var vmr = new Remote();
try
{
    vmr.Login();

    // Perform operations with VoicemeeterRemote
}
finally
{
    vmr.Logout();
}
```

#### Login()

Opens communication pipe with VoicemeeterRemote. Updates `LoginStatus` on successful login.

Throws if the pipe is already open.

#### Logout()

Closes communication pipe with VoicemeeterRemote. Updates `LoginStatus` on timeout or successful logout.

Does nothing if the pipe is already closed.

#### Run(app)

Launches the specified application.

`app` parameter can be an `int`, `App`, `Kind`, or `string` representing the application to launch.

#### GetKind()

Returns the currently running Voicemeeter `Kind`. Possible values: `Standard, Banana, Potato`.

Ensures `LoginStatus` is `Ok` if the call is successful.

Throws if not logged in or API response is an error.

Using the `RunningKind` property will reduce thrown exceptions, as it only calls `GetKind()` if `LoginStatus` is `Ok`, but this will not be accurate if Voicemeeter was launched by an external process. `RunningKind` returns `None` if `LoginStatus` is `VoicemeeterNotRunning` and `Unknown` if `LoginStatus` is otherwise not `Ok`.

#### GetVersion()

Returns the currently running Voicemeeter `VmVersion`.

Ensures `LoginStatus` is `Ok` if the call is successful.

Throws if not logged in or API response is an error.

As with `RunningKind`, using the `RunningVersion` property will reduce thrown exceptions but can fall prone to innaccuracy in unexpected circumstances. `RunningVersion` returns a `VmVersion` "0.0.0.0" if `LoginStatus` is `VoicemeeterNotRunning` and a `VmVersion` "255.0.0.0" if `LoginStatus` is otherwise not `Ok`.

## Licensing

- This library is licensed under MPL-2.0.
- You may use this library (modified or not) in commercially and non-commercially distributed proprietary applications.
- If you use this library (modified or not) in your app, you must:
  - Include the MPL-2.0 license (LICENSE) with your distribution.
  - Provide a link to the library's source.
- If you modify any files from this repository, you must provide the source of those modified files to your recipients and retain copyright/license headers.
- Full text: <https://www.mozilla.org/MPL/2.0/>

### Voicemeeter SDK License

- The Voicemeeter Remote SDK is MIT with one restriction: any created application must interact with the VoicemeeterRemote API.
- The SDK license is included verbatim in this repository (vmr-license.txt).

## Notices

- Source: <https://github.com/pblivingston/voicemeeter-api-net>

<!-- not published yet - NuGet: <https://www.nuget.org/packages/VoicemeeterAPI> -->

### Dependencies

- [a-tg.VmrapiDynWrap](https://github.com/A-tG/Voicemeeter-Remote-API-dll-dynamic-wrapper) (MIT)
- Microsoft.Win32.Registry (MIT)
- Microsoft.SourceLink.GitHub (MIT)

### References

- A-tG's [wrapper extension](https://github.com/A-tG/voicemeeter-remote-api-extended)

Other .NET wrappers:

- [VoiceMeeter.NET](https://github.com/sidewinder94/VoiceMeeter.NET) by [sidewinder94](https://github.com/sidewinder94)
- [VoicemeeterRemote](https://github.com/bobhelander/VoicemeeterRemote) by [Bob Helander](https://github.com/bobhelander)

Wrappers by [Onyx and Iris](https://github.com/onyx-and-iris):

- [Powershell](https://github.com/onyx-and-iris/voicemeeter-api-powershell)
- [Python](https://github.com/onyx-and-iris/voicemeeter-api-python)
- [Ruby](https://github.com/onyx-and-iris/voicemeeter-rb)
- [Go](https://github.com/onyx-and-iris/voicemeeter)

Voicemeeter Remote API documentation:

- [VoicemeeterRemoteAPI.pdf](https://github.com/vburel2018/Voicemeeter-SDK/blob/main/VoicemeeterRemoteAPI.pdf)
- [C header file](https://github.com/vburel2018/Voicemeeter-SDK/blob/main/VoicemeeterRemote.h)

### Acknowledgements
