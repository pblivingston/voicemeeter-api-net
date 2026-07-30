// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.InteropServices;
using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;

public partial class Remote
{
    /// <summary>
    ///   Implements the <see cref="IWrapper"/> interface to abstract underlying calls to the VoicemeeterRemote API.
    /// </summary>
    /// <param name="remoteApiWrapper"><see cref="RemoteApiWrapper"/></param>
    /// <remarks>
    ///   The primary constructor initializes a new instance of the <see cref="Wrapper"/> class with a provided <see cref="RemoteApiWrapper"/>.
    /// </remarks>
    private sealed partial class Wrapper : IWrapper
    {
        private const string VmrName = "VoicemeeterRemote";

        private readonly RemoteApiWrapper remoteApiWrapper;
        private readonly Dictionary<App, ProcessWrapper> apps = [];

        public bool Is64Bit { get; } = Environment.Is64BitProcess;
        public string InstallDir { get; } = GetInstallDirectory();

        private string DllName => VmrName + (this.Is64Bit ? "64.dll" : ".dll");

        public Wrapper(RemoteApiWrapper remoteApiWrapper)
        {
            this.remoteApiWrapper = remoteApiWrapper
                ?? throw new ArgumentNullException(nameof(remoteApiWrapper));

            this.InitApps();
        }

        public Wrapper(string installDir)
        {
            this.InstallDir = installDir;
            this.remoteApiWrapper = new RemoteApiWrapper(Path.Combine(this.InstallDir, this.DllName));

            this.InitApps();
        }

        public Wrapper()
        {
            this.remoteApiWrapper = new RemoteApiWrapper(Path.Combine(this.InstallDir, this.DllName));

            this.InitApps();
        }

        public void Dispose()
        {
            this.ReleaseApps();

            this.remoteApiWrapper.Dispose();
        }

        private static string GetInstallDirectory()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException("Cannot get Voicemeeter installation path on current OS");
            }

            return PathHelper.GetProgramFolder();
        }
    }
}
