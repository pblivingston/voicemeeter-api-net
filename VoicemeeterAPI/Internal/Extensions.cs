// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Internal;

internal static class SemaphoreExt
{
    public static async Task<Scope> EnterScopeAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken)
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
