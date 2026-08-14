// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Internal;

#if NET9_0_OR_GREATER
#else
internal class LockObject
{
    public Scope EnterScope()
        => new(this);

    public readonly ref struct Scope : IDisposable
    {
        private readonly LockObject lockObject;

        public Scope(LockObject lockObject)
        {
            this.lockObject = lockObject;
            Monitor.Enter(this.lockObject);
        }

        public void Dispose()
            => Monitor.Exit(this.lockObject);
    }
}
#endif
