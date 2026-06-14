// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

#if NET9_0_OR_GREATER
global using LockObject = System.Threading.Lock;
#else
global using LockObject = System.Object;
#endif

namespace PBLivingston.VoicemeeterAPI;

internal static class SemaphoreExt
{
    public static async Task<Scope> EnterScopeAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync(cancellationToken);
        return new Scope(semaphore);
    }

    public static Scope EnterScope(this SemaphoreSlim semaphore)
    {
        semaphore.Wait();
        return new Scope(semaphore);
    }

    public readonly struct Scope(SemaphoreSlim semaphore) : IDisposable
    {
        private readonly SemaphoreSlim semaphore = semaphore;
        public void Dispose()
            => this.semaphore?.Release();
    }
}

#if NET9_0_OR_GREATER
#else
internal static class LockExt
{
    public static Scope EnterScope(this LockObject lockObject)
        => new(lockObject);

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
