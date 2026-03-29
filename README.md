# A .NET Library for the Voicemeeter Remote API built on A-tG's Dynamic Wrapper

This library is meant to ease interaction with Voicemeeter. It provides:

- Typed methods for interacting with VoicemeeterRemote via A-tG's [dynamic wrapper](https://github.com/A-tG/Voicemeeter-Remote-API-dll-dynamic-wrapper)
- An object model representing Voicemeeter parameters and commands
- An object model representing Voicemeeter MacroButtons
- Brief, informative console messages and exceptions

Notable changes will be documented in the [CHANGELOG](CHANGELOG.md).

Although this library may function in its current state, it is still in early development and not currently intended for use.

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

## Features

### Remote

The `Remote` class provides access to the Voicemeeter Remote API.

#### Usage

`Login()` must be called before any other methods, and `Logout()` should be called when finished.

example:

```csharp
using PBLivingston.VoicemeeterAPI;

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

#### Highlights

- If Voicemeeter is running, `Remote.Login()` ensures the running instance can be reached.
- `Remote.Run(app)` can be used with `App` or `Kind` enums, strings, or integers and handles OS bitness and process-start polling automatically.
- `Remote.GetVersion()` returns a `VmVersion` struct backed by the 32-bit integer returned by the API, providing easy comparison, string representation, access to individual version components, etc.
- All `Remote` methods are protected by a state-aware `LoginGuard()` to prevent ghost API calls.

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
