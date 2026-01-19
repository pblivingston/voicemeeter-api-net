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

The following methods are available (see below for details):

- `Login()`: `void`; Updates `LoginStatus` on successful login.
- `Logout()`: `void`; Updates `LoginStatus` on timeout or successful logout.
- `RunVoicemeeter(kind, ms = 2000)`: `void`; Launches the Voicemeeter application specified by `kind`: `int`, `Kind`, `string`.
- `GetVoicemeeterKind()`: `Kind`; Returns the currently running Voicemeeter `Kind`.

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

If API response is "Ok", optional `ms` parameter specifies how long to wait for the running Voicemeeter instance to respond before throwing. Default is 2000ms.

#### Logout()

Closes communication pipe with VoicemeeterRemote. Updates `LoginStatus` on timeout or successful logout.

Does nothing if the pipe is already closed.

Optional `ms` parameter specifies for how long logout re-attempts will be made before giving up. Default is 1000ms.

#### RunVoicemeeter(kind, ms = 2000)

Launches the specified Voicemeeter application.

`kind` parameter can be an `int`, `Kind`, or `string` representing the Voicemeeter application to launch.

If the requested application is a Voicemeeter kind (e.g. `3`, `Kind.Bananax64`, `"Standard"`, etc.), optional `ms` parameter specifies how long to wait in milliseconds for the launched Voicemeeter instance to respond before throwing.

#### GetVoicemeeterKind()

Returns the currently running Voicemeeter `Kind`. Possible values: `Standard, Banana, Potato`.

Ensures `LoginStatus` is `Ok` if the call is successful.

Throws if not logged in or API response is an error.

Using the `RunningKind` property will reduce thrown exceptions, as it only calls `GetVoicemeeterKind()` if `LoginStatus` is `Ok`, but this will not be accurate if Voicemeeter was launched by an external process. `RunningKind` returns `None` if `LoginStatus` is `VoicemeeterNotRunning` and `Unknown` if `LoginStatus` is otherwise not `Ok`.

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

[not published yet]: # (- NuGet: <https://www.nuget.org/packages/VoicemeeterAPI>)

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